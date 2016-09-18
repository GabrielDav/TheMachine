namespace GameEditor.TriggerEditor
{
    partial class TriggersMenu
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
            this.triggersList = new System.Windows.Forms.ListBox();
            this.triggersListMenuStrip = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.toolStripRenameButton = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripEnableButton = new System.Windows.Forms.ToolStripMenuItem();
            this.removeTriggerButton = new System.Windows.Forms.Button();
            this.addTriggerButton = new System.Windows.Forms.Button();
            this.triggersListMenuStrip.SuspendLayout();
            this.SuspendLayout();
            // 
            // triggersList
            // 
            this.triggersList.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed;
            this.triggersList.FormattingEnabled = true;
            this.triggersList.Location = new System.Drawing.Point(0, 0);
            this.triggersList.Name = "triggersList";
            this.triggersList.Size = new System.Drawing.Size(260, 459);
            this.triggersList.TabIndex = 0;
            this.triggersList.DrawItem += new System.Windows.Forms.DrawItemEventHandler(this.TriggersListDrawItem);
            this.triggersList.SelectedIndexChanged += new System.EventHandler(this.TriggersListSelectedIndexChanged);
            this.triggersList.MouseDown += new System.Windows.Forms.MouseEventHandler(this.TriggersListMouseDown);
            // 
            // triggersListMenuStrip
            // 
            this.triggersListMenuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripRenameButton,
            this.toolStripEnableButton});
            this.triggersListMenuStrip.Name = "triggersListMenuStrip";
            this.triggersListMenuStrip.Size = new System.Drawing.Size(118, 48);
            this.triggersListMenuStrip.Opening += new System.ComponentModel.CancelEventHandler(this.TriggersListMenuStripOpening);
            // 
            // toolStripRenameButton
            // 
            this.toolStripRenameButton.Name = "toolStripRenameButton";
            this.toolStripRenameButton.Size = new System.Drawing.Size(117, 22);
            this.toolStripRenameButton.Text = "Rename";
            this.toolStripRenameButton.Click += new System.EventHandler(this.ToolStripRenameButtonClick);
            // 
            // toolStripEnableButton
            // 
            this.toolStripEnableButton.CheckOnClick = true;
            this.toolStripEnableButton.Name = "toolStripEnableButton";
            this.toolStripEnableButton.Size = new System.Drawing.Size(117, 22);
            this.toolStripEnableButton.Text = "Enabled";
            this.toolStripEnableButton.Click += new System.EventHandler(this.ToolStripEnableButtonClick);
            // 
            // removeTriggerButton
            // 
            this.removeTriggerButton.Enabled = false;
            this.removeTriggerButton.Location = new System.Drawing.Point(0, 465);
            this.removeTriggerButton.Name = "removeTriggerButton";
            this.removeTriggerButton.Size = new System.Drawing.Size(119, 23);
            this.removeTriggerButton.TabIndex = 1;
            this.removeTriggerButton.Text = "Remove";
            this.removeTriggerButton.UseVisualStyleBackColor = true;
            this.removeTriggerButton.Click += new System.EventHandler(this.RemoveTriggerButtonClick);
            // 
            // addTriggerButton
            // 
            this.addTriggerButton.Location = new System.Drawing.Point(141, 465);
            this.addTriggerButton.Name = "addTriggerButton";
            this.addTriggerButton.Size = new System.Drawing.Size(119, 23);
            this.addTriggerButton.TabIndex = 2;
            this.addTriggerButton.Text = "Add";
            this.addTriggerButton.UseVisualStyleBackColor = true;
            this.addTriggerButton.Click += new System.EventHandler(this.AddTriggerButtonClick);
            // 
            // TriggersMenu
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.addTriggerButton);
            this.Controls.Add(this.removeTriggerButton);
            this.Controls.Add(this.triggersList);
            this.Name = "TriggersMenu";
            this.Size = new System.Drawing.Size(260, 490);
            this.triggersListMenuStrip.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button removeTriggerButton;
        private System.Windows.Forms.ListBox triggersList;
        private System.Windows.Forms.Button addTriggerButton;
        private System.Windows.Forms.ContextMenuStrip triggersListMenuStrip;
        private System.Windows.Forms.ToolStripMenuItem toolStripRenameButton;
        private System.Windows.Forms.ToolStripMenuItem toolStripEnableButton;
    }
}
