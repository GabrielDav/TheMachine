using System.Windows.Forms;

namespace GameEditor.TriggerEditor.Selectors
{
    public partial class BoolSelector : Form
    {
        public bool BoolValue;

        public BoolSelector()
        {
            InitializeComponent();
        }

        public void SetValue(bool value)
        {
            if (value)
                radioButtonTrue.Checked = true;
            else
                radioButtonFalse.Checked = true;
        }

        private void RadioButtonFalseCheckedChanged(object sender, System.EventArgs e)
        {
            BoolValue = radioButtonTrue.Checked;
        }
    }
}
