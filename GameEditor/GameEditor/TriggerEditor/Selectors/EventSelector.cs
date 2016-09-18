using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace GameEditor.TriggerEditor.Selectors
{
    internal class EventSelector : ListSelector
    {
        protected override void ItemSelectorLoad(object sender, EventArgs e)
        {
            comboBox1.DisplayMember = "Value";
            comboBox1.ValueMember = "Key";
            comboBox1.DataSource = new BindingSource(TriggerController.EventsStrings, null);
            comboBox1.SelectedIndex = 0;
            itemLabel.Text = "Event:";
        }

        protected override void ComboBoxSelectedIndexChanged(object sender, EventArgs e)
        {
            Result = ((KeyValuePair<int, string>) comboBox1.SelectedItem).Key;
        }

        protected override void InitializeComponent()
        {
            base.InitializeComponent();
            this.SuspendLayout();
            // 
            // EventSelector
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.ClientSize = new System.Drawing.Size(472, 70);
            this.Name = "EventSelector";
            this.Text = "Event Selector";
            this.ResumeLayout(false);
            this.PerformLayout();

        }
    }
}
