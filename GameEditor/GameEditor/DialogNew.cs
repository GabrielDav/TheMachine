using System;
using System.Windows.Forms;

namespace GameEditor
{
    public partial class DialogNew : Form
    {
        public DialogNew()
        {
            InitializeComponent();
            comboBox1.SelectedIndex = 0;
        }

        private void Button1Click(object sender, EventArgs e)
        {
            int numW;
            int numH;
            if (textBox1.Text == string.Empty || textBox2.Text == string.Empty)
            {
                MessageBox.Show("Width and height required");
                return;
            }
            if (!int.TryParse(textBox1.Text, out numW))
            {
                MessageBox.Show("'" + textBox1.Text + "'  is NOT an integer.");
                return;
            }
            if (!int.TryParse(textBox2.Text, out numH))
            {
                MessageBox.Show("'" + textBox2.Text + "' is NOT an integer.");
                return;
            }
            if (numW < 81 || numW > 8000)
            {
                MessageBox.Show("Map width incorrect must be greater then 80 and less then 8000");
                return;
            }
            if (numH < 50 || numH > 5000)
            {
                MessageBox.Show("Map height is incorrect must be greater then 50 and less then 5000");
                return;
            }
            DialogResult = DialogResult.OK;
            Close();
        }
    }
}
