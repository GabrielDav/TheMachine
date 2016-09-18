namespace GameEditor.Sprite_Editor
{
    partial class SpriteEditor
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
            this.toolStrip1 = new System.Windows.Forms.ToolStrip();
            this.toolStripNewButton = new System.Windows.Forms.ToolStripButton();
            this.toolStripOpenButton = new System.Windows.Forms.ToolStripButton();
            this.toolStripSaveButton = new System.Windows.Forms.ToolStripButton();
            this.toolStripExportButton = new System.Windows.Forms.ToolStripButton();
            this.toolStripDeleteSpriteButton = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.toolStripImportAnimationButton = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this.toolStripBackgroundColorPickerButton = new System.Windows.Forms.ToolStripButton();
            this.colorDialog1 = new System.Windows.Forms.ColorDialog();
            this.openFileDialog1 = new System.Windows.Forms.OpenFileDialog();
            this.saveFileDialog1 = new System.Windows.Forms.SaveFileDialog();
            this.tabControl1 = new GameEditor.Sprite_Editor.TabControlMod();
            this.toolStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // toolStrip1
            // 
            this.toolStrip1.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden;
            this.toolStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripNewButton,
            this.toolStripOpenButton,
            this.toolStripSaveButton,
            this.toolStripExportButton,
            this.toolStripDeleteSpriteButton,
            this.toolStripSeparator1,
            this.toolStripImportAnimationButton,
            this.toolStripSeparator2,
            this.toolStripBackgroundColorPickerButton});
            this.toolStrip1.Location = new System.Drawing.Point(0, 0);
            this.toolStrip1.Name = "toolStrip1";
            this.toolStrip1.RenderMode = System.Windows.Forms.ToolStripRenderMode.System;
            this.toolStrip1.Size = new System.Drawing.Size(654, 25);
            this.toolStrip1.TabIndex = 1;
            this.toolStrip1.Text = "toolStrip1";
            this.toolStrip1.ItemClicked += new System.Windows.Forms.ToolStripItemClickedEventHandler(this.ToolStrip1ItemClicked);
            // 
            // toolStripNewButton
            // 
            this.toolStripNewButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.toolStripNewButton.Image = global::GameEditor.IconsResource.New;
            this.toolStripNewButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripNewButton.Name = "toolStripNewButton";
            this.toolStripNewButton.Size = new System.Drawing.Size(23, 22);
            this.toolStripNewButton.Text = "New Sprite";
            this.toolStripNewButton.Click += new System.EventHandler(this.ToolStripNewButtonClick);
            // 
            // toolStripOpenButton
            // 
            this.toolStripOpenButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.toolStripOpenButton.Image = global::GameEditor.IconsResource.OpenFolder;
            this.toolStripOpenButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripOpenButton.Name = "toolStripOpenButton";
            this.toolStripOpenButton.Size = new System.Drawing.Size(23, 22);
            this.toolStripOpenButton.Text = "Open Sprite";
            this.toolStripOpenButton.Click += new System.EventHandler(this.ToolStripOpenButtonClick);
            // 
            // toolStripSaveButton
            // 
            this.toolStripSaveButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.toolStripSaveButton.Enabled = false;
            this.toolStripSaveButton.Image = global::GameEditor.IconsResource.Save;
            this.toolStripSaveButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripSaveButton.Name = "toolStripSaveButton";
            this.toolStripSaveButton.Size = new System.Drawing.Size(23, 22);
            this.toolStripSaveButton.Text = "Save";
            this.toolStripSaveButton.Click += new System.EventHandler(this.ToolStripSaveButtonClick);
            // 
            // toolStripExportButton
            // 
            this.toolStripExportButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.toolStripExportButton.Enabled = false;
            this.toolStripExportButton.Image = global::GameEditor.IconsResource.exportRed;
            this.toolStripExportButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripExportButton.Name = "toolStripExportButton";
            this.toolStripExportButton.Size = new System.Drawing.Size(23, 22);
            this.toolStripExportButton.Text = "Export Sprite";
            this.toolStripExportButton.Click += new System.EventHandler(this.ToolStripExportButtonClick);
            // 
            // toolStripDeleteSpriteButton
            // 
            this.toolStripDeleteSpriteButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.toolStripDeleteSpriteButton.Enabled = false;
            this.toolStripDeleteSpriteButton.Image = global::GameEditor.IconsResource.DeleteProject;
            this.toolStripDeleteSpriteButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripDeleteSpriteButton.Name = "toolStripDeleteSpriteButton";
            this.toolStripDeleteSpriteButton.Size = new System.Drawing.Size(23, 22);
            this.toolStripDeleteSpriteButton.Text = "Delete Sprite";
            this.toolStripDeleteSpriteButton.Click += new System.EventHandler(this.ToolStripDeleteSpriteButtonClick);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(6, 25);
            // 
            // toolStripImportAnimationButton
            // 
            this.toolStripImportAnimationButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.toolStripImportAnimationButton.Enabled = false;
            this.toolStripImportAnimationButton.Image = global::GameEditor.IconsResource.import;
            this.toolStripImportAnimationButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripImportAnimationButton.Name = "toolStripImportAnimationButton";
            this.toolStripImportAnimationButton.Size = new System.Drawing.Size(23, 22);
            this.toolStripImportAnimationButton.Text = "Import Animation";
            this.toolStripImportAnimationButton.Click += new System.EventHandler(this.ToolStripImportButtonClick);
            // 
            // toolStripSeparator2
            // 
            this.toolStripSeparator2.Name = "toolStripSeparator2";
            this.toolStripSeparator2.Size = new System.Drawing.Size(6, 25);
            // 
            // toolStripBackgroundColorPickerButton
            // 
            this.toolStripBackgroundColorPickerButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.toolStripBackgroundColorPickerButton.Image = global::GameEditor.IconsResource.ColorDialog;
            this.toolStripBackgroundColorPickerButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripBackgroundColorPickerButton.Name = "toolStripBackgroundColorPickerButton";
            this.toolStripBackgroundColorPickerButton.Size = new System.Drawing.Size(23, 22);
            this.toolStripBackgroundColorPickerButton.Text = "Background Color";
            this.toolStripBackgroundColorPickerButton.Click += new System.EventHandler(this.ToolStripBackgroundColorPickerButtonClick);
            // 
            // colorDialog1
            // 
            this.colorDialog1.AnyColor = true;
            this.colorDialog1.Color = System.Drawing.Color.White;
            // 
            // openFileDialog1
            // 
            this.openFileDialog1.DefaultExt = "xml";
            this.openFileDialog1.Filter = "Sprite data files|*.xml|Sprite package|*.spr";
            // 
            // saveFileDialog1
            // 
            this.saveFileDialog1.Filter = "Sprite package|*.spr";
            // 
            // tabControl1
            // 
            this.tabControl1.HideTabPageButtons = true;
            this.tabControl1.Location = new System.Drawing.Point(0, 28);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(654, 444);
            this.tabControl1.TabIndex = 0;
            // 
            // SpriteEditor
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(654, 472);
            this.Controls.Add(this.toolStrip1);
            this.Controls.Add(this.tabControl1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.Name = "SpriteEditor";
            this.Text = "Sprite Editor";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.SpriteEditorFormClosing);
            this.Load += new System.EventHandler(this.SpriteEditorLoad);
            this.Shown += new System.EventHandler(this.SpriteEditorShown);
            this.toolStrip1.ResumeLayout(false);
            this.toolStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        public TabControlMod tabControl1;
        private System.Windows.Forms.ToolStripButton toolStripBackgroundColorPickerButton;
        private System.Windows.Forms.ColorDialog colorDialog1;
        private System.Windows.Forms.ToolStripButton toolStripNewButton;
        private System.Windows.Forms.ToolStripButton toolStripOpenButton;
        private System.Windows.Forms.ToolStripButton toolStripImportAnimationButton;
        private System.Windows.Forms.ToolStripButton toolStripSaveButton;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.OpenFileDialog openFileDialog1;
        public System.Windows.Forms.ToolStrip toolStrip1;
        private System.Windows.Forms.ToolStripButton toolStripDeleteSpriteButton;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator2;
        private System.Windows.Forms.ToolStripButton toolStripExportButton;
        private System.Windows.Forms.SaveFileDialog saveFileDialog1;

    }
}