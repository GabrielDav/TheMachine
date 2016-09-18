using Engine.Core;
using Engine.Graphics;
using GameLibrary.Gui.ScreenManagement.NewScreens;
using Microsoft.Xna.Framework;

namespace GameLibrary.Objects
{
    public class TrapDoors : Spike
    {

        public int OpenTime { get; set; }
        public int HowerTime { get; set; }
        public int CloseTime { get; set; }

        public enum State { Opening, Howering, Closing, Closed }

        public State CurrentState { get; set; }

        protected Timer _timer;
        protected MoveEffectP _moveEffect;

        public TrapDoors()
        {
            TypeId = GameObjectType.TrapDoor.ToString();
            
        }

        protected virtual void StartNextStep()
        {
            if (!IsActivated)
            {
                return;
            }
            switch (CurrentState)
            {
                case State.Opening:
                    CurrentState = State.Howering;
                    _timer.Start(HowerTime, false);
                    break;
                case State.Closed:
                    switch (Orientation)
                    {
                        case Orientation.Top:
                            _moveEffect.Reset(Pos.ToVector(), new Vector2(Pos.X, Pos.Y - 50), OpenTime);
                            break;
                        case Orientation.Right:
                            _moveEffect.Reset(Pos.ToVector(), new Vector2(Pos.X + 50, Pos.Y), OpenTime);
                            break;
                        case Orientation.Bottom:
                            _moveEffect.Reset(Pos.ToVector(), new Vector2(Pos.X, Pos.Y + 50), OpenTime);
                            break;
                        case Orientation.Left:
                            _moveEffect.Reset(Pos.ToVector(), new Vector2(Pos.X - 50, Pos.Y), OpenTime);
                            break;
                    }
                    CurrentState = State.Opening;
                    _timer.Start(OpenTime, false);
                    break;
                case State.Howering:
                    switch (Orientation)
                    {
                        case Orientation.Top:
                            _moveEffect.Reset(Pos.ToVector(), new Vector2(Pos.X, Pos.Y + 50), OpenTime);
                            break;
                        case Orientation.Right:
                            _moveEffect.Reset(Pos.ToVector(), new Vector2(Pos.X - 50, Pos.Y), OpenTime);
                            break;
                        case Orientation.Bottom:
                            _moveEffect.Reset(Pos.ToVector(), new Vector2(Pos.X, Pos.Y - 50), OpenTime);
                            break;
                        case Orientation.Left:
                            _moveEffect.Reset(Pos.ToVector(), new Vector2(Pos.X + 50, Pos.Y), OpenTime);
                            break;
                    }
                    CurrentState = State.Closing;
                    _timer.Start(CloseTime, false);
                    break;
                case State.Closing:
                    CurrentState = State.Closed;
                    _timer.Stop();
                    IsActivated = false;
                    break;
            }
        }

        public override void Update()
        {
            base.Update();
            if (IsActivated)
            {
                if (!_timer.Started)
                {
                    StartNextStep();
                }
                if (CurrentState == State.Opening || CurrentState == State.Closing)
                    _moveEffect.Update();
                _timer.Update();
                if (_timer.Finished && _moveEffect.Finished)
                
                    StartNextStep();
            }
        }

        public override void Load(string resourceId, int index)
        {
            base.Load(resourceId, index);
            _timer = new Timer(true);
            _moveEffect = new MoveEffectP(this, new Vector2(), 0);
        }


#if EDITOR
        public override void LoadDefault(string resourceId, int index, int subObjectId)
        {
            base.LoadDefault(resourceId, index, subObjectId);
            Orientation = Orientation.Top;
            SetRectangle(0, 0, 30, 100);
            OpenTime = 1000;
            HowerTime = 3000;
            CloseTime = 1000;
            CurrentState = State.Closed;
        }
#endif

    }
}
