using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace GameEditor.TriggerEditor.Selectors
{
    class ActionSelector : ListSelector
    {

        protected override void ItemSelectorLoad(object sender, System.EventArgs e)
        {
            comboBox1.DisplayMember = "Value";
            comboBox1.ValueMember = "Key";
            comboBox1.DataSource = new BindingSource(TriggerController.ActionStrings, null);
            comboBox1.SelectedIndex = 0;
            itemLabel.Text = "Action:";
        }

        protected override void ComboBoxSelectedIndexChanged(object sender, EventArgs e)
        {
            Result = ((KeyValuePair<int, string>)comboBox1.SelectedItem).Key;
        }

        protected override void InitializeComponent()
        {
            base.InitializeComponent();
            Text = "Action Selector";
        }

    }
}
