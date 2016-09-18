using System.Windows.Forms;
using Microsoft.Xna.Framework;

namespace GameEditor.TriggerEditor.Selectors
{
    public partial class PointSelector : Form
    {
        public Point PointValue;

        public PointSelector()
        {
            InitializeComponent();
        }

        public void SetValue(Point value)
        {
            numericUpDown1.Value = value.X;
            numericUpDown2.Value = value.Y;
        }

        private void NumericUpDown1ValueChanged(object sender, System.EventArgs e)
        {
            PointValue.X = (int) numericUpDown1.Value;
        }

        private void NumericUpDown2ValueChanged(object sender, System.EventArgs e)
        {
            PointValue.Y = (int) numericUpDown2.Value;
        }
    }
}
