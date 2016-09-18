using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace GameEditor.TriggerEditor.Selectors
{
    class ConditionSelector : ListSelector
    {

        protected override void ItemSelectorLoad(object sender, System.EventArgs e)
        {
            comboBox1.DisplayMember = "Value";
            comboBox1.ValueMember = "Key";
            comboBox1.DataSource = new BindingSource(TriggerController.ConditionStrings, null);
            comboBox1.SelectedIndex = 0;
            itemLabel.Text = "Condition:";
        }

        protected override void ComboBoxSelectedIndexChanged(object sender, EventArgs e)
        {
            Result = ((KeyValuePair<int, string>)comboBox1.SelectedItem).Key;
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            Size = new Size(Size.Width + 20, Size.Height);
            comboBox1.Location = new Point(comboBox1.Location.X + 20, comboBox1.Location.Y);
            button1.Location = new Point(button1.Location.X + 20, button1.Location.Y);
            button2.Location = new Point(button2.Location.X + 20, button2.Location.Y);
        }

        protected override void InitializeComponent()
        {
            base.InitializeComponent();
            Text = "Condition Selector";
        }

    }
}
