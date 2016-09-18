using System;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using Engine.Mechanics;
using GameLibrary.Objects;
using TheGoo;

namespace GameEditor.TriggerEditor.Selectors
{
    class CameraPathSelector : ListSelector
    {
        public string CameraPathName;

        protected override void ItemSelectorLoad(object sender, EventArgs e)
        {
            try
            {
                if (GameGlobals.Map.GameObjects.Count < 1)
                {
                    comboBox1.Items.Add("[No Objects]");
                    comboBox1.SelectedIndex = 0;
                    comboBox1.Enabled = false;
                    button1.Enabled = false;
                }
                else
                {
                    comboBox1.DisplayMember = "Name";
                    comboBox1.ValueMember = "Name";
                    comboBox1.DataSource = GameGlobals.Map.GameObjects.Where(item => item is CameraPath).ToList();
                    comboBox1.SelectedIndex = 0;
                }
                itemLabel.Text = "Camera Path:";
            }
            catch (Exception exception)
            {

                MessageBox.Show("Error: " + exception.Message);
            }

        }

        protected override void ComboBoxSelectedIndexChanged(object sender, EventArgs e)
        {
            if (comboBox1.SelectedItem is PhysicalObject)
                CameraPathName = ((PhysicalObject)comboBox1.SelectedItem).Name;
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            this.Name = "CameraPathSelector";
            this.Text = "Camera Path Selector";
            comboBox1.Size = new Size(comboBox1.Size.Width - 30, comboBox1.Size.Height);
            comboBox1.Location = new Point(comboBox1.Location.X + 30, comboBox1.Location.Y);
        }

        private void InitializeComponent()
        {
            this.SuspendLayout();
            // 
            // PhysicalObjectSelector
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.ClientSize = new System.Drawing.Size(472, 70);
            
            this.ResumeLayout(false);
            this.PerformLayout();

        }

    }
}
