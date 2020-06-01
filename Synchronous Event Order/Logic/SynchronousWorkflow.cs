using System.Collections.Generic;
using System.Linq;
using Microsoft.Crm.Sdk.Messages;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;

namespace Synchronous_Event_Order.Logic
{
    internal partial class SynchronousWorkflow : ISynchronousEvent
    {
        public SynchronousWorkflow(Entity workflow)
        {
            this._workflow = workflow;
            _initialRank = Rank;

            // Stage
            if (workflow.GetAttributeValue<bool>("triggeroncreate"))
            {
                OptionSetValue stageCode = workflow.GetAttributeValue<OptionSetValue>("createstage");
                Stage = stageCode?.Value ?? 40;
                Message = "Create";
            }
            else if (workflow.Contains("triggeronupdateattributelist") &&
                     workflow.GetAttributeValue<string>("triggeronupdateattributelist") != null)
            {
                OptionSetValue stageCode = workflow.GetAttributeValue<OptionSetValue>("updatestage");
                Stage = stageCode?.Value ?? 40;
                Message = "Update";
            }
            else if (workflow.GetAttributeValue<bool>("triggerondelete"))
            {
                OptionSetValue stageCode = workflow.GetAttributeValue<OptionSetValue>("deletestage");
                Stage = stageCode?.Value ?? 20;
                Message = "Delete";
            }
        }


        public void UpdateRank(IOrganizationService service)
        {
            // Return if value has not changed
            if (!HasChanged) return;
            
            // Retrieve workflow 
            Entity wf = service.Retrieve("workflow", _workflow.Id, new ColumnSet("statecode"));
            
            // If Workflow is active then set state to Draft
            if (wf.GetAttributeValue<OptionSetValue>("statecode").Value != 0)
                service.Execute(new SetStateRequest
                {
                    EntityMoniker = wf.ToEntityReference(),
                    State = new OptionSetValue(0),
                    Status = new OptionSetValue(-1)
                });

            _workflow.Attributes.Remove("statecode");
            _workflow.Attributes.Remove("statuscode");
            service.Update(_workflow);

            if (wf.GetAttributeValue<OptionSetValue>("statecode").Value != 0)
                service.Execute(new SetStateRequest
                {
                    EntityMoniker = wf.ToEntityReference(),
                    State = new OptionSetValue(1),
                    Status = new OptionSetValue(-1)
                });
            _initialRank = _workflow.GetAttributeValue<int>("rank");
        }

        public static IEnumerable<SynchronousWorkflow> RetrieveWorkflowSteps(IOrganizationService service)
        {
            QueryByAttribute qba = new QueryByAttribute("workflow")
            {
                Attributes = {"mode", "type", "category", "statuscode"},

                // Mode = Real-time, Type = Definition, Category = Workflow, StatusCode = Activated
                Values = {1, 1, 0, 2},
                ColumnSet = new ColumnSet(true)
            };

            EntityCollection steps = service.RetrieveMultiple(qba);

            return steps.Entities.Select(e => new SynchronousWorkflow(e));
        }
    }

    internal partial class SynchronousWorkflow
    {
        private readonly Entity _workflow;
        private int _initialRank;

        public int Rank
        {
            get => _workflow.GetAttributeValue<int>("rank");
            set => _workflow["rank"] = value;
        }

        public string EntityLogicalName => _workflow.GetAttributeValue<string>("primaryentity");
        public int Stage { get; }
        public string Message { get; }
        public string Name => _workflow.GetAttributeValue<string>("name");
        public string UpdateAttributes => _workflow.GetAttributeValue<string>("triggeronupdateattributelist");
        public string Description => _workflow.GetAttributeValue<string>("description");
        public bool HasChanged => _initialRank != Rank;
        public string Type => "Workflow";
    }
}