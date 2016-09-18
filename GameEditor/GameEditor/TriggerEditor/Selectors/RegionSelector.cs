using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using Engine.Mechanics;
using TheGoo;
using Region = Engine.Mechanics.Triggers.Region;

namespace GameEditor.TriggerEditor.Selectors
{
    class RegionSelector : ListSelector
    {
        public string RegionName;

        protected override void ItemSelectorLoad(object sender, System.EventArgs e)
        {
            try
            {


                if (GameGlobals.Map.Regions.Count < 1)
                {
                    comboBox1.Items.Add("[No Regions]");
                    comboBox1.SelectedIndex = 0;
                    comboBox1.Enabled = false;
                    button1.Enabled = false;
                }
                else
                {
                    comboBox1.DisplayMember = "Name";
                    comboBox1.ValueMember = "Name";
                    comboBox1.DataSource = GameGlobals.Map.Regions;
                    comboBox1.SelectedIndex = 0;
                }
                itemLabel.Text = "Region:";
            }
            catch (Exception exception)
            {

                MessageBox.Show("Error: " + exception.Message);
            }

        }

        protected override void ComboBoxSelectedIndexChanged(object sender, EventArgs e)
        {
            if (comboBox1.SelectedItem is Region)
                RegionName = ((Region)comboBox1.SelectedItem).Name;
        }

        private void InitializeComponent()
        {
            this.SuspendLayout();
            // 
            // RegionSelector
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.ClientSize = new System.Drawing.Size(472, 70);
            this.Name = "RegionSelector";
            this.Text = "Region Selector";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

    }
}
