using System.Windows.Forms;

namespace GameEditor.TriggerEditor.Selectors
{
    public partial class FloatSelector : Form
    {
        public float DecimalValue;

        public FloatSelector()
        {
            InitializeComponent();
        }

        public void SetValue(float value)
        {
            numericUpDown1.Value = (decimal)value;
        }

        private void NumericUpDown1ValueChanged(object sender, System.EventArgs e)
        {
            DecimalValue = (float)numericUpDown1.Value;
        }
    }
}
