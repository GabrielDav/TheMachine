namespace GameEditor.TriggerEditor
{
    partial class TriggerWindow
    {
        /// <summary> 
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.Windows.Forms.TreeNode treeNode1 = new System.Windows.Forms.TreeNode("Events");
            System.Windows.Forms.TreeNode treeNode2 = new System.Windows.Forms.TreeNode("Conditions", 0, 0);
            System.Windows.Forms.TreeNode treeNode3 = new System.Windows.Forms.TreeNode("Actions");
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(TriggerWindow));
            this.triggerTreeView = new System.Windows.Forms.TreeView();
            this.imageList1 = new System.Windows.Forms.ImageList(this.components);
            this.rootMenuStrip = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.addEventToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.addConditionToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.addActionToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.itemsMenuStrip = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.editStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.deleteToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.moveUpToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.moveDownToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.rootMenuStrip.SuspendLayout();
            this.itemsMenuStrip.SuspendLayout();
            this.SuspendLayout();
            // 
            // triggerTreeView
            // 
            this.triggerTreeView.Dock = System.Windows.Forms.DockStyle.Fill;
            this.triggerTreeView.ImageIndex = 0;
            this.triggerTreeView.ImageList = this.imageList1;
            this.triggerTreeView.Location = new System.Drawing.Point(0, 0);
            this.triggerTreeView.Name = "triggerTreeView";
            treeNode1.ImageKey = "Event.png";
            treeNode1.Name = "Events";
            treeNode1.SelectedImageIndex = 1;
            treeNode1.Text = "Events";
            treeNode2.ImageIndex = 0;
            treeNode2.Name = "Conditions";
            treeNode2.SelectedImageIndex = 0;
            treeNode2.Text = "Conditions";
            treeNode3.ImageIndex = 2;
            treeNode3.Name = "Actions";
            treeNode3.SelectedImageKey = "Run.png";
            treeNode3.Text = "Actions";
            this.triggerTreeView.Nodes.AddRange(new System.Windows.Forms.TreeNode[] {
            treeNode1,
            treeNode2,
            treeNode3});
            this.triggerTreeView.SelectedImageIndex = 0;
            this.triggerTreeView.Size = new System.Drawing.Size(555, 344);
            this.triggerTreeView.TabIndex = 0;
            this.triggerTreeView.DoubleClick += new System.EventHandler(this.TriggerTreeViewDoubleClick);
            this.triggerTreeView.MouseDown += new System.Windows.Forms.MouseEventHandler(this.TriggerTreeViewMouseDown);
            // 
            // imageList1
            // 
            this.imageList1.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("imageList1.ImageStream")));
            this.imageList1.TransparentColor = System.Drawing.Color.Transparent;
            this.imageList1.Images.SetKeyName(0, "Condition.png");
            this.imageList1.Images.SetKeyName(1, "Event.png");
            this.imageList1.Images.SetKeyName(2, "Run.png");
            this.imageList1.Images.SetKeyName(3, "Regions.png");
            this.imageList1.Images.SetKeyName(4, "RegionType.png");
            this.imageList1.Images.SetKeyName(5, "Check.png");
            this.imageList1.Images.SetKeyName(6, "Camera.png");
            this.imageList1.Images.SetKeyName(7, "Timer.png");
            this.imageList1.Images.SetKeyName(8, "IntegerValue.png");
            this.imageList1.Images.SetKeyName(9, "FloatValue.png");
            this.imageList1.Images.SetKeyName(10, "Object.png");
            this.imageList1.Images.SetKeyName(11, "Property.png");
            this.imageList1.Images.SetKeyName(12, "Point.png");
            this.imageList1.Images.SetKeyName(13, "Pointer.png");
            this.imageList1.Images.SetKeyName(14, "Mechanics.png");
            this.imageList1.Images.SetKeyName(15, "Trigger.png");
            this.imageList1.Images.SetKeyName(16, "String.png");
            this.imageList1.Images.SetKeyName(17, "Sound.png");
            this.imageList1.Images.SetKeyName(18, "Back.png");
            // 
            // rootMenuStrip
            // 
            this.rootMenuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.addEventToolStripMenuItem,
            this.addConditionToolStripMenuItem,
            this.addActionToolStripMenuItem});
            this.rootMenuStrip.Name = "rootMenuStrip";
            this.rootMenuStrip.Size = new System.Drawing.Size(153, 70);
            this.rootMenuStrip.Opening += new System.ComponentModel.CancelEventHandler(this.RootMenuStripOpening);
            // 
            // addEventToolStripMenuItem
            // 
            this.addEventToolStripMenuItem.Enabled = false;
            this.addEventToolStripMenuItem.Image = global::GameEditor.IconsResource.Event;
            this.addEventToolStripMenuItem.ImageTransparentColor = System.Drawing.Color.Fuchsia;
            this.addEventToolStripMenuItem.Name = "addEventToolStripMenuItem";
            this.addEventToolStripMenuItem.Size = new System.Drawing.Size(152, 22);
            this.addEventToolStripMenuItem.Text = "Add Event";
            this.addEventToolStripMenuItem.Click += new System.EventHandler(this.AddEventToolStripMenuItemClick);
            // 
            // addConditionToolStripMenuItem
            // 
            this.addConditionToolStripMenuItem.Enabled = false;
            this.addConditionToolStripMenuItem.Image = global::GameEditor.IconsResource.Condition;
            this.addConditionToolStripMenuItem.Name = "addConditionToolStripMenuItem";
            this.addConditionToolStripMenuItem.Size = new System.Drawing.Size(152, 22);
            this.addConditionToolStripMenuItem.Text = "Add Condition";
            this.addConditionToolStripMenuItem.Click += new System.EventHandler(this.AddConditionToolStripMenuItemClick);
            // 
            // addActionToolStripMenuItem
            // 
            this.addActionToolStripMenuItem.Enabled = false;
            this.addActionToolStripMenuItem.Image = global::GameEditor.IconsResource.Run;
            this.addActionToolStripMenuItem.Name = "addActionToolStripMenuItem";
            this.addActionToolStripMenuItem.Size = new System.Drawing.Size(152, 22);
            this.addActionToolStripMenuItem.Text = "Add Action";
            this.addActionToolStripMenuItem.Click += new System.EventHandler(this.AddActionToolStripMenuItemClick);
            // 
            // itemsMenuStrip
            // 
            this.itemsMenuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.editStripMenuItem,
            this.deleteToolStripMenuItem,
            this.moveUpToolStripMenuItem,
            this.moveDownToolStripMenuItem});
            this.itemsMenuStrip.Name = "contextMenuStrip1";
            this.itemsMenuStrip.Size = new System.Drawing.Size(139, 92);
            this.itemsMenuStrip.Opening += new System.ComponentModel.CancelEventHandler(this.ItemsMenuStripOpening);
            // 
            // editStripMenuItem
            // 
            this.editStripMenuItem.Image = global::GameEditor.IconsResource.Edit;
            this.editStripMenuItem.Name = "editStripMenuItem";
            this.editStripMenuItem.Size = new System.Drawing.Size(138, 22);
            this.editStripMenuItem.Text = "Edit";
            this.editStripMenuItem.Click += new System.EventHandler(this.EditStripMenuItemClick);
            // 
            // deleteToolStripMenuItem
            // 
            this.deleteToolStripMenuItem.Image = global::GameEditor.IconsResource.Delete;
            this.deleteToolStripMenuItem.Name = "deleteToolStripMenuItem";
            this.deleteToolStripMenuItem.Size = new System.Drawing.Size(138, 22);
            this.deleteToolStripMenuItem.Text = "Delete";
            this.deleteToolStripMenuItem.Click += new System.EventHandler(this.DeleteToolStripMenuItemClick);
            // 
            // moveUpToolStripMenuItem
            // 
            this.moveUpToolStripMenuItem.Image = global::GameEditor.IconsResource.Up;
            this.moveUpToolStripMenuItem.Name = "moveUpToolStripMenuItem";
            this.moveUpToolStripMenuItem.Size = new System.Drawing.Size(138, 22);
            this.moveUpToolStripMenuItem.Text = "Move Up";
            this.moveUpToolStripMenuItem.Click += new System.EventHandler(this.MoveUpToolStripMenuItemClick);
            // 
            // moveDownToolStripMenuItem
            // 
            this.moveDownToolStripMenuItem.Image = global::GameEditor.IconsResource.Down;
            this.moveDownToolStripMenuItem.Name = "moveDownToolStripMenuItem";
            this.moveDownToolStripMenuItem.Size = new System.Drawing.Size(138, 22);
            this.moveDownToolStripMenuItem.Text = "Move Down";
            this.moveDownToolStripMenuItem.Click += new System.EventHandler(this.MoveDownToolStripMenuItemClick);
            // 
            // TriggerWindow
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.triggerTreeView);
            this.Name = "TriggerWindow";
            this.Size = new System.Drawing.Size(555, 344);
            this.rootMenuStrip.ResumeLayout(false);
            this.itemsMenuStrip.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TreeView triggerTreeView;
        private System.Windows.Forms.ContextMenuStrip rootMenuStrip;
        private System.Windows.Forms.ToolStripMenuItem addEventToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem addConditionToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem addActionToolStripMenuItem;
        private System.Windows.Forms.ContextMenuStrip itemsMenuStrip;
        private System.Windows.Forms.ToolStripMenuItem deleteToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem editStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem moveUpToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem moveDownToolStripMenuItem;
        public System.Windows.Forms.ImageList imageList1;
    }
}
