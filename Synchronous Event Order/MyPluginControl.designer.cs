namespace Synchronous_Event_Order
{
    partial class MyPluginControl
    {
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MyPluginControl));
            this.toolStrip1 = new System.Windows.Forms.ToolStrip();
            this.tsbClose = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this.tsbLoadEvents = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.tsbUpdate = new System.Windows.Forms.ToolStripButton();
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
            this.toolStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tsbClose,
            this.toolStripSeparator2,
            this.tsbLoadEvents,
            this.toolStripSeparator1,
            this.tsbUpdate});
            this.toolStrip1.Location = new System.Drawing.Point(0, 0);
            this.toolStrip1.Name = "toolStrip1";
            this.toolStrip1.Size = new System.Drawing.Size(731, 25);
            this.toolStrip1.TabIndex = 1;
            this.toolStrip1.Text = "toolStrip1";
            // 
            // tsbClose
            // 
            this.tsbClose.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.tsbClose.Image = ((System.Drawing.Image)(resources.GetObject("tsbClose.Image")));
            this.tsbClose.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tsbClose.Name = "tsbClose";
            this.tsbClose.Size = new System.Drawing.Size(23, 22);
            this.tsbClose.Text = "Close this tool";
            this.tsbClose.Click += new System.EventHandler(this.tsbClose_Click);
            // 
            // toolStripSeparator2
            // 
            this.toolStripSeparator2.Name = "toolStripSeparator2";
            this.toolStripSeparator2.Size = new System.Drawing.Size(6, 25);
            // 
            // tsbLoadEvents
            // 
            this.tsbLoadEvents.Image = global::Synchronous_Event_Order.Properties.Resources.tsbLoadEvents_Image;
            this.tsbLoadEvents.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tsbLoadEvents.Name = "tsbLoadEvents";
            this.tsbLoadEvents.Size = new System.Drawing.Size(90, 22);
            this.tsbLoadEvents.Text = "Load events";
            this.tsbLoadEvents.Click += new System.EventHandler(this.tsbLoadEvents_Click);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(6, 25);
            // 
            // tsbUpdate
            // 
            this.tsbUpdate.Image = ((System.Drawing.Image)(resources.GetObject("tsbUpdate.Image")));
            this.tsbUpdate.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tsbUpdate.Name = "tsbUpdate";
            this.tsbUpdate.Size = new System.Drawing.Size(111, 22);
            this.tsbUpdate.Text = "Apply update(s)";
            this.tsbUpdate.Click += new System.EventHandler(this.tsbUpdate_Click);
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
            this.splitContainer1.Location = new System.Drawing.Point(0, 25);
            this.splitContainer1.Margin = new System.Windows.Forms.Padding(2, 3, 2, 3);
            this.splitContainer1.Name = "splitContainer1";
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.tvEvents);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.dgvSynchronousEvent);
            this.splitContainer1.Size = new System.Drawing.Size(731, 472);
            this.splitContainer1.SplitterDistance = 243;
            this.splitContainer1.SplitterWidth = 3;
            this.splitContainer1.TabIndex = 2;
            // 
            // tvEvents
            // 
            this.tvEvents.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tvEvents.ImageIndex = 0;
            this.tvEvents.ImageList = this.ilEventTreeNode;
            this.tvEvents.Location = new System.Drawing.Point(2, 3);
            this.tvEvents.Margin = new System.Windows.Forms.Padding(2, 3, 2, 3);
            this.tvEvents.Name = "tvEvents";
            this.tvEvents.SelectedImageIndex = 0;
            this.tvEvents.Size = new System.Drawing.Size(239, 468);
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
            this.dgvSynchronousEvent.Location = new System.Drawing.Point(2, 3);
            this.dgvSynchronousEvent.Margin = new System.Windows.Forms.Padding(2, 3, 2, 3);
            this.dgvSynchronousEvent.Name = "dgvSynchronousEvent";
            this.dgvSynchronousEvent.RowTemplate.Height = 24;
            this.dgvSynchronousEvent.Size = new System.Drawing.Size(481, 467);
            this.dgvSynchronousEvent.TabIndex = 0;
            this.dgvSynchronousEvent.CellContentClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dgvSynchronousEvent_CellContentClick);
            this.dgvSynchronousEvent.CellValueChanged += new System.Windows.Forms.DataGridViewCellEventHandler(this.dgvSynchronousEvent_CellValueChanged);
            // 
            // dgvSynchronousEventRank
            // 
            this.dgvSynchronousEventRank.HeaderText = "Rank";
            this.dgvSynchronousEventRank.Name = "dgvSynchronousEventRank";
            // 
            // dgvSynchronousEventType
            // 
            this.dgvSynchronousEventType.FillWeight = 150F;
            this.dgvSynchronousEventType.HeaderText = "Type";
            this.dgvSynchronousEventType.Name = "dgvSynchronousEventType";
            this.dgvSynchronousEventType.ReadOnly = true;
            // 
            // dgvSynchronousEventName
            // 
            this.dgvSynchronousEventName.HeaderText = "Name";
            this.dgvSynchronousEventName.Name = "dgvSynchronousEventName";
            this.dgvSynchronousEventName.ReadOnly = true;
            this.dgvSynchronousEventName.Width = 300;
            // 
            // dgvSynchronousDescription
            // 
            this.dgvSynchronousDescription.HeaderText = "Description";
            this.dgvSynchronousDescription.Name = "dgvSynchronousDescription";
            this.dgvSynchronousDescription.ReadOnly = true;
            this.dgvSynchronousDescription.Width = 400;
            // 
            // UpdateAttributes
            // 
            this.UpdateAttributes.HeaderText = "Update Attributes";
            this.UpdateAttributes.Name = "UpdateAttributes";
            this.UpdateAttributes.ReadOnly = true;
            // 
            // MyPluginControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.Controls.Add(this.splitContainer1);
            this.Controls.Add(this.toolStrip1);
            this.Margin = new System.Windows.Forms.Padding(2, 3, 2, 3);
            this.Name = "MyPluginControl";
            this.Size = new System.Drawing.Size(731, 497);
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
    }
}
