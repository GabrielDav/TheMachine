namespace GameEditor.Sprite_Editor
{
    partial class SpriteControl
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
            this.propertyGrid = new System.Windows.Forms.PropertyGrid();
            this.comboBox1 = new System.Windows.Forms.ComboBox();
            this.Play = new System.Windows.Forms.Button();
            this.Stop = new System.Windows.Forms.Button();
            this.TrackBar = new System.Windows.Forms.TrackBar();
            this.IsLoopedBox = new System.Windows.Forms.CheckBox();
            this.Delete = new System.Windows.Forms.Button();
            this.spriteWindow = new GameEditor.Sprite_Editor.XnaWindowControl();
            ((System.ComponentModel.ISupportInitialize)(this.TrackBar)).BeginInit();
            this.SuspendLayout();
            // 
            // propertyGrid
            // 
            this.propertyGrid.Font = new System.Drawing.Font("Consolas", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.propertyGrid.HelpVisible = false;
            this.propertyGrid.LineColor = System.Drawing.SystemColors.ControlLightLight;
            this.propertyGrid.Location = new System.Drawing.Point(406, 21);
            this.propertyGrid.Name = "propertyGrid";
            this.propertyGrid.Size = new System.Drawing.Size(239, 379);
            this.propertyGrid.TabIndex = 1;
            // 
            // comboBox1
            // 
            this.comboBox1.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBox1.FormattingEnabled = true;
            this.comboBox1.Location = new System.Drawing.Point(406, 0);
            this.comboBox1.Name = "comboBox1";
            this.comboBox1.Size = new System.Drawing.Size(218, 21);
            this.comboBox1.TabIndex = 3;
            this.comboBox1.SelectedIndexChanged += new System.EventHandler(this.ComboBox1SelectedIndexChanged);
            // 
            // Play
            // 
            this.Play.Enabled = false;
            this.Play.Image = global::GameEditor.IconsResource.Run;
            this.Play.Location = new System.Drawing.Point(0, 406);
            this.Play.Name = "Play";
            this.Play.Size = new System.Drawing.Size(23, 23);
            this.Play.TabIndex = 4;
            this.Play.UseVisualStyleBackColor = true;
            this.Play.Click += new System.EventHandler(this.PlayClick);
            // 
            // Stop
            // 
            this.Stop.Enabled = false;
            this.Stop.Image = global::GameEditor.IconsResource.Stop;
            this.Stop.Location = new System.Drawing.Point(29, 406);
            this.Stop.Name = "Stop";
            this.Stop.Size = new System.Drawing.Size(23, 23);
            this.Stop.TabIndex = 5;
            this.Stop.UseVisualStyleBackColor = true;
            this.Stop.Click += new System.EventHandler(this.StopClick);
            // 
            // TrackBar
            // 
            this.TrackBar.Enabled = false;
            this.TrackBar.Location = new System.Drawing.Point(58, 406);
            this.TrackBar.Name = "TrackBar";
            this.TrackBar.Size = new System.Drawing.Size(342, 45);
            this.TrackBar.TabIndex = 6;
            this.TrackBar.Scroll += new System.EventHandler(this.TrackBarScroll);
            // 
            // IsLoopedBox
            // 
            this.IsLoopedBox.AutoSize = true;
            this.IsLoopedBox.Checked = true;
            this.IsLoopedBox.CheckState = System.Windows.Forms.CheckState.Checked;
            this.IsLoopedBox.Enabled = false;
            this.IsLoopedBox.Location = new System.Drawing.Point(406, 410);
            this.IsLoopedBox.Name = "IsLoopedBox";
            this.IsLoopedBox.Size = new System.Drawing.Size(62, 17);
            this.IsLoopedBox.TabIndex = 7;
            this.IsLoopedBox.Text = "Looped";
            this.IsLoopedBox.UseVisualStyleBackColor = true;
            // 
            // Delete
            // 
            this.Delete.Enabled = false;
            this.Delete.Image = global::GameEditor.IconsResource.Delete;
            this.Delete.Location = new System.Drawing.Point(624, 0);
            this.Delete.Name = "Delete";
            this.Delete.Size = new System.Drawing.Size(22, 22);
            this.Delete.TabIndex = 8;
            this.Delete.UseVisualStyleBackColor = true;
            this.Delete.Click += new System.EventHandler(this.DeleteClick);
            // 
            // spriteWindow
            // 
            this.spriteWindow.BackColor = System.Drawing.Color.CornflowerBlue;
            this.spriteWindow.Location = new System.Drawing.Point(0, 0);
            this.spriteWindow.Name = "spriteWindow";
            this.spriteWindow.Size = new System.Drawing.Size(400, 400);
            this.spriteWindow.TabIndex = 2;
            // 
            // SpriteControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.Delete);
            this.Controls.Add(this.IsLoopedBox);
            this.Controls.Add(this.TrackBar);
            this.Controls.Add(this.Stop);
            this.Controls.Add(this.Play);
            this.Controls.Add(this.comboBox1);
            this.Controls.Add(this.spriteWindow);
            this.Controls.Add(this.propertyGrid);
            this.Name = "SpriteControl";
            this.Size = new System.Drawing.Size(649, 439);
            ((System.ComponentModel.ISupportInitialize)(this.TrackBar)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        public System.Windows.Forms.PropertyGrid propertyGrid;
        public XnaWindowControl spriteWindow;
        private System.Windows.Forms.ComboBox comboBox1;
        private System.Windows.Forms.Button Play;
        private System.Windows.Forms.Button Stop;
        private System.Windows.Forms.TrackBar TrackBar;
        private System.Windows.Forms.CheckBox IsLoopedBox;
        private System.Windows.Forms.Button Delete;

    }
}
