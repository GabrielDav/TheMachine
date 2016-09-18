using System;
using System.Collections.Generic;
#if EDITOR
using System.Net.Configuration;
#endif
using Engine.Core;
using Engine.Graphics;
using Engine.ScreenManagement;
using Microsoft.Xna.Framework;
using TheGoo;

namespace GameLibrary.Gui.ScreenManagement.NewScreens
{
    class AchievementScreen : IScreen
    {
        public IScreen Parent { get; set; }
        public List<IScreen> ChildScreens { get; set; }
        public bool IsPopup { get; set; }
        public ScreenState State { get; set; }

        protected Achievement _achievement;
        protected TextRegion _achievementText;
        protected TextRegion _achievementStatus;
        protected TextRegion _acievementDescription;
        protected TextRegion _continuTextRegion;
        protected Image _achievementImage;
        protected Image _background;
        protected ColorEffect _continueFadeOut;
        protected ColorEffect _continueFadeIn;
        protected bool _locked;
        protected Timer _timer;

        public AchievementScreen(Achievement achievement)
        {
            _achievement = achievement;
            if (GameGlobals.SaveData.UnlockedAchievements.Contains((int) achievement))
                _locked = false;
            else
            {
                _locked = true;
            }
        }

        public void Load()
        {
             _background = new Image(new GameTexture("GUI\\AchievementScreen"), new Rectangle(125, 110, 550, 220)) { LayerDepth = 0.8f, Owner = this };
            _achievementImage = new Image(EngineGlobals.Resources.Textures["Achievement"][(int)_achievement]) { LayerDepth = 0.5f, Owner = this};
            if (_locked)
                _achievementImage.Transparency = 50;
            _achievementImage.Rect = new Rectangle((int) _background.X + 30, (int) _background.Y + 40, 100, 100);
            _achievementText =
                new TextRegion(new Rectangle((int) _background.X, (int) _background.Y + 5, _background.Width, 25),
                    GameGlobals.MenuGlobals.MenuFont, Color.Black,
                    /*"Achievement: " +*/ GameGlobals.GetAchievementName(_achievement), false)
                {
                    Owner = this,
                    LayerDepth = 0.5f,
                    HorizontaAlign = FontHorizontalAlign.Center
                };
            _acievementDescription =
                new TextRegion(new Rectangle((int) _background.X + 160, (int) _background.Y + 50, _background.Width - 160, 180),
                    GameGlobals.Font, Color.Black, GameGlobals.GetAchievementDescription(_achievement), true)
                {
                    LayerDepth = 0.5f,
                    Owner = this
                };
            _achievementStatus =
                new TextRegion(
                    new Rectangle((int)_background.X,
                        (int)_achievementImage.Rect.Y + (int)_achievementImage.Rect.Height + 4, 160, 25), GameGlobals.MenuGlobals.MenuFont,
                    new Color(0, 0, 0, _locked? 75 : 255), _locked? "Locked" : "Unlocked", false) {Owner = this, LayerDepth = 0.5f, HorizontaAlign = FontHorizontalAlign.Center};
            _continuTextRegion = new TextRegion(
                    new Rectangle((int)_background.X,
                        (int)_background.Y - 45 + _background.Height, _background.Width, 25), GameGlobals.MenuGlobals.MenuFont,
                    new Color(0, 0, 0, 75), "Continue", false) { Owner = this, LayerDepth = 0.5f, HorizontaAlign = FontHorizontalAlign.Center };
            _timer = new Timer();

        }

        public void Initialize()
        {
            _continuTextRegion.Color = new Color(0, 0, 0, 0);
            _continueFadeIn = new ColorEffect(_continuTextRegion, new Color(0, 0, 0, 255), 500);
            _continuTextRegion.Color = Color.Black;
            _continueFadeOut = new ColorEffect(_continuTextRegion, new Color(0, 0, 0, 0), 500);
            _timer.Start(500);
        }

       

        public void Dispose()
        {

        }

        public void HandleTouch(Point p, object sender)
        {
            if (!_timer.Finished)
                return;
            EngineGlobals.ScreenManager.QueueClosePopup();
        }

        public void HandleBack(object sender)
        {
            if (!_timer.Finished)
                return;
            EngineGlobals.ScreenManager.QueueClosePopup();
        }

        public void Update(GameTime gameTime)
        {
            if (_continueFadeOut.Finished)
            {
                _continueFadeIn.Update();
                if (_continueFadeIn.Finished)
                {
                    _continueFadeOut.Reset(new Color(0, 0, 0, 0), 500);
                }
            }
            else
            {
                _continueFadeOut.Update();
                if (_continueFadeOut.Finished)
                    _continueFadeIn.Reset(new Color(0, 0, 0, 255), 500);
            }
            _timer.Update();
        }

        public void Draw(GameTime gameTime)
        {
            EngineGlobals.Batch.Begin();
            _background.Draw();
            _achievementImage.Draw();
            _achievementText.Draw();
            _acievementDescription.Draw();
            _achievementStatus.Draw();
            _continuTextRegion.Draw();
            EngineGlobals.Batch.End();
        }
    }
}
