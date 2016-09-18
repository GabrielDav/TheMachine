using System;
using System.ComponentModel;
using System.Windows.Forms;
using Engine.Core;
using Engine.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using XnaGraphicsDeviceControl;
using Timer = System.Windows.Forms.Timer;

namespace GameEditor.Sprite_Editor
{
    public class XnaWindowControl : GraphicsDeviceControl
    {
        protected Timer _timer;
        public Sprite Sprite;
        public SpriteBatch SpriteBatch;
        protected string _selectedAnimation;
        protected DateTime _time;
        protected TimeSpan _totalTime;
        protected SpriteBatch _editorBatch;

        
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public string SelectedAnimation
        {
            get { return _selectedAnimation; }
            set { _selectedAnimation = value; }
        }

        protected override void Initialize()
        {
            _timer = new Timer { Interval = 12 };
            _timer.Tick += TimerOnTick;
            _time = DateTime.Now;
            _timer.Start();
            SpriteBatch = new SpriteBatch(GraphicsDevice);
            
        }

        private void TimerOnTick(object sender, EventArgs eventArgs)
        {
            var elapsedTIme = DateTime.Now - _time;
            _totalTime = _totalTime.Add(elapsedTIme);
            EngineGlobals.GameTime = new GameTime(_totalTime, elapsedTIme);
            Invalidate();
            _time = DateTime.Now;
        }


        protected override void Draw()
        {
            try
            {
                var color = new Color(BackColor.R, BackColor.G, BackColor.B, BackColor.A);
                GraphicsDevice.Clear(color);
                if (Sprite != null)
                {
                    Sprite.Update();

                    if (_selectedAnimation != null)
                    {
                        _editorBatch = EngineGlobals.Batch;
                        EngineGlobals.Batch = SpriteBatch;
                        SpriteBatch.Begin();
                        Sprite.Draw();
                        SpriteBatch.End();
                        EngineGlobals.Batch = _editorBatch;
                    }
                }
            }
            catch (Exception exception)
            {
                MessageBox.Show(exception.Message);
                throw;
            }
        }
    }
}
