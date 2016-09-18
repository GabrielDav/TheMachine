
using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace Engine.ScreenManagement
{
    public interface IScreen
    {
        IScreen Parent { get; set; }

        List<IScreen> ChildScreens { get; set; }

        bool IsPopup
        {
            get; set;
        }

        ScreenState State
        {
            get;
            set;
        }
    
        void Initialize();

        void Load();

        void Dispose();

        void HandleTouch(Point p, object sender);

        void HandleBack(object sender);

        void Update(GameTime gameTime);

        void Draw(GameTime gameTime);
    }
}
