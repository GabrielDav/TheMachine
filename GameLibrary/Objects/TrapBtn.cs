using System;
using System.Runtime.Serialization;
using Microsoft.Xna.Framework.Content;
#if EDITOR
using System.ComponentModel;
using System.Drawing;
#endif
using Engine.Core;
using Engine.Graphics;
using Engine.Mechanics;
using Engine.Mechanics.Triggers;
using GameLibrary.Triggers;
using Microsoft.Xna.Framework;
using TheGoo;
using Image = Engine.Graphics.Image;
using Rectangle = Microsoft.Xna.Framework.Rectangle;

namespace GameLibrary.Objects
{
    public class TrapBtn : Plane
    {
        //protected Timer _timer;

       /* public enum Details
        {
            Cube = 0,
            Gear1 = 1,
            Gear2 = 2,
            SubButton = 3,
            CubeWhite = 4

        }*/
        protected bool _isDeactivating;

        public virtual bool IsOn { get; set; }
        public virtual int Time { get; set; }
        public virtual bool AllowRepetitvePush { get; set; }
        protected Image _switchImage;
        protected RotateEffect _rotateEffect;
        protected RotateEffect _rotateBackEffect;
        [ContentSerializer(Optional = true)] 
        public virtual bool CountDownEnabled { get; set; }

        protected virtual Timer _soundStartTimer { get; set; }
        //protected Image _fillTexture;
       // protected Image[] _gears;
       // protected Image _pushButton;
       // protected Image _subButton;
       // protected Image _cube;
       // protected Image _cubeWhite;
        // protected MoveEffect _moveEffect;
        // protected MoveEffect _pushMoveEffect;
        // protected ResizeEffect _resizeEffect;

    #if EDITOR
        [ReadOnly(true)]
        public override Size GridSize
        {
            get
            {
                return base.GridSize;
            }
            set
            {
                base.GridSize = value;
            }
        }

#endif

        public TrapBtn()
        {
            TypeId = GameObjectType.TrapBtn.ToString();
            IsActivated = true;
            _soundStartTimer = new Timer(false);
            _soundStartTimer.OnFinish += SoundStartTimerOnOnFinish;
            GlobalUpdate = true;
            //_timer = new Timer(true);
        }

        private void SoundStartTimerOnOnFinish(object sender)
        {
            if (IsOn)
            {
                EngineGlobals.SoundManager.Play("trap_btn_ticking_" + Name, "ticking", this, 1.0f, true, 1.0f, 1.0f);
            }
        }

        public override CollisionResponce Collide(CollisionResult collisionResult)
        {
            if (collisionResult.Source is Player)
            {
                TurnOn();
            }
            return base.Collide(collisionResult);
        }

        public virtual void TurnOn()
        {
            if (!IsActivated)
                return;
            if (IsOn && !AllowRepetitvePush)
                return;

            
            //((Image)Mask).LoadTexture(EngineGlobals.Resources.Textures["TrapBtnOn"][0]);
            //_moveEffect.Reset(_subButton.Pos, new Vector2(Rectangle.X + 25, _subButton.Pos.Y), 400);
            //_pushMoveEffect.Reset(_pushButton.Pos, new Vector2(Rectangle.X - 1, _pushButton.Pos.Y), 100);
            /*if (IsOn)
            {
                _cube.Height = 0;
            }*/
            //_resizeEffect.Reset(new Vector2(_cubeWhite.Width, _cubeWhite.Height), (int)Time);
            //_timer.Start(Time, false);
            if (!IsOn)
            {
                EngineGlobals.SoundManager.Play(
                    "trap_btn",
                    "trapBtn",
                    this,
                    1.0f,
                    false,
                    1.0f,
                    1.0f);
                _rotateEffect.Reset(MathHelper.PiOver2, 600);
                if (CountDownEnabled && Time > 600)
                {
                    _soundStartTimer.OnFinish -= SoundStartTimerOnOnFinish;
                    _soundStartTimer.OnFinish += SoundStartTimerOnOnFinish;
                    _soundStartTimer.Start(600);
                }
            }
            IsOn = true;
            
            EngineGlobals.TriggerManager.ActionOccured((int)GameEventType.TrapButtonOn, new EventParams {TriggeringObject = this});
        }

        public virtual void TurnOff()
        {
            if (!IsActivated)
                return;
            IsOn = false;
            _switchImage.Rotation = 0;
            //if (_timer.Ticking)
             //   _timer.Stop();
            //_moveEffect.Reset(_subButton.Pos, new Vector2(Rectangle.X + 10, _subButton.Pos.Y), 100);
            /*if (GameGlobals.Player.CurrentObject != this)
            {
                _pushMoveEffect.Reset(_pushButton.Pos, new Vector2(Rectangle.X - 10, _pushButton.Pos.Y), 100);
            }
            _resizeEffect.Reset(new Vector2(_cube.Width, 0), 400);*/
            EngineGlobals.SoundManager.Stop("trap_btn_ticking_" + Name);
            EngineGlobals.SoundManager.Play(
                    "trap_btn_switch_off",
                    "switchOff",
                    this,
                    1.0f,
                    false,
                    1.0f,
                    1.0f);
            EngineGlobals.TriggerManager.ActionOccured((int)GameEventType.TrapButtonOff, new EventParams { TriggeringObject = this });
        }



#if EDITOR
        public override void LoadDefault(string resourceId, int index, int subObjectId)
        {
            base.LoadDefault(resourceId, index, subObjectId);
            IsOn = false;
            Time = 5000;
            AllowRepetitvePush = true;
            GridSize = new Size(12, 12);
        }

        public override bool IsResizeAvailable(ResizeType resizeType)
        {
            return false;
        }
#endif

        public override void Update()
        {
           // _moveEffect.Update();
           // _resizeEffect.Update();
           // _pushMoveEffect.Update();
            if (IsOn)
            {
                
                _rotateEffect.Update();
                _soundStartTimer.Update();
                if (_rotateEffect.Finished && CountDownEnabled)
                {
                    if (_isDeactivating || Time == 0)
                    {
                        TurnOff();
                        _isDeactivating = false;
                    }
                    else
                    {
                        _rotateEffect.Reset(0, Time);
                        _isDeactivating = true;
                    }
                }
               /* if (_moveEffect.Finished)
                {
                    _gears[0].Rotation += (0.002f*EngineGlobals.GetElapsedInGameTime());
                    if (_gears[0].Rotation >= MathHelper.Pi*2f)
                    {
                        _gears[0].Rotation = _gears[0].Rotation - MathHelper.Pi*2f;
                    }
                    _gears[1].Rotation -= (0.002f*EngineGlobals.GetElapsedInGameTime());
                    if (_gears[1].Rotation <= -MathHelper.Pi*2f)
                    {
                        _gears[1].Rotation = _gears[1].Rotation + MathHelper.Pi*2f;
                    }
                }*/
                //_timer.Update();
                //if (_timer.Finished)
                //{
                //    TurnOff();
                //}
            }
            base.Update();
        }

        public override void Load(string resourceId, int index)
        {
            base.Load(resourceId, index);
            Mask.IsHidden = false;
            _switchImage = new Image(EngineGlobals.Resources.Textures["TrapBtnDetails"][0],
                                     new Rectangle((int)HalfPos.X, (int)HalfPos.Y, 95, 15)) { LayerDepth = Mask.LayerDepth - 0.01f, Owner = this };
            _switchImage.Orgin = _switchImage.OriginCenter();
            Controller.AddObject(_switchImage);
            _rotateEffect = new RotateEffect(_switchImage, MathHelper.PiOver2, 600);
            /*_fillTexture = new Image(EngineGlobals.Resources.Textures["TrapBtnDetails"][(int)Details.Gear2], new Rectangle(0, 0, 50, 50)) { LayerDepth = Mask.LayerDepth - 0.01f };
            _gears = new[]
                         {
                             new Image(EngineGlobals.Resources.Textures["TrapBtnDetails"][(int) Details.Gear1])
                                 {LayerDepth = Mask.LayerDepth - 0.01f, Rotation = MathHelper.ToRadians(280)},
                             new Image(EngineGlobals.Resources.Textures["TrapBtnDetails"][(int) Details.Gear2])
                                 {LayerDepth = Mask.LayerDepth - 0.01f},
                         };
            foreach (var image in _gears)
            {

                image.Owner = this;
                image.Orgin = image.OriginCenter();
                Controller.AddObject(image);
            }
            _gears[1].Rotation = MathHelper.ToRadians(2);
            _pushButton = new Image(EngineGlobals.Resources.Textures["TrapBtnDetails"][(int)Details.Cube]) { Owner = this, LayerDepth = Mask.LayerDepth + 0.01f };
            _subButton = new Image(EngineGlobals.Resources.Textures["TrapBtnDetails"][(int)Details.SubButton]) { Owner = this, LayerDepth = Mask.LayerDepth - 0.01f };
            Controller.AddObject(_pushButton);
            Controller.AddObject(_subButton);
            _cube = new Image(EngineGlobals.Resources.Textures["TrapBtnDetails"][(int) Details.Cube])
                        {Owner = this, LayerDepth = Mask.LayerDepth + 0.01f};
            _cubeWhite = new Image(EngineGlobals.Resources.Textures["TrapBtnDetails"][(int)Details.CubeWhite]) { Owner = this, LayerDepth = Mask.LayerDepth + 0.02f };
            Controller.AddObject(_cube);
            Controller.AddObject(_cubeWhite);
            SetDetailsRectangle(Rectangle.GetRectangle());
            _fillTexture.Owner = this;
            _fillTexture.IsHidden = true;
            _moveEffect = new MoveEffect(_subButton, _subButton.Pos, 1000);
            _pushMoveEffect = new MoveEffect(_pushButton, new Vector2(_pushButton.Pos.X, _pushButton.Pos.Y), 200 );
            _resizeEffect = new ResizeEffect(_cube, new Vector2(_cubeWhite.Width, 0), 1);
            Controller.AddObject(_fillTexture);*/
        }

#if EDITOR
        public override void UpdatePreviewRectangle(Rectangle rectangle)
        {
            base.UpdatePreviewRectangle(rectangle);
            SetDetailsRectangle(rectangle);
        }

        public override void EditorDeleteObject()
        {
            base.EditorDeleteObject();
           /* Controller.RemoveObject(_fillTexture);
            foreach (var image in _gears)
            {
                Controller.RemoveObject(image);
            }*/
            
        }

#endif

        protected virtual void SetDetailsRectangle(Rectangle rectangle)
        {
            if (_switchImage == null)
                return;
            _switchImage.Rect = new Rectangle((int) HalfPos.X, (int) HalfPos.Y, 95, 15);
            /*  if (_gears == null)
                return;
            _gears[0].Rect = new Rectangle(rectangle.X + 45, rectangle.Y + 42, 70, 70);
            _gears[1].Rect = new Rectangle(rectangle.X + 83, rectangle.Y + 96, 70, 70);
            _pushButton.Rect = new Rectangle(rectangle.X - 8, rectangle.Y + 30, 10, 120);
            _subButton.Rect = new Rectangle(rectangle.X + 10, rectangle.Y + 94, 70, 50);
            _cube.Rect = new Rectangle(rectangle.X + 110, rectangle.Y + 10, 30, 0);
            _cubeWhite.Rect = new Rectangle(rectangle.X + 110, rectangle.Y + 10, 30, 130);*/
        }

        protected override void SetRectangle(float x, float y, float width, float height)
        {
            base.SetRectangle(x, y, width, height);
            SetDetailsRectangle(Rectangle.GetRectangle());
        }

        
        

    }
}
