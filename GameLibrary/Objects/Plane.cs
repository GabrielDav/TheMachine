using GameLibrary.Gui.ScreenManagement.NewScreens;
using TheGoo;

namespace GameLibrary.Objects
{
    public class Plane : BoxPhysicalObject
    {
        public Plane()
        {
            TypeId = GameObjectType.Plane.ToString();
            Init();
        }

#if EDITOR
        public override void LoadDefault(string resourceId, int index, int subObjectId)
        {
            _resourceId = resourceId;
            _resourceVariation = index;
            Load(resourceId, index);
            SetRectangle(0,0,100,30);
            Mask.LayerDepth = 0.5f;          
        }
#endif

        public override void Load(string resourceId, int index)
        {
            base.Load(resourceId, index);
#if !EDITOR
            if (!GameGlobals.EditorMode)
                Mask.IsHidden = true;
#endif
        }

    }
}
