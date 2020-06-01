using System.Collections.Generic;
using System.Linq;
using Microsoft.Crm.Sdk.Messages;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;

namespace Synchronous_Event_Order.Logic
{
    internal class SynchronousWorkflow : ISynchronousEvent
    {
        private readonly Entity workflow;

        private int initialRank;

        public SynchronousWorkflow(Entity workflow)
        {
            this.workflow = workflow;
            initialRank = Rank;

            // Stage
            if (workflow.GetAttributeValue<bool>("triggeroncreate"))
            {
                var stageCode = workflow.GetAttributeValue<OptionSetValue>("createstage");
                Stage = stageCode != null ? stageCode.Value : 40;
                Message = "Create";
            }
            else if (workflow.Contains("triggeronupdateattributelist") &&
                     workflow.GetAttributeValue<string>("triggeronupdateattributelist") != null)
            {
                var stageCode = workflow.GetAttributeValue<OptionSetValue>("updatestage");
                Stage = stageCode != null ? stageCode.Value : 40;
                Message = "Update";
            }
            else if (workflow.GetAttributeValue<bool>("triggerondelete"))
            {
                var stageCode = workflow.GetAttributeValue<OptionSetValue>("deletestage");
                Stage = stageCode != null ? stageCode.Value : 20;
                Message = "Delete";
            }
        }


        public int Rank
        {
            get => workflow.GetAttributeValue<int>("rank");
            set => workflow["rank"] = value;
        }

        public string EntityLogicalName => workflow.GetAttributeValue<string>("primaryentity");

        public int Stage { get; }

        public string Message { get; }


        public string Name => workflow.GetAttributeValue<string>("name");

        public string UpdateAttributes => workflow.GetAttributeValue<string>("triggeronupdateattributelist");

        public string Description => workflow.GetAttributeValue<string>("description");

        public void UpdateRank(IOrganizationService service)
        {
            if (HasChanged)
            {
                var wf = service.Retrieve("workflow", workflow.Id, new ColumnSet("statecode"));
                if (wf.GetAttributeValue<OptionSetValue>("statecode").Value != 0)
                    service.Execute(new SetStateRequest
                    {
                        EntityMoniker = wf.ToEntityReference(),
                        State = new OptionSetValue(0),
                        Status = new OptionSetValue(-1)
                    });

                workflow.Attributes.Remove("statecode");
                workflow.Attributes.Remove("statuscode");
                service.Update(workflow);

                if (wf.GetAttributeValue<OptionSetValue>("statecode").Value != 0)
                    service.Execute(new SetStateRequest
                    {
                        EntityMoniker = wf.ToEntityReference(),
                        State = new OptionSetValue(1),
                        Status = new OptionSetValue(-1)
                    });
                initialRank = workflow.GetAttributeValue<int>("rank");
            }
        }

        public bool HasChanged => initialRank != Rank;

        public string Type => "Workflow";

        public static IEnumerable<SynchronousWorkflow> RetrievePluginSteps(IOrganizationService service)
        {
            var qba = new QueryByAttribute("workflow")
            {
                Attributes = {"mode", "type", "category", "statuscode"},
                Values = {1, 1, 0, 2},
                ColumnSet = new ColumnSet(true)
            };

            var steps = service.RetrieveMultiple(qba);

            return steps.Entities.Select(e => new SynchronousWorkflow(e));
        }
    }
}