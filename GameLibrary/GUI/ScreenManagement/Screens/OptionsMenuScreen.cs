using Microsoft.Xna.Framework;

namespace GameLibrary.Gui.ScreenManagement.Screens
{
    internal class OptionsMenuScreen : MenuScreen
    {
        private readonly MenuEntry _entry;
        private readonly string[] _entryValues = {"1", "2", "3"};
        private int _currentEntry;

        public OptionsMenuScreen()
        {
            _entry = new MenuEntry("Values: " + _entryValues[_currentEntry], null, new Rectangle(200, 50, 100, 50), null);
            _entry.Selected += EntrySelected;
            MenuEntries.Add(_entry);
        }

        void SetMenuEntryText()
        {
            _entry.Text = "Values: " + _entryValues[_currentEntry];
        }

        void EntrySelected(object sender, object e)
        {
            _currentEntry = (_currentEntry + 1) % _entryValues.Length;

            SetMenuEntryText();
        }
    }
}
