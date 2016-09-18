using Engine.Graphics;
using GameLibrary.Gui.ScreenManagement.NewScreens;

namespace GameLibrary.Objects
{
    public class RedBall : Circle
    {
        public RedBall()
        {
            Animated = true;
            TypeId = GameObjectType.RedBall.ToString();
            Init();
        }

        #if EDITOR
        public override void LoadDefault(string resourceId, int index, int subObjectId)
        {
            Load(resourceId, index);
            Mask.LayerDepth = 0.5f;
            _rectangle = new RectangleF(0,0,50,50);
        }
        #endif
    }
}
