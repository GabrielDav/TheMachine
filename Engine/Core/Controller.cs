using System;
using System.Collections.Generic;
using Engine.Graphics;
using Engine.Mechanics;
using Engine.ScreenManagement;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace Engine.Core
{
    public static class Controller
    {
        private static Game _game;

        private static List<IGraphicsObject> _drawableObjects;
        private static List<IGraphicsObject> _onScreenObjects;
        private static List<PhysicalObject> _backgroundObjects;
        private static List<PhysicalObject> _gameObjects;
        private static List<IEffect> _effects;
        private static List<IDynamic> _dynamicObjects;
        private static List<IEffect> _effectsToRemove;
        private static List<IEffect> _effectAddQueue;
        public static Game CurrentGame;
        public static RectangleF UpdateCheckRectangle;
        public static RectangleF DrawCheckRectangle;
        public static RectangleF BackgroundDrawRectangle;

        public static event SimpleEvent CameraMove;

        public static int DrawnObjectsCount;

        public static int UpdatedObjectCount;

        private static bool _reallocateObjectStack;
        private static PhysicalObject _reallocationSource;
        private static PhysicalObject _reallocationTarget;

#if DEBUG_TIMER
        public static long CurrentUpdateId;
#endif

        public static void OnCameraMove()
        {
            CalculateUpdateRectangle();
            if (CameraMove != null)
            {
                CameraMove(null);
            }
        }

        private static void InitController()
        {
            _drawableObjects = new List<IGraphicsObject>();
            _onScreenObjects = new List<IGraphicsObject>();
            _backgroundObjects = new List<PhysicalObject>();
            _gameObjects = new List<PhysicalObject>();
            _dynamicObjects = new List<IDynamic>();
            _effects = new List<IEffect>();
            _effectsToRemove = new List<IEffect>();
            _effectAddQueue = new List<IEffect>();
        }

        public static void InitGame(Game game)
        {
            EngineGlobals.Graphics = new GraphicsDeviceManager(game);
            game.Content.RootDirectory = "GameContent";
#if WINDOWS_PHONE
            EngineGlobals.Graphics.IsFullScreen = true;
          //  EngineGlobals.Graphics.GraphicsProfile = GraphicsProfile.Reach;

           // EngineGlobals.Graphics.PreferredBackBufferWidth = 480;
           // EngineGlobals.Graphics.PreferredBackBufferHeight = 800;
           // EngineGlobals.Graphics.ApplyChanges();
#endif
            game.IsFixedTimeStep = true; //false
            game.TargetElapsedTime = TimeSpan.FromTicks(333333);
            game.InactiveSleepTime = TimeSpan.FromSeconds(5);
            _game = game;
            InitController();
            EngineGlobals.Input = new Input();
            EngineGlobals.ScreenManager = new ScreenManager(game);
            game.Components.Add(EngineGlobals.ScreenManager);
            CurrentGame = game;
            EngineGlobals.Resources = new ResourcesManager();
            
        }

        public static void InitFormWindow(GraphicsDevice device, ContentManager contentManager)
        {
            EngineGlobals.Device = device;
            EngineGlobals.Viewport = device.Viewport;
            EngineGlobals.ContentCache = contentManager;
            EngineGlobals.Batch = new SpriteBatch(device);
            // ReSharper disable UseObjectOrCollectionInitializer
            EngineGlobals.Camera2D = new Camera2D();
            EngineGlobals.Camera2D.CornerPos = new Vector2(0, 0);
            // ReSharper restore UseObjectOrCollectionInitializer
            EngineGlobals.Input = new Input();
            EngineGlobals.Storage = new StorageControl();
            EngineGlobals.ParticleStorageManager = new ParticleStorageManager();
            InitController();

            var rectangleTexture = new Texture2D(EngineGlobals.Device, 64, 64, false, SurfaceFormat.Color);
            var color = new Color[64*64];

            for (int i = 0; i < color.Length; i++)
            {
                color[i] = new Color(255, 255, 255, 255);
            }
            rectangleTexture.SetData(color);

            EngineGlobals.RegionTexture = new GameTexture(rectangleTexture);

        }

        public static void BaseInit()
        {
            EngineGlobals.Device = _game.GraphicsDevice;
            EngineGlobals.Viewport = EngineGlobals.Device.Viewport;
            EngineGlobals.Batch = new SpriteBatch(_game.GraphicsDevice);
            //EngineGlobals.Content = _game.Content;
            
            
            EngineGlobals.Storage = new StorageControl();
            EngineGlobals.ParticleStorageManager = new ParticleStorageManager();

#if EDITOR
            _game.IsMouseVisible = true;
            EngineGlobals.Graphics.PreferredBackBufferWidth = 800;
            EngineGlobals.Graphics.PreferredBackBufferHeight = 480;
            EngineGlobals.Graphics.ApplyChanges();
#endif
            
        }

#if EDITOR
        /*private static List<IGraphicsObject> _drawableObjects;
        private static List<IGraphicsObject> _onScreenObjects;
        private static List<PhysicalObject> _backgroundObjects;
        private static List<PhysicalObject> _gameObjects;
        private static List<IEffect> _effects;
        private static List<IDynamic> _dynamicObjects;*/
        /// <summary>
        /// Returns all objects buffers in following order:
        /// DrawableObjects, OnScreenObjects, BackgroundObjects, GameObjects, Effects,DynamicObjects
        /// </summary>
        /// <returns>Objects buffers</returns>
        public static Tuple<List<IGraphicsObject>, 
            List<IGraphicsObject>, 
            List<PhysicalObject>, 
            List<PhysicalObject>, 
            List<IEffect>, 
            List<IDynamic>> GetObjectsBuffers()
        {
            return new Tuple
                <List<IGraphicsObject>,
                List<IGraphicsObject>,
                List<PhysicalObject>,
                List<PhysicalObject>,
                List<IEffect>
                    , List<IDynamic>>(
                new List<IGraphicsObject>(_drawableObjects),
                new List<IGraphicsObject>(_onScreenObjects),
                new List<PhysicalObject>(_backgroundObjects),
                new List<PhysicalObject>(_gameObjects),
                new List<IEffect>(_effects),
                new List<IDynamic>(_dynamicObjects)
                );
        }
#endif

        public static void ClearObjectsBuffers()
        {
            _backgroundObjects.Clear();
            _gameObjects.Clear();
            _drawableObjects.Clear();
            _onScreenObjects.Clear();
            _dynamicObjects.Clear();
            _effects.Clear();
        }

        public static void AddEffectQueue(IEffect effect)
        {
            _effectAddQueue.Add(effect);
        }

        public static void RemoveEffect(IEffect effect)
        {
            _effects.Remove(effect);
        }

        public static void AddObject(IGraphicsObject gameObject)
        {
            var dynamicObject = gameObject as IDynamic;
            if (dynamicObject != null)
            {
                AddDynamicObject(dynamicObject);
            }
            if (gameObject.StaticPosition)
            {
                if (_onScreenObjects.Contains(gameObject))
                    throw new Exception("GameObject already registered.");
                _onScreenObjects.Add(gameObject);
            }
            else
            {
                if (_drawableObjects.Contains(gameObject))
                    throw new Exception("GameObject already registered.");
                _drawableObjects.Add(gameObject);
            }
            //gameObject.OnPositionTypeChanged += GameObjectPositionTypeChanged;
        }

        public static void AddBackgroundObject(PhysicalObject backgroundObject)
        {
            if (_gameObjects.Contains(backgroundObject))
                throw new Exception("GameObject '" + backgroundObject.Name + "' already registered");
            if (_drawableObjects.Contains(backgroundObject.Mask))
                throw new Exception("Background object mask already registered in drawable objects");
            _backgroundObjects.Add(backgroundObject);
        }

        public static void AddGameObject(PhysicalObject gameObject)
        {
            if (_gameObjects.Contains(gameObject))
                throw new Exception("GameObject '" + gameObject.Name + "' already registered");
            if (_drawableObjects.Contains(gameObject.Mask))
                throw new Exception("Game object mask already registered in drawable objects");
            _gameObjects.Add(gameObject);
        }

        public static void RemoveBackgroundObject(PhysicalObject backgroundObject)
        {
            _backgroundObjects.Remove(backgroundObject);
        }

        public static void RemoveGameObject(PhysicalObject gameObject)
        {
            _gameObjects.Remove(gameObject);
        }

        public static void AddDynamicObject(IDynamic dynamicObject)
        {
            if (_dynamicObjects.Contains(dynamicObject))
                throw new Exception("Dynamic object already registered.");
            _dynamicObjects.Add(dynamicObject);
        }

        public static void ClearBackgroundobjects()
        {
            _backgroundObjects.Clear();
        }

        public static void ClearGameObjects()
        {
            _gameObjects.Clear();
        }

        public static void RemoveObject(IGraphicsObject gameObject)
        {
            var dynamic = gameObject as IDynamic;
            if (dynamic != null)
            {
                _dynamicObjects.Remove(dynamic);
            }

            if (gameObject.StaticPosition)
            {
                _onScreenObjects.Remove(gameObject);
            }
            else
                _drawableObjects.Remove(gameObject);
            gameObject.OnPositionTypeChanged -= GameObjectPositionTypeChanged;
        }

        private static void GameObjectPositionTypeChanged(object sender)
        {
            var obj = (GameObject) sender;
            if (obj.StaticPosition)
            {
                _drawableObjects.Remove(obj);
                _onScreenObjects.Add(obj);
            }
            else
            {
                _drawableObjects.Add(obj);
                _onScreenObjects.Remove(obj);
            }
        }

        public static void ClearRelativeObjects()
        {
            _drawableObjects.Clear();
        }

        public static void ClearDynamciObject()
        {
            _dynamicObjects.Clear();
        }

        public static void Update()
        {
            UpdatedObjectCount = 0;
#if DEBUG_TIMER
            CurrentUpdateId++;
#endif
            // Game objects update
            foreach (var physicalObject in _gameObjects)
            {
                var update = true;
                if (!physicalObject.IgnoreCulling && !physicalObject.GlobalUpdate)
                {
                    update = UpdateCheckRectangle.Intersects(physicalObject.Rectangle);
                }

                if (update)
                {
                    UpdatedObjectCount++;
                    physicalObject.Update();
                }
            }

            // Background objects update
            foreach (var backgroundObject in _backgroundObjects)
            {
                backgroundObject.Update();
            }

            // Dynamic objects update
            foreach (var dynamicObject in _dynamicObjects)
            {
                dynamicObject.Update();
            }

            // Effects update
            if (_effectAddQueue.Count > 0)
            {
                foreach (var effect in _effectAddQueue)
                {
                    _effects.Add(effect);
                }
                _effectAddQueue.Clear();
            }

            if (_reallocateObjectStack)
            {
                var sourceIndex = _gameObjects.IndexOf(_reallocationSource);
                var targetIndex = _gameObjects.IndexOf(_reallocationTarget);
                if (sourceIndex < targetIndex)
                {
                    var tmp = _gameObjects[sourceIndex];
                    _gameObjects[sourceIndex] = _gameObjects[targetIndex];
                    _gameObjects[targetIndex] = tmp;
                    _reallocateObjectStack = false;
                }
            }
            if (_effects.Count == 0)
                return;
            foreach (var effect in _effects)
            {
                effect.Update();
                if (effect.Finished)
                {
                    _effectsToRemove.Add(effect);
                }
            }
            if (_effectsToRemove.Count < 1) return;
            foreach (var effect in _effectsToRemove)
            {
                _effects.Remove(effect);
            }
            _effectsToRemove.Clear();
        }

        public static void ReallocateObjectsStack(PhysicalObject source, PhysicalObject target)
        {
            _reallocateObjectStack = true;
            _reallocationSource = source;
            _reallocationTarget = target;
        }

        public static void Draw()
        {
            DrawnObjectsCount = 0;

            // Background
            EngineGlobals.Batch.Begin(
                SpriteSortMode.BackToFront,
                BlendState.NonPremultiplied,
                null,
                null,
                null,
                null,
                EngineGlobals.Camera2D.GetBackgroundTransformation());
            if (EngineGlobals.Background != null)
            {
                EngineGlobals.Background.Draw();
            }

            foreach (var backgroundObject in _backgroundObjects)
            {
                if (BackgroundDrawRectangle.Intersects(backgroundObject.Rectangle))
                {
                    DrawnObjectsCount++;
                    backgroundObject.Draw();
                }
            }
            EngineGlobals.Batch.End();

            // Foreground objects
            EngineGlobals.Batch.Begin(SpriteSortMode.BackToFront, BlendState.NonPremultiplied, null, null, null, null,
                                      EngineGlobals.Camera2D.GetTransformation());

            foreach (var drawableObject in _drawableObjects)
            {
                if (drawableObject.IgnoreCulling || DrawCheckRectangle.Intersects(drawableObject.CornerRectangle))
                {
                    DrawnObjectsCount++;
                    drawableObject.Draw();
                }
            }

            
            foreach (var physicalObject in _gameObjects)
            {
                if (DrawCheckRectangle.Intersects(physicalObject.Rectangle))
                {
                    DrawnObjectsCount++;
                    physicalObject.Draw();
                }
            }

            EngineGlobals.Batch.End();


            //GUI
            EngineGlobals.Batch.Begin(SpriteSortMode.BackToFront, BlendState.NonPremultiplied);
            foreach (var onScreenObject in _onScreenObjects)
            {
                DrawnObjectsCount++;
                onScreenObject.Draw();
            }

            EngineGlobals.Batch.End();
        }

        public static void CalculateUpdateRectangle()
        {
            if (EngineGlobals.Camera2D == null)
                return;
            
            UpdateCheckRectangle = new RectangleF(EngineGlobals.Camera2D.Position.X - 1600, EngineGlobals.Camera2D.Position.Y - 920, 3200, 1840);
            var rectangle = new RectangleF(EngineGlobals.Camera2D.CameraRectangle);
            //DrawCheckRectangle = new RectangleF(rectangle.X - 50, rectangle.Y - 50, rectangle.Width + 100, rectangle.Height + 100);
            DrawCheckRectangle = new RectangleF(rectangle.X - rectangle.Width/2, rectangle.Y - rectangle.Height/2, rectangle.Width*2, rectangle.Height*2);
            var backgroundRectangle = new RectangleF(EngineGlobals.Camera2D.CameraBackgroundRectangle);
            BackgroundDrawRectangle = new RectangleF(backgroundRectangle.X, backgroundRectangle.Y, backgroundRectangle.Width, backgroundRectangle.Height);
        }

        public static List<PhysicalObject> GetGameObjects()
        {
            return _gameObjects;
        }
    }
}
