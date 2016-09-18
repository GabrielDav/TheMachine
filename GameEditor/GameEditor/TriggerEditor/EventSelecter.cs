using System;
using System.Collections.Generic;
using System.Windows.Forms;
using Engine.Mechanics;

namespace GameEditor.TriggerEditor
{

    public partial class EventSelecter : Form
    {
        

        public int Result;

        public EventSelecter()
        {
            InitializeComponent();
            
            foreach (var eventsString in TriggerController.EventsStrings)
            {
                comboBox1.Items.Add(eventsString);
            }
            Result = 0;
        }

        private void ComboBox1SelectedIndexChanged(object sender, EventArgs e)
        {
            Result = ((KeyValuePair<int, string>) comboBox1.SelectedItem).Key;
        }
    }
}
