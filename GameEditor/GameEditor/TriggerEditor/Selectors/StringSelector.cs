using System.Windows.Forms;

namespace GameEditor.TriggerEditor.Selectors
{
    public partial class StringSelector : Form
    {
        public string StringValue;

        public StringSelector()
        {
            InitializeComponent();
        }

        public void SetValue(string value)
        {
            textBox1.Text = value;
        }

        private void textBox1_TextChanged(object sender, System.EventArgs e)
        {
            StringValue = textBox1.Text;
        }

    }
}
