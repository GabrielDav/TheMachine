using System.Windows.Forms;

namespace GameEditor.TriggerEditor.Selectors
{
    public partial class IntegerSelector : Form
    {
        public int IntegerValue;

        public IntegerSelector()
        {
            InitializeComponent();
        }

        public void SetValue(int value)
        {
            numericUpDown1.Value = (int)value;
        }

        private void NumericUpDown1ValueChanged(object sender, System.EventArgs e)
        {
            IntegerValue = (int)numericUpDown1.Value;
        }
    }
}
