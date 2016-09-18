using Engine.Core;
using Microsoft.Xna.Framework;

namespace TheGoo
{
    public interface IGameManager
    {
        event SimpleEvent Exit;
        void Initialize();
        void Load();
        void Update(GameTime gameTime);
        void Draw();
    }
}
