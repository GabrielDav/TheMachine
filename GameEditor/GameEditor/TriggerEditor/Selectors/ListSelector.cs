using System;
using System.Windows.Forms;

namespace GameEditor.TriggerEditor.Selectors
{

    public partial class ListSelector : Form
    {
        public int Result;

        public ListSelector()
        {
            InitializeComponent();
        }

        protected virtual void ComboBoxSelectedIndexChanged(object sender, EventArgs e){}

        protected virtual void ItemSelectorLoad(object sender, EventArgs e){}

        public void SetResult(int id)
        {
            Result = id;
            comboBox1.SelectedItem = id;
        }
    }
}
