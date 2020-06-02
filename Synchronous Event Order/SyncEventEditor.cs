﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows.Forms;
using McTools.Xrm.Connection;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using Synchronous_Event_Order.Logic;
using Synchronous_Event_Order.Properties;
using XrmToolBox.Extensibility;
using XrmToolBox.Extensibility.Interfaces;

namespace Synchronous_Event_Order
{
    public class SyncEventEditor : PluginControlBase, IPayPalPlugin
    {
        // Public Properties
        public string DonationDescription => "Synchronous Event Execution Order Editor Fan Club!";
        public string EmailAccount => "jlax58@gmail.com";

        // Private Properties
        private List<ISynchronousEvent> _events;
        private Settings _mySettings;

        public SyncEventEditor()
        {
            InitializeComponent();
        }

        private void MyPluginControl_Load(object sender, EventArgs e)
        {
            ShowInfoNotification("This is a notification that can lead to XrmToolBox repository",
                new Uri("https://github.com/MscrmTools/XrmToolBox"));

            // Loads or creates the settings for the plugin
            if (!SettingsManager.Instance.TryLoad(GetType(), out _mySettings))
            {
                _mySettings = new Settings();

                LogWarning("Settings not found => a new settings file has been created!");
            }
            else
            {
                LogInfo("Settings found and loaded");
            }
        }

        #region Load Events

        private void tsbLoadEvents_Click(object sender, EventArgs e)
        {
            ExecuteMethod(LoadEvents);
        }

        private void LoadEvents()
        {
            tvEvents.Nodes.Clear();

            WorkAsync(new WorkAsyncInfo
            {
                Message = "Loading Sdk message filters...",
                Work = (bw, e) =>
                {
                    _events = new List<ISynchronousEvent>();

                    List<Entity> filters =
                        Service.RetrieveMultiple(new QueryExpression("sdkmessagefilter")
                        {
                            ColumnSet = new ColumnSet("sdkmessageid", "primaryobjecttypecode")
                        }).Entities.ToList();

                    bw.ReportProgress(0, "Loading SDK messages...");

                    List<Entity> messages = Service.RetrieveMultiple(new QueryExpression("sdkmessage")
                    {
                        ColumnSet = new ColumnSet("name")
                    }).Entities.ToList();

                    bw.ReportProgress(25, "Loading Plugin steps...");

                    _events.AddRange(PluginStep.RetrievePluginSteps(Service, filters, messages));

                    bw.ReportProgress(50, "Loading Synchronous workflows...");

                    _events.AddRange(SynchronousWorkflow.RetrieveWorkflowSteps(Service));
                },
                PostWorkCallBack = e =>
                {
                    if (e.Error != null)
                    {
                        MessageBox.Show(ParentForm, "An error occured: " + e.Error, "Error", MessageBoxButtons.OK,
                            MessageBoxIcon.Error);
                    }
                    else
                    {
                        TreeViewHelper tvh = new TreeViewHelper(tvEvents);

                        foreach (ISynchronousEvent sEvent in _events) tvh.AddSynchronousEvent(sEvent);
                    }
                },
                ProgressChanged = e =>
                {
                    // it will display "I have found the user id" in this example
                    SetWorkingMessage(e.UserState.ToString());
                }
            });
        }

        #endregion

        #region Create Event Rows After Select

        private void tvEvents_AfterSelect(object sender, TreeViewEventArgs e)
        {
            dgvSynchronousEvent.Rows.Clear();

            if (e.Node.Nodes.Count > 0) return;

            List<ISynchronousEvent> localEvents = (List<ISynchronousEvent>)e.Node.Tag;

            foreach (ISynchronousEvent sEvent in localEvents)
            {
                DataGridViewRow row = new DataGridViewRow
                {
                    Tag = sEvent
                };
                row.Cells.Add(new DataGridViewTextBoxCell { Value = sEvent.Rank });
                row.Cells.Add(new DataGridViewTextBoxCell { Value = sEvent.Type });
                row.Cells.Add(new DataGridViewTextBoxCell { Value = sEvent.Name });
                row.Cells.Add(new DataGridViewTextBoxCell { Value = sEvent.Description });
                if (sEvent.Message != "Create")
                    row.Cells.Add(new DataGridViewTextBoxCell { Value = sEvent.UpdateAttributes });
                dgvSynchronousEvent.Rows.Add(row);
            }
        }

        #endregion

        #region Rank Update

        private void dgvSynchronousEvent_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            if (dgvSynchronousEvent.Rows.Count == 0) return;
            dgvSynchronousEventRank.ValueType = typeof(int);

            if (int.TryParse(dgvSynchronousEvent.Rows[e.RowIndex].Cells[e.ColumnIndex].Value.ToString(), out var rank))
            {
                var sEvent = (ISynchronousEvent) dgvSynchronousEvent.Rows[e.RowIndex].Tag;
                sEvent.Rank = rank;

                dgvSynchronousEvent.Sort(dgvSynchronousEvent.Columns[e.ColumnIndex], ListSortDirection.Ascending);
            }
            else
            {
                MessageBox.Show(ParentForm, "Only integer value is allowed for rank", "Warning", MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);
            }
        }

        private void tsbUpdate_Click(object sender, EventArgs e)
        {
            var updatedEvents = _events.Where(ev => ev.HasChanged);

            if (updatedEvents.Any(ev => ev.Type == "Workflow") && DialogResult.No ==
                MessageBox.Show(ParentForm,
                    Resources.SyncEventEditor_tsbUpdate_Click_Workflows_will_be_deactivated__updated__then_activated_back__Are_you_sure_you_want_to_continue_,
                    Resources.SyncEventEditor_tsbUpdate_Click_Question, MessageBoxButtons.YesNo, MessageBoxIcon.Question))
                return;

            WorkAsync(new WorkAsyncInfo
            {
                Message = "Updating...",
                Work = (bw, evt) =>
                {
                    foreach (var sEvent in _events.Where(ev => ev.HasChanged))
                    {
                        bw.ReportProgress(0, $"Updating {sEvent.Type} {sEvent.Name}");
                        sEvent.UpdateRank(Service);
                    }
                },
                PostWorkCallBack = evt =>
                {
                    if (evt.Error != null)
                        MessageBox.Show(ParentForm, $"An error occured: {evt.Error.Message}", "Error",
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Error);
                },
                ProgressChanged = evt =>
                {
                    // it will display "I have found the user id" in this example
                    SetWorkingMessage(evt.UserState.ToString());
                }
            });
        }

        #endregion

        #region On Close of Plugin

        private void tsbClose_Click(object sender, EventArgs e)
        {
            CloseTool();
        }


        /// <summary>
        ///     This event occurs when the plugin is closed
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SyncEventExecEditor_OnClose(object sender, EventArgs e)
        {
            // Before leaving, save the settings
            SettingsManager.Instance.Save(GetType(), _mySettings);
        }

        #endregion

        #region Update Connection

        /// <summary>
        ///     This event occurs when the connection has been updated in XrmToolBox
        /// </summary>
        public override void UpdateConnection(IOrganizationService newService, ConnectionDetail detail,
            string actionName, object parameter)
        {
            base.UpdateConnection(newService, detail, actionName, parameter);

            if (_mySettings == null || detail == null) return;
            _mySettings.LastUsedOrganizationWebappUrl = detail.WebApplicationUrl;
            LogInfo("Connection has changed to: {0}", detail.WebApplicationUrl);
        }

        #endregion

        #region Keyboard Shortcuts

        public void ReceiveKeyDownShortcut(KeyEventArgs e)
        {
            if (e.KeyCode == Keys.F5 && tsbLoadEvents.Enabled)
            {
                tsbLoadEvents_Click(null, null);
            }
            else if (e.Control && e.KeyCode == Keys.S && tsbUpdate.Enabled)
            {
                tsbUpdate_Click(null, null);
            }
        }

        #endregion

        // Event to open record?
        private void dgvSynchronousEvent_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
        }

        /// <summary> 
        /// Variable nécessaire au concepteur.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// Nettoyage des ressources utilisées.
        /// </summary>
        /// <param name="disposing">true si les ressources managées doivent être supprimées ; sinon, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Code généré par le Concepteur de composants

        /// <summary> 
        /// Méthode requise pour la prise en charge du concepteur - ne modifiez pas 
        /// le contenu de cette méthode avec l'éditeur de code.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.toolStrip1 = new System.Windows.Forms.ToolStrip();
            this.tsbClose = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this.tsbLoadEvents = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.tsbUpdate = new System.Windows.Forms.ToolStripButton();
            this.toolStripButton1 = new System.Windows.Forms.ToolStripButton();
            this.ilEventTreeNode = new System.Windows.Forms.ImageList(this.components);
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.tvEvents = new System.Windows.Forms.TreeView();
            this.dgvSynchronousEvent = new System.Windows.Forms.DataGridView();
            this.dgvSynchronousEventRank = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dgvSynchronousEventType = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dgvSynchronousEventName = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dgvSynchronousDescription = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.UpdateAttributes = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.toolStrip1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvSynchronousEvent)).BeginInit();
            this.SuspendLayout();
            // 
            // toolStrip1
            // 
            this.toolStrip1.ImageScalingSize = new System.Drawing.Size(32, 32);
            this.toolStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tsbClose,
            this.toolStripSeparator2,
            this.tsbLoadEvents,
            this.toolStripSeparator1,
            this.tsbUpdate,
            this.toolStripButton1});
            this.toolStrip1.Location = new System.Drawing.Point(0, 0);
            this.toolStrip1.Name = "toolStrip1";
            this.toolStrip1.Padding = new System.Windows.Forms.Padding(0, 0, 4, 0);
            this.toolStrip1.Size = new System.Drawing.Size(1462, 42);
            this.toolStrip1.TabIndex = 1;
            this.toolStrip1.Text = "toolStrip1";
            // 
            // tsbClose
            // 
            this.tsbClose.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.tsbClose.Image = global::Synchronous_Event_Order.Properties.Resources.tsbClose_Image;
            this.tsbClose.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tsbClose.Name = "tsbClose";
            this.tsbClose.Size = new System.Drawing.Size(46, 36);
            this.tsbClose.Text = "Close this tool";
            this.tsbClose.Click += new System.EventHandler(this.tsbClose_Click);
            // 
            // toolStripSeparator2
            // 
            this.toolStripSeparator2.Name = "toolStripSeparator2";
            this.toolStripSeparator2.Size = new System.Drawing.Size(6, 42);
            // 
            // tsbLoadEvents
            // 
            this.tsbLoadEvents.Image = global::Synchronous_Event_Order.Properties.Resources.CDS;
            this.tsbLoadEvents.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tsbLoadEvents.Name = "tsbLoadEvents";
            this.tsbLoadEvents.Size = new System.Drawing.Size(225, 36);
            this.tsbLoadEvents.Text = "Load events (F5)";
            this.tsbLoadEvents.Click += new System.EventHandler(this.tsbLoadEvents_Click);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(6, 42);
            // 
            // tsbUpdate
            // 
            this.tsbUpdate.Image = global::Synchronous_Event_Order.Properties.Resources.tsbUpdate_Image;
            this.tsbUpdate.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tsbUpdate.Name = "tsbUpdate";
            this.tsbUpdate.Size = new System.Drawing.Size(218, 36);
            this.tsbUpdate.Text = "Apply update(s)";
            this.tsbUpdate.Click += new System.EventHandler(this.tsbUpdate_Click);
            // 
            // toolStripButton1
            // 
            this.toolStripButton1.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.toolStripButton1.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButton1.Name = "toolStripButton1";
            this.toolStripButton1.Size = new System.Drawing.Size(46, 36);
            this.toolStripButton1.Text = "toolStripButton1";
            // 
            // ilEventTreeNode
            // 
            this.ilEventTreeNode.ColorDepth = System.Windows.Forms.ColorDepth.Depth8Bit;
            this.ilEventTreeNode.ImageSize = new System.Drawing.Size(16, 16);
            this.ilEventTreeNode.TransparentColor = System.Drawing.Color.Transparent;
            // 
            // splitContainer1
            // 
            this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer1.Location = new System.Drawing.Point(0, 42);
            this.splitContainer1.Margin = new System.Windows.Forms.Padding(4, 6, 4, 6);
            this.splitContainer1.Name = "splitContainer1";
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.tvEvents);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.dgvSynchronousEvent);
            this.splitContainer1.Size = new System.Drawing.Size(1462, 914);
            this.splitContainer1.SplitterDistance = 485;
            this.splitContainer1.SplitterWidth = 6;
            this.splitContainer1.TabIndex = 2;
            // 
            // tvEvents
            // 
            this.tvEvents.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tvEvents.ImageIndex = 0;
            this.tvEvents.ImageList = this.ilEventTreeNode;
            this.tvEvents.Location = new System.Drawing.Point(4, 6);
            this.tvEvents.Margin = new System.Windows.Forms.Padding(4, 6, 4, 6);
            this.tvEvents.Name = "tvEvents";
            this.tvEvents.SelectedImageIndex = 0;
            this.tvEvents.Size = new System.Drawing.Size(473, 860);
            this.tvEvents.TabIndex = 1;
            this.tvEvents.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.tvEvents_AfterSelect);
            // 
            // dgvSynchronousEvent
            // 
            this.dgvSynchronousEvent.AllowUserToAddRows = false;
            this.dgvSynchronousEvent.AllowUserToDeleteRows = false;
            this.dgvSynchronousEvent.AllowUserToResizeRows = false;
            this.dgvSynchronousEvent.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.dgvSynchronousEvent.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvSynchronousEvent.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.dgvSynchronousEventRank,
            this.dgvSynchronousEventType,
            this.dgvSynchronousEventName,
            this.dgvSynchronousDescription,
            this.UpdateAttributes});
            this.dgvSynchronousEvent.Location = new System.Drawing.Point(4, 6);
            this.dgvSynchronousEvent.Margin = new System.Windows.Forms.Padding(4, 6, 4, 6);
            this.dgvSynchronousEvent.Name = "dgvSynchronousEvent";
            this.dgvSynchronousEvent.RowHeadersWidth = 82;
            this.dgvSynchronousEvent.RowTemplate.Height = 24;
            this.dgvSynchronousEvent.Size = new System.Drawing.Size(959, 862);
            this.dgvSynchronousEvent.TabIndex = 0;
            this.dgvSynchronousEvent.CellContentClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dgvSynchronousEvent_CellContentClick);
            this.dgvSynchronousEvent.CellValueChanged += new System.Windows.Forms.DataGridViewCellEventHandler(this.dgvSynchronousEvent_CellValueChanged);
            // 
            // dgvSynchronousEventRank
            // 
            this.dgvSynchronousEventRank.HeaderText = "Rank";
            this.dgvSynchronousEventRank.MinimumWidth = 10;
            this.dgvSynchronousEventRank.Name = "dgvSynchronousEventRank";
            this.dgvSynchronousEventRank.Width = 200;
            // 
            // dgvSynchronousEventType
            // 
            this.dgvSynchronousEventType.FillWeight = 150F;
            this.dgvSynchronousEventType.HeaderText = "Type";
            this.dgvSynchronousEventType.MinimumWidth = 10;
            this.dgvSynchronousEventType.Name = "dgvSynchronousEventType";
            this.dgvSynchronousEventType.ReadOnly = true;
            this.dgvSynchronousEventType.Width = 200;
            // 
            // dgvSynchronousEventName
            // 
            this.dgvSynchronousEventName.HeaderText = "Name";
            this.dgvSynchronousEventName.MinimumWidth = 10;
            this.dgvSynchronousEventName.Name = "dgvSynchronousEventName";
            this.dgvSynchronousEventName.ReadOnly = true;
            this.dgvSynchronousEventName.Width = 300;
            // 
            // dgvSynchronousDescription
            // 
            this.dgvSynchronousDescription.HeaderText = "Description";
            this.dgvSynchronousDescription.MinimumWidth = 10;
            this.dgvSynchronousDescription.Name = "dgvSynchronousDescription";
            this.dgvSynchronousDescription.ReadOnly = true;
            this.dgvSynchronousDescription.Width = 400;
            // 
            // UpdateAttributes
            // 
            this.UpdateAttributes.HeaderText = "Update Attributes";
            this.UpdateAttributes.MinimumWidth = 10;
            this.UpdateAttributes.Name = "UpdateAttributes";
            this.UpdateAttributes.ReadOnly = true;
            this.UpdateAttributes.Width = 200;
            // 
            // SyncEventEditor
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(12F, 25F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.Controls.Add(this.splitContainer1);
            this.Controls.Add(this.toolStrip1);
            this.Margin = new System.Windows.Forms.Padding(4, 6, 4, 6);
            this.Name = "SyncEventEditor";
            this.Size = new System.Drawing.Size(1462, 956);
            this.toolStrip1.ResumeLayout(false);
            this.toolStrip1.PerformLayout();
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
            this.splitContainer1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dgvSynchronousEvent)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ToolStrip toolStrip1;
        private System.Windows.Forms.ImageList ilEventTreeNode;
        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.TreeView tvEvents;
        private System.Windows.Forms.ToolStripButton tsbLoadEvents;
        private System.Windows.Forms.DataGridView dgvSynchronousEvent;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripButton tsbUpdate;
        private System.Windows.Forms.ToolStripButton tsbClose;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator2;
        private System.Windows.Forms.DataGridViewTextBoxColumn dgvSynchronousEventRank;
        private System.Windows.Forms.DataGridViewTextBoxColumn dgvSynchronousEventType;
        private System.Windows.Forms.DataGridViewTextBoxColumn dgvSynchronousEventName;
        private System.Windows.Forms.DataGridViewTextBoxColumn dgvSynchronousDescription;
        private System.Windows.Forms.DataGridViewTextBoxColumn UpdateAttributes;
        private System.Windows.Forms.ToolStripButton toolStripButton1;
    }
}