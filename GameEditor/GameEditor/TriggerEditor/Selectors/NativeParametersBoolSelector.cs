using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using GameLibrary.Triggers;
using TheGoo;

namespace GameEditor.TriggerEditor.Selectors
{
    public class NativeFunctionsSelector : ListSelector
    {
        public int DefaultValue;
        public int SelectedValue;

        protected override void ItemSelectorLoad(object sender, EventArgs e)
        {
            var binding = new BindingSource {
                DataSource = Enum.GetValues(typeof (NativeFunctions)).Cast<NativeFunctions>().ToDictionary(key => (int) key,
                                                                                                                              value => value.ToString() )
            };
            comboBox1.DisplayMember = "Value";
            comboBox1.ValueMember = "Key";
            comboBox1.DataSource = binding;
            //comboBox1.DisplayMember = "Key";
           // comboBox1.ValueMember = "Value";
           // comboBox1.DataSource = new BindingSource(parameters, null);
            
            //comboBox1.DataSource = new BindingSource(GameGlobals.Map.Triggers, null);
            comboBox1.SelectedIndex = 0;
            itemLabel.Text = "Native parameter:";
            if (DefaultValue != 0)
                comboBox1.SelectedValue = DefaultValue;
        }

        protected override void ComboBoxSelectedIndexChanged(object sender, EventArgs e)
        {
            SelectedValue = (int)comboBox1.SelectedValue;
        }

        public void SetValue(int value)
        {
            SelectedValue = value;
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            comboBox1.Location = new Point(comboBox1.Location.X + 50, comboBox1.Location.Y);
            comboBox1.Size = new Size(comboBox1.Size.Width - 50, comboBox1.Size.Height);
            this.SuspendLayout();

            AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            ClientSize = new System.Drawing.Size(472, 70);
            Name = "NativeParameterBoolSelector";
            Text = "Native Parameter Selector - Bool";
            ResumeLayout(false);
            PerformLayout();
        }

    }
}
