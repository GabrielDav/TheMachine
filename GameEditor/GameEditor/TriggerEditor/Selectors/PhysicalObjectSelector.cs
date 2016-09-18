using System;
using System.Windows.Forms;
using Engine.Mechanics;
using TheGoo;

namespace GameEditor.TriggerEditor.Selectors
{
    class PhysicalObjectSelector : ListSelector
    {
        public string PhysicalObjectName;

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
                    comboBox1.DataSource = GameGlobals.Map.GameObjects;
                    comboBox1.SelectedIndex = 0;
                }
                itemLabel.Text = "Object:";
            }
            catch (Exception exception)
            {

                MessageBox.Show("Error: " + exception.Message);
            }

        }

        protected override void ComboBoxSelectedIndexChanged(object sender, EventArgs e)
        {
            if (comboBox1.SelectedItem is PhysicalObject)
                PhysicalObjectName = ((PhysicalObject)comboBox1.SelectedItem).Name;
        }

        private void InitializeComponent()
        {
            this.SuspendLayout();
            // 
            // PhysicalObjectSelector
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.ClientSize = new System.Drawing.Size(472, 70);
            this.Name = "PhysicalObjectSelector";
            this.Text = "Object Selector";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

    }
}
