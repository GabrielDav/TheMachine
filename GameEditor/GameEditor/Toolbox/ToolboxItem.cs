
using GameLibrary;

namespace GameEditor.ToolBox
{
    public class ToolboxItem : ToolboxItemBase
    {

        private int _iconIndex;
        private readonly ToolboxType _type;
        public readonly MapType ObjectMapType;
        public bool Enabled;

        public ToolboxItem(string caption, int iconIndex, ToolboxType type, MapType objMapType = MapType.Game)
        {
            _caption = caption;
            _iconIndex = iconIndex;
            _type = type;
            ObjectMapType = objMapType;
            Enabled = true;
        }

        public int IconIndex
        {
            get
            {
                return _iconIndex;
            }
            set
            {
                _iconIndex = value;
            }
        }

        public ToolboxType TypeInfo
        {
            get
            {
                return _type;
            }
        }

    }
}
