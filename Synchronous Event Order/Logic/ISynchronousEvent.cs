using Microsoft.Xrm.Sdk;

namespace Synchronous_Event_Order.Logic
{
    internal interface ISynchronousEvent
    {
        int Rank { get; set; }

        string EntityLogicalName { get; }

        int Stage { get; }

        string Message { get; }

        string Name { get; }

        string Description { get; }

        bool HasChanged { get; }

        string Type { get; }
        string UpdateAttributes { get; }

        void UpdateRank(IOrganizationService service);
    }
}