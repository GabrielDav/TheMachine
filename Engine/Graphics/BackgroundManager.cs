using System;
using Engine.Core;
using Engine.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace GameLibrary
{
    public class BackgroundManager : IGraphicsObject, IDisposable
    {
        protected Texture2D _texture;
        protected Vector2 _drawPos;
        protected Rectangle _visibleRect;
        protected int _mapWidth;
        protected int _mapHeight;

        public BackgroundManager(int mapWidth, int mapHeight)
        {
            Controller.CameraMove += OnCameraMove;
            _mapWidth = mapWidth;
            _mapHeight = mapHeight;
        }

        public void LoadBackground(GameTexture texture)
        {
            _texture = texture.Data;
            OnCameraMove(null);
        }

        private void OnCameraMove(object sender)
        {
            if (_texture == null)
                return;
            _drawPos = EngineGlobals.Camera2D.Position/EngineGlobals.Camera2D.BackgroundSpeedModifier;
            _visibleRect = new Rectangle();
            var startPos = new Vector2();
            var camRect = EngineGlobals.Camera2D.CameraRectangle;
                camRect.X /= EngineGlobals.Camera2D.BackgroundSpeedModifier;
                camRect.Y /= EngineGlobals.Camera2D.BackgroundSpeedModifier;

            if (camRect.X <= 0)
                startPos.X = 0;
            else
            {
                startPos.X = (int)(_texture.Width * Math.Floor((float)camRect.X / _texture.Width));
            }
            //startPos.X = (int) (EngineGlobals.Camera2D.CameraRectangle.X - _minBounds.X);

            if (camRect.Y <= 0)
                startPos.Y = 0;
            else
            {
                startPos.Y = (int)(_texture.Height * Math.Floor((float)camRect.Y / _texture.Height));
            }
            _visibleRect.X = (int)startPos.X;
            _visibleRect.Y = (int)startPos.Y;


            if (camRect.X + EngineGlobals.Device.Viewport.Width / EngineGlobals.Camera2D.Zoom > _mapWidth)
                _visibleRect.Width = (_mapWidth - _visibleRect.X) / _texture.Width;
            else
            {
                var width = Math.Ceiling((camRect.X + EngineGlobals.Device.Viewport.Width/EngineGlobals.Camera2D.Zoom)/
                            _texture.Width) - Math.Floor(camRect.X / (float)_texture.Width);
                _visibleRect.Width = (int)width;
            }

            if (camRect.Y + EngineGlobals.Device.Viewport.Height / EngineGlobals.Camera2D.Zoom > _mapHeight)
                _visibleRect.Height = (_mapHeight - _visibleRect.Y) / _texture.Height;
            else
            {
                var height = Math.Ceiling((camRect.Y + EngineGlobals.Device.Viewport.Height / EngineGlobals.Camera2D.Zoom) /
                            _texture.Height) - Math.Floor(camRect.Y / (float)_texture.Height);
                _visibleRect.Height = (int) height;
            }
        }

        public bool StaticPosition { get { return false; } set{} }
        public bool IgnoreCulling { get { return true; } }
        public Rectangle Rect { get { throw new NotImplementedException();} set { throw new NotImplementedException();} }
        [ContentSerializerIgnore]
        public Rectangle CornerRectangle { get { return Rect; } }
        public event SimpleEvent OnPositionTypeChanged;
        public void Draw()
        {
            if (_texture == null)
                return;
            if (_visibleRect.Width < 1 || _visibleRect.Height < 1)
                return;
            var pos = new Vector2(_visibleRect.X, _visibleRect.Y);
            for (int x = 0; x < _visibleRect.Width; x++)
            {
                for (int y = 0; y < _visibleRect.Height; y++)
                {
                    EngineGlobals.Batch.Draw(_texture, pos, null, Color.White, 0f, new Vector2(0,0), 1f, SpriteEffects.None, 0.9f);
                    pos.Y += _texture.Height;
                }
                pos.X += _texture.Width;
                pos.Y = _visibleRect.Y;
            }
        }

        public void UnloadBackgroundTexture()
        {
            _texture = null;
        }

        public void Dispose()
        {
            OnPositionTypeChanged = null;
            Controller.CameraMove -= OnCameraMove;
        }
    }
}
