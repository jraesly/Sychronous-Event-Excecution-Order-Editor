using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Windows.Forms;

namespace Synchronous_Event_Order.Logic
{
    internal class TreeViewHelper
    {
        private readonly TreeView _tv;

        public TreeViewHelper(TreeView tv)
        {
            this._tv = tv;
            tv.Sorted = true;
        }

        public void AddSynchronousEvent(ISynchronousEvent sEvent)
        {
            TreeNode entityNode = _tv.Nodes.Find(sEvent.EntityLogicalName, false).ToList().SingleOrDefault();
            if (entityNode == null)
            {
                entityNode = new TreeNode(sEvent.EntityLogicalName)
                    {ImageIndex = 0, SelectedImageIndex = 0, Name = sEvent.EntityLogicalName};
                _tv.Nodes.Add(entityNode);
            }

            TreeNode messageNode = entityNode.Nodes.Find(sEvent.Message, false).ToList().SingleOrDefault();
            if (messageNode == null)
            {
                messageNode = new TreeNode(sEvent.Message)
                    {ImageIndex = 1, SelectedImageIndex = 1, Name = sEvent.Message};
                entityNode.Nodes.Add(messageNode);
            }

            TreeNode stageNode = messageNode.Nodes.Find(sEvent.Stage.ToString(CultureInfo.InvariantCulture), false).ToList()
                .SingleOrDefault();
            if (stageNode == null)
            {
                string stageName = string.Empty;
                switch (sEvent.Stage)
                {
                    case 10:
                        stageName = "PreValidation";
                        break;
                    case 20:
                        stageName = "PreOperation";
                        break;
                    case 40:
                        stageName = "PostOperation";
                        break;
                }

                stageNode = new TreeNode(stageName)
                {
                    ImageIndex = 2,
                    SelectedImageIndex = 2,
                    Name = sEvent.Stage.ToString(CultureInfo.InvariantCulture),
                    Tag = new List<ISynchronousEvent>()
                };
                messageNode.Nodes.Add(stageNode);
            }

            ((List<ISynchronousEvent>) stageNode.Tag).Add(sEvent);
        }
    }
}