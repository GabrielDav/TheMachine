using System;
using System.Collections.Generic;
using Engine.Core;
using Engine.Graphics;
using Engine.ScreenManagement;
using GameLibrary.Gui.ScreenManagement.ScreenManager;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input.Touch;
using TheGoo;

namespace GameLibrary.Gui.ScreenManagement.Screens
{
    public class MenuScreen : GameScreen
    {
        public Vector2 TitlePosition;
        public string MenuTitle;
        private readonly IList<MenuEntry> _menuEntries = new List<MenuEntry>(); 
        protected IList<MenuEntry> MenuEntries { get { return _menuEntries; } }
        protected IList<GameObject> _objects = new List<GameObject>();

        public MenuScreen()
        {
            EnabledGestures = GestureType.Tap;

            TransitionOnTime = TimeSpan.FromSeconds(0.5);
            TransitionOffTime = TimeSpan.FromSeconds(0.5);
        }

        public override void InputOnPress(Point point, int id)
        {
            for (int i = 0; i < MenuEntries.Count; i++)
            {
                MenuEntry menuEntry = MenuEntries[i];

                if (menuEntry.Rectangle.Contains(point))
                {
                    OnSelectEntry(i);
                }
            }
        }

        protected virtual void OnSelectEntry(int entryIndex)
        {
            MenuEntries[entryIndex].OnSelectEntry();
        }

        protected virtual void OnCancel()
        {
            ExitScreen();
        }

        protected virtual void UpdateMenuEntryLocations()
        {
            
        }

        public override void Draw(GameTime gameTime)
        {
            EngineGlobals.Batch.Begin(SpriteSortMode.BackToFront, BlendState.NonPremultiplied);

            if (!string.IsNullOrEmpty(MenuTitle))
            {
                EngineGlobals.Batch.DrawString(GameGlobals.MenuGlobals.MenuFont, MenuTitle, TitlePosition, new Color(1.0f, 1.0f, 1.0f, TransitionAlpha));
            }

            foreach (var menuEntry in MenuEntries)
            {
                var clr = menuEntry.Color.ToVector3();
                menuEntry.Draw(this, gameTime, new Color(clr.X, clr.Y, clr.Z, TransitionAlpha));
            }

            foreach (var obj in _objects)
            {
                if (TransitionAlpha < 1f)
                {
                    var clr = obj.Color.ToVector4();
                    obj.Color = new Color(clr.X, clr.Y, clr.Z, TransitionAlpha);
                    obj.Draw();
                    obj.Color = new Color(clr);
                }
                else
                {
                    obj.Draw();
                }
            }

            EngineGlobals.Batch.End();
        }
    }
}
