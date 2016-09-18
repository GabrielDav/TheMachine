using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using System.Linq;
using Engine.Mechanics;
using Engine.Mechanics.Triggers;


namespace GameEditor.TriggerEditor
{
    public delegate void TriggerPropertyChangingEventHandler(object sender, TriggerPropertyEventArgs e);

    public delegate void TriggerPropertyChangedEventHandler(object sender, TriggerPropertyEventArgs e);

    public delegate void TriggerAddedEventHandler(object sender, TriggerEventArgs e);

    public delegate void TriggerRemovedEventHandler(object sender, TriggerEventArgs e);

    public delegate void TriggerSelectedEventHandler(object sender, string triggerName);

    public partial class TriggersMenu : UserControl
    {

        private class TriggerItem
        {
            public string Name;
            public bool Enabled;

            public TriggerItem(string name)
            {
                Name = name;
                Enabled = true;
            }

            public override string ToString()
            {
                return Name;
            }
        }

        public event TriggerPropertyChangingEventHandler TriggerPropertyChanging;
        public event TriggerPropertyChangedEventHandler TriggerPropertyChanged;
        public event TriggerAddedEventHandler TriggerAdded;
        public event TriggerRemovedEventHandler TriggerRemoved;
        public event TriggerSelectedEventHandler TriggerSelected;

        private TriggerItem _selectedTrigger;

        public TriggersMenu()
        {
            InitializeComponent();
        }

        #region TriggerList

        private void TriggersListDrawItem(object sender, DrawItemEventArgs e)
        {
            e.DrawBackground();
            Graphics g = e.Graphics;

            g.FillRectangle(new SolidBrush(e.BackColor), e.Bounds);
            if (e.Index > -1)
            {
                var triggerItem = (TriggerItem) triggersList.Items[e.Index];
                var color = triggerItem.Enabled ? Color.Black : Color.DarkGray;

                g.DrawString(triggerItem.ToString(), e.Font, new SolidBrush(color), new PointF(e.Bounds.X, e.Bounds.Y));
            }
            e.DrawFocusRectangle();
        }

        private bool IsTriggerExist(string name)
        {
            return triggersList.Items.Cast<TriggerItem>().Any(item => item.Name.ToLower() == name.ToLower());
        }

        public bool AddTrigger(string name)
        {
            if (IsTriggerExist(name))
            {
                MessageBox.Show("Trigger named '" + name + "' already exist.");
                return false;
            }
            var eventArgs = new TriggerEventArgs(name);
            if (TriggerAdded != null)
                TriggerAdded(this, eventArgs);
            if (!eventArgs.Continue)
            {
                MessageBox.Show("Unable to add trigger");
                return false;
            }
            var triggerItem = new TriggerItem(name);
            triggersList.Items.Add(triggerItem);
            triggersList.SelectedItem = triggerItem;
            return true;
        }

        public void RemoveTrigger(string name)
        {
            if (!IsTriggerExist(name))
                return;
            var eventArgs = new TriggerEventArgs(name);
            if (TriggerRemoved != null)
                TriggerRemoved(this, eventArgs);
            if (!eventArgs.Continue)
                return;
            triggersList.Items.Remove(triggersList.Items.Cast<TriggerItem>().First(item => item.Name == name));
        }

        public void AddTriggerForced(Trigger trigger)
        {
            var item = new TriggerItem(trigger.Name) {Enabled = trigger.Enabled};
            triggersList.Items.Add(item);
        }

        public void TriggerRemoveForced(string name)
        {
            triggersList.Items.Remove(triggersList.Items.Cast<TriggerItem>().First(item => item.Name == name));
        }

        public void TriggerSelectItemForced(string name)
        {
            triggersList.SelectedItem = triggersList.Items.Cast<TriggerItem>().First(item => item.Name == name);
        }

        public void RefreshTriggerValues(string name, Trigger trigger)
        {
            var triggerItem = triggersList.Items.Cast<TriggerItem>().First(item => item.Name == name);
            triggerItem.Enabled = trigger.Enabled;
            triggerItem.Name = trigger.Name;
            triggersList.Refresh();
        }

        public string GetSafeName(string name = "")
        {
            var counter = 1;
            if (!string.IsNullOrEmpty(name) && triggersList.Items.Cast<TriggerItem>().All(item => item.Name.ToLower() != name.ToLower()))
                return name;
            while (true)
            {
                name = "Trigger" + counter.ToString("000");
                if (triggersList.Items.Cast<TriggerItem>().Any(item => item.Name.ToLower() == name.ToLower()))
                {
                    counter++;
                    continue;
                }
                break;
            }
            return name;
        }

        public bool Rename(string oldName, string newName)
        {
            if (IsTriggerExist(newName))
            {
                MessageBox.Show("Trigger named '" + newName + "' already exist.");
                return false;
            }
            if (TriggerPropertyChanging != null)
                TriggerPropertyChanging(this, new TriggerPropertyEventArgs(oldName, "Name", oldName));
            triggersList.Items.Cast<TriggerItem>().First(item => item.Name == oldName).Name = newName;
            if (TriggerPropertyChanged != null)
                TriggerPropertyChanged(this, new TriggerPropertyEventArgs(oldName, "Name", newName));
            triggersList.Refresh();
            return true;
        }

        public void SetEnabled(string name, bool enabled)
        {
            var triggerItem = triggersList.Items.Cast<TriggerItem>().First(item => item.Name == name);
            if (TriggerPropertyChanging != null)
                TriggerPropertyChanging(this, new TriggerPropertyEventArgs(name, "Enabled", triggerItem.Enabled));
            triggerItem.Enabled = enabled;
            if (TriggerPropertyChanged != null)
                TriggerPropertyChanged(this, new TriggerPropertyEventArgs(name, "Enabled", triggerItem.Enabled));
        }

        private void TriggersListSelectedIndexChanged(object sender, EventArgs e)
        {
            _selectedTrigger = triggersList.SelectedItem as TriggerItem;
            removeTriggerButton.Enabled = _selectedTrigger != null;
            if (TriggerSelected != null)
                TriggerSelected(this, _selectedTrigger != null ? _selectedTrigger.Name : null);
        }

        private void TriggersListMouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                var index = triggersList.IndexFromPoint(e.Location);
                triggersList.SelectedIndex = index;
                triggersListMenuStrip.Show(triggersList, e.Location);
            }
        }

        #endregion

        public string UserTriggerGetName(string name)
        {
            var dlg = new TriggerNameDialog {triggerNameTextBox = {Text = name}};
            if (dlg.ShowDialog() != DialogResult.OK)
                return null;
            return dlg.triggerNameTextBox.Text;
        }

        private void RemoveTriggerButtonClick(object sender, EventArgs e)
        {
            RemoveTrigger(_selectedTrigger.Name);
        }

        private void AddTriggerButtonClick(object sender, EventArgs e)
        {
            var name = GetSafeName();
            showDialog:
            var newTriggerName = UserTriggerGetName(name);
            if (string.IsNullOrEmpty(newTriggerName))
                return;
            if (!AddTrigger(newTriggerName))
                goto showDialog;
        }

        private void TriggersListMenuStripOpening(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (_selectedTrigger == null)
            {
                toolStripEnableButton.Enabled = false;
                toolStripRenameButton.Enabled = false;
            }
            else
            {
                toolStripEnableButton.Enabled = true;
                toolStripEnableButton.Checked = _selectedTrigger.Enabled;
                toolStripRenameButton.Enabled = true;
            }
        }

        private void ToolStripRenameButtonClick(object sender, EventArgs e)
        {
            showDialog:
            var newTriggerName = UserTriggerGetName(_selectedTrigger.Name);
            if (string.IsNullOrEmpty(newTriggerName))
                return;
            if (newTriggerName.ToLower() == _selectedTrigger.Name.ToLower())
                return;
            if (!Rename(_selectedTrigger.Name, newTriggerName))
                goto showDialog;
        }


        private void ToolStripEnableButtonClick(object sender, EventArgs e)
        {
            SetEnabled(_selectedTrigger.Name, !_selectedTrigger.Enabled);
            triggersList.Refresh();
        }

        public void Fill(List<Trigger> triggers)
        {
            foreach (var trigger in triggers)
            {
                AddTriggerForced(trigger);
            }
        }

        

    }

    public class TriggerPropertyEventArgs : EventArgs
    {
        public string TriggerName;
        public string PropertyName;
        public object PropertyValue;

        public TriggerPropertyEventArgs(string triggerName, string propertyName, object propertyValue)
        {
            TriggerName = triggerName;
            PropertyName = propertyName;
            PropertyValue = propertyValue;
        }
    }

    public class TriggerEventArgs : EventArgs
    {
        public string TriggerName;
        public bool Continue = true;

        public TriggerEventArgs(string triggerName)
        {
            TriggerName = triggerName;
        }
    }
}
