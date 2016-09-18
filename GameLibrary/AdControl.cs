using System;
using System.ComponentModel;
using System.IO;
using System.IO.IsolatedStorage;
using Engine.Core;
using Engine.Graphics;
using GameLibrary.Objects;
using Microsoft.Xna.Framework.Graphics;
using TheGoo;
#if WINDOWS_PHONE
using Microsoft.Phone.Tasks;
using SOMAWP7;
#endif
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input.Touch;

namespace GameLibrary
{
    internal class AdControl : IDynamic, IGraphicsObject
    {
        #if WINDOWS_PHONE
        SomaAd _somaAd;
        #endif
        protected Vector2 _adPosition;
        protected Vector2 _adSize;
        protected Texture2D _adTexture;
        protected Texture2D _adBackground;
        protected string _currentAdImageFileName = "";
        protected Vector2 _origin;
        protected float _rotation;
        protected Rectangle _backgroundRectangle;
        protected bool _bannerSet;
        protected BackgroundWorker _backgroundWorker;
        public bool StaticPosition
        {
            get { return true; }
            set { throw new NotImplementedException(); }
        }

        public bool IgnoreCulling
        {
            get { return true; }
            set { throw new NotImplementedException(); }
        }

        public Rectangle Rect { get; set; }
        public Rectangle CornerRectangle { get; private set; }
        public event SimpleEvent OnPositionTypeChanged;

        public AdControl(Vector2 adPosition, Vector2 adSize)
        {
            _adPosition = adPosition;
            _adSize = adSize;
        }

        public void Init()
        {
            #if WINDOWS_PHONE
            _somaAd = new SomaAd
            {
                Adspace = 65816665,
                Pub = 923872969,
                AdSpaceHeight = (int)_adSize.Y,
                AdSpaceWidth = (int)_adSize.X,
            };
            _somaAd.GetAd();
            #endif
            _adTexture = EngineGlobals.ContentCache.Load<Texture2D>("Banner2");
            _adBackground = EngineGlobals.ContentCache.Load<Texture2D>("AddBack");
            _backgroundRectangle = new Rectangle(0, 0, 80, 480);
            _origin = new Vector2(_adTexture.Width/2f, _adTexture.Height/2f);
            #if WINDOWS_PHONE
            _backgroundWorker = new BackgroundWorker();
            _backgroundWorker.DoWork += BackgroundWorkerOnDoWork;
            #endif
        }
        
        #if WINDOWS_PHONE
        private void BackgroundWorkerOnDoWork(object sender, DoWorkEventArgs doWorkEventArgs)
        {
            try
            {
                var tempAdFileImage = _somaAd.AdImageFileName;
                var myIsoStore = IsolatedStorageFile.GetUserStoreForApplication();
                var myAd = new IsolatedStorageFileStream(_somaAd.AdImageFileName, FileMode.Open, myIsoStore);
                _adTexture = Texture2D.FromStream(EngineGlobals.Device, myAd);
                _origin = new Vector2(_adTexture.Width/2f, _adTexture.Height/2f);
                myAd.Close();
                myAd.Dispose();
                myIsoStore.Dispose();
                _currentAdImageFileName = tempAdFileImage;
                _bannerSet = true;
            }
            catch
            {
                _bannerSet = false;
            }
        }
        #endif

        public void Setup(Vector2 position, Orientation orientation)
        {
            _adPosition = position;
            switch (orientation)
            {
                    case Orientation.Top:
                    _rotation = 0;
                    _backgroundRectangle = new Rectangle(0, 0, 800, (int)_adSize.Y);
                    break;
                    case Orientation.Bottom:
                    _rotation = 0;
                    _backgroundRectangle = new Rectangle(0, 480 - (int)_adSize.Y, 800, (int)_adSize.Y);
                    break;
                case Orientation.Left:
                    _rotation = MathHelper.PiOver2;
                    _backgroundRectangle = new Rectangle(0, 0, (int)_adSize.Y, 480);
                    break;
                case Orientation.Right:
                    _rotation = MathHelper.PiOver2;
                    _backgroundRectangle = new Rectangle(800 - (int)_adSize.Y, 0, (int)_adSize.Y, 480);
                    break;
            }

            #if WINDOWS_PHONE
            _somaAd.GetAd();
            #endif
        }

        public bool OnPress(Point point)
        {
            #if WINDOWS_PHONE
            if ((((_rotation < EngineGlobals.Epsilon) &&
                (point.X >= _adPosition.X - _adSize.X/2f &&
                 point.X <= _adPosition.X + _adSize.X/2f &&
                 point.Y >= _adPosition.Y - _adSize.Y/2f &&
                 point.Y <= _adPosition.Y + _adSize.Y/2f)) ||
                ((Math.Abs(_rotation - MathHelper.PiOver2) < EngineGlobals.Epsilon) &&
                 (point.X >= _adPosition.X - _adSize.Y/2f &&
                  point.X <= _adPosition.X + _adSize.Y/2f &&
                  point.Y >= _adPosition.Y - _adSize.X/2f &&
                  point.Y <= _adPosition.Y + _adSize.X/2f))))
            {
                if (_bannerSet)
                {
                    var webBrowserTask = new WebBrowserTask { Uri = new Uri(_somaAd.Uri) };
                    webBrowserTask.Show();
                    return true;
                }
                else
                {
                    var marketplace = new MarketplaceDetailTask()
                    {
                        ContentIdentifier = GameGlobals.AdFreeAppId,
                        ContentType = MarketplaceContentType.Applications
                    };
                    marketplace.Show();
                    return true;
                }
            }
            if (_backgroundRectangle.Contains(point))
            {
                return true;
            }
            #endif
            return false;
        }

        public void Update()
        {
            #if WINDOWS_PHONE

            if (_somaAd.Status == "success" && _somaAd.AdImageFileName != null && _somaAd.ImageOK)
            {
                try
                {
                    if (!_backgroundWorker.IsBusy && _currentAdImageFileName != _somaAd.AdImageFileName)
                    {
                        _backgroundWorker.RunWorkerAsync();
                    }
                }
                catch (Exception)
                {
                    _bannerSet = false;
                }
               
            }
            #endif
        }

        public void Draw()
        {
            EngineGlobals.Batch.Draw(_adTexture,
                new Rectangle((int) _adPosition.X, (int) _adPosition.Y, (int) _adSize.X, (int) _adSize.Y), null,
                Color.White, _rotation, _origin, SpriteEffects.None, 0.11f);
            EngineGlobals.Batch.Draw(_adBackground, _backgroundRectangle, null, Color.White, 0, new Vector2(0,0), SpriteEffects.None, 0.13f);
        }
    }
}
