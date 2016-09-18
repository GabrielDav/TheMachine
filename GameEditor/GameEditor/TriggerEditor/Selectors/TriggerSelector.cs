using System;
using System.Windows.Forms;
using Engine.Core;
using TheGoo;

namespace GameEditor.TriggerEditor.Selectors
{
    public class TriggerSelector : ListSelector
    {

        public string SelectedTrigger;
        public string DefaultValue;

        protected override void ItemSelectorLoad(object sender, EventArgs e)
        {
            comboBox1.DisplayMember = "Name";
            comboBox1.ValueMember = "Name";
            comboBox1.DataSource = new BindingSource(GameGlobals.Map.Triggers, null);
            comboBox1.SelectedIndex = 0;
            itemLabel.Text = "Trigger:";
            if (!string.IsNullOrEmpty(DefaultValue))
            {
                comboBox1.SelectedValue = DefaultValue;
            }
        }

        protected override void ComboBoxSelectedIndexChanged(object sender, EventArgs e)
        {
            SelectedTrigger = comboBox1.SelectedValue.ToString();
        }

        public void SetValue(string value)
        {
            SelectedTrigger = value;
        }

        protected override void InitializeComponent()
        {
            base.InitializeComponent();
            this.SuspendLayout();
            // 
            // EventSelector
            // 
            AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            ClientSize = new System.Drawing.Size(472, 70);
            Name = "TriggerSelector";
            Text = "Trigger Selector";
            ResumeLayout(false);
            PerformLayout();
        }
    }
}
