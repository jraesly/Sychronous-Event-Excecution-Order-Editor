using System.Collections.Generic;
using System.Linq;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;

namespace Synchronous_Event_Order.Logic
{
    internal class PluginStep : ISynchronousEvent
    {
        private readonly Entity pluginStep;
        private int initialRank;

        public PluginStep(Entity pluginStep, IEnumerable<Entity> sdkMessageFilers, IEnumerable<Entity> sdkMessages)
        {
            this.pluginStep = pluginStep;
            initialRank = Rank;

            // EntityLogicalName
            Entity messageFilter = sdkMessageFilers.FirstOrDefault(
                s => pluginStep.GetAttributeValue<EntityReference>("sdkmessagefilterid") != null &&
                     s.Id == pluginStep.GetAttributeValue<EntityReference>("sdkmessagefilterid").Id);
            if (messageFilter != null)
            {
                EntityLogicalName = messageFilter.GetAttributeValue<string>("primaryobjecttypecode");

                if (EntityLogicalName.Length == 0) EntityLogicalName = "None";

                Entity message = sdkMessages.FirstOrDefault(
                    m => m.Id == messageFilter.GetAttributeValue<EntityReference>("sdkmessageid").Id);
                if (message != null) Message = message.GetAttributeValue<string>("name");
            }
            else
            {
                EntityLogicalName = "(none)";

                Entity message = sdkMessages.FirstOrDefault(
                    m => m.Id == pluginStep.GetAttributeValue<EntityReference>("sdkmessageid").Id);
                if (message != null) Message = message.GetAttributeValue<string>("name");
            }
        }

        public int Rank
        {
            get => pluginStep.GetAttributeValue<int>("rank");
            set => pluginStep["rank"] = value;
        }

        public string EntityLogicalName { get; }

        public int Stage => pluginStep.GetAttributeValue<OptionSetValue>("stage").Value;

        public string Message { get; }

        public string Name => pluginStep.GetAttributeValue<string>("name");

        public string Description => pluginStep.GetAttributeValue<string>("description");
        public string UpdateAttributes => pluginStep.GetAttributeValue<string>("filteringattributes");

        public void UpdateRank(IOrganizationService service)
        {
            if (HasChanged)
            {
                service.Update(pluginStep);
                initialRank = pluginStep.GetAttributeValue<int>("rank");
            }
        }


        public bool HasChanged => initialRank != Rank;

        public string Type => "Plugin step";

        public static IEnumerable<PluginStep> RetrievePluginSteps(IOrganizationService service,
            IEnumerable<Entity> sdkMessageFilers, IEnumerable<Entity> sdkMessages)
        {
            QueryExpression qe = new QueryExpression("sdkmessageprocessingstep")
            {
                ColumnSet = new ColumnSet(true),
                Criteria =
                {
                    Conditions =
                    {
                        new ConditionExpression("mode", ConditionOperator.Equal, 0)
                    }
                },
                LinkEntities =
                {
                    new LinkEntity
                    {
                        LinkFromEntityName = "sdkmessageprocessingstep",
                        LinkFromAttributeName = "plugintypeid",
                        LinkToAttributeName = "plugintypeid",
                        LinkToEntityName = "plugintype",
                        LinkCriteria =
                        {
                            Conditions =
                            {
                                new ConditionExpression("typename", ConditionOperator.NotLike, "Microsoft.Crm.%"),
                                new ConditionExpression("typename", ConditionOperator.NotLike, "Compiled.%")
                            }
                        }
                    }
                }
            };
            // Add link-entity QEsdkmessageprocessingstep_sdkmessagefilter
            //var QEsdkmessageprocessingstep_sdkmessagefilter = qe.AddLink("sdkmessagefilter", "sdkmessagefilterid", "sdkmessagefilterid");

            // Define filter QEsdkmessageprocessingstep_sdkmessagefilter.LinkCriteria
            //QEsdkmessageprocessingstep_sdkmessagefilter.LinkCriteria.AddCondition("primaryobjecttypecode", ConditionOperator.Equal, 10037);

            EntityCollection steps = service.RetrieveMultiple(qe);

            return steps.Entities.Select(e => new PluginStep(e, sdkMessageFilers, sdkMessages));
        }
    }
}