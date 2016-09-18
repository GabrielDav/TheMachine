namespace GameEditor.TriggerEditor
{
    partial class TriggerEditorDialog
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

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.triggerWindow1 = new GameEditor.TriggerEditor.TriggerWindow();
            this.triggersBox1 = new GameEditor.TriggerEditor.TriggersMenu();
            this.toolStrip = new System.Windows.Forms.ToolStrip();
            this.toolStripUndoButton = new System.Windows.Forms.ToolStripButton();
            this.toolStripRedoButton = new System.Windows.Forms.ToolStripButton();
            this.toolStrip.SuspendLayout();
            this.SuspendLayout();
            // 
            // triggerWindow1
            // 
            this.triggerWindow1.Location = new System.Drawing.Point(278, 28);
            this.triggerWindow1.Name = "triggerWindow1";
            this.triggerWindow1.Size = new System.Drawing.Size(636, 459);
            this.triggerWindow1.TabIndex = 1;
            // 
            // triggersBox1
            // 
            this.triggersBox1.Location = new System.Drawing.Point(12, 28);
            this.triggersBox1.Name = "triggersBox1";
            this.triggersBox1.Size = new System.Drawing.Size(260, 496);
            this.triggersBox1.TabIndex = 0;
            // 
            // toolStrip
            // 
            this.toolStrip.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden;
            this.toolStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripUndoButton,
            this.toolStripRedoButton});
            this.toolStrip.Location = new System.Drawing.Point(0, 0);
            this.toolStrip.Name = "toolStrip";
            this.toolStrip.RenderMode = System.Windows.Forms.ToolStripRenderMode.System;
            this.toolStrip.Size = new System.Drawing.Size(926, 25);
            this.toolStrip.TabIndex = 2;
            this.toolStrip.Text = "toolStrip";
            // 
            // toolStripUndoButton
            // 
            this.toolStripUndoButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.toolStripUndoButton.Enabled = false;
            this.toolStripUndoButton.Image = global::GameEditor.IconsResource.Edit_Undo;
            this.toolStripUndoButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripUndoButton.Name = "toolStripUndoButton";
            this.toolStripUndoButton.Size = new System.Drawing.Size(23, 22);
            this.toolStripUndoButton.Text = "Undo";
            this.toolStripUndoButton.ToolTipText = "Undo";
            this.toolStripUndoButton.Click += new System.EventHandler(this.ToolStripUndoButtonClick);
            // 
            // toolStripRedoButton
            // 
            this.toolStripRedoButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.toolStripRedoButton.Enabled = false;
            this.toolStripRedoButton.Image = global::GameEditor.IconsResource.Edit_Redo;
            this.toolStripRedoButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripRedoButton.Name = "toolStripRedoButton";
            this.toolStripRedoButton.Size = new System.Drawing.Size(23, 22);
            this.toolStripRedoButton.Text = "Redo";
            this.toolStripRedoButton.ToolTipText = "Redo";
            this.toolStripRedoButton.Click += new System.EventHandler(this.ToolStripRedoButtonClick);
            // 
            // TriggerEditorDialog
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(926, 528);
            this.Controls.Add(this.toolStrip);
            this.Controls.Add(this.triggerWindow1);
            this.Controls.Add(this.triggersBox1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Name = "TriggerEditorDialog";
            this.Text = "Trigger Editor";
            this.toolStrip.ResumeLayout(false);
            this.toolStrip.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private TriggersMenu triggersBox1;
        private System.Windows.Forms.ToolStrip toolStrip;
        private System.Windows.Forms.ToolStripButton toolStripUndoButton;
        private System.Windows.Forms.ToolStripButton toolStripRedoButton;
        public TriggerWindow triggerWindow1;
    }
}