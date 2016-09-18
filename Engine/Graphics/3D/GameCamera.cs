using Microsoft.Xna.Framework;

namespace Engine.Graphics._3D
{
    public class GameCamera
    {
        protected float _minViewRange;
        protected float _maxViewRange;
        protected float _angle;

        public float MinViewRange
        {
            get { return _minViewRange; }
            set
            {
                _minViewRange = value;
                CreateProjection();
            }
        }

        public float MaxViewRange
        {
            get { return _maxViewRange; }
            set
            {
                _maxViewRange = value;
                CreateProjection();
            }
        }

        public float Angle
        {
            get { return _angle; }
            set
            {
                _angle = value;
                CreateProjection();
            }
        }

        public float MovementSpeed;
        public float RotationSpeed;
        public float AspectRatio;

        public Matrix View;
        public Matrix Projection;

        public Vector3 Position;
        public Vector3 LookAt;
        public Vector3 UpVector;
        public GameModel ObjectToFollow;
        public Vector3 PreviousPosition;

        public Vector3 DestinationView;
        public Vector3 DestinationLook;
        public Vector3 StartPosition;
        public Vector3 StartLook;
        private Vector3 _updateVector;
        private Vector3 _updateVectorLook;
        public bool MoveTo;

        public GameCamera(
            float aspectRatio,
            Vector3 position,
            Vector3 lookAt,
            Vector3 upVector,
            float cameraAngle = 45.0f,
            float minViewRange = 0.1f,
            float maxViewRange = 500.0f,
            float movementSpeed = 1f)
        {
            AspectRatio = aspectRatio;
            Position = position;
            LookAt = lookAt;
            UpVector = upVector;
            Angle = cameraAngle;
            _minViewRange = minViewRange;
            _maxViewRange = maxViewRange;
            MovementSpeed = movementSpeed;

            CreateView();

            Projection = Matrix.CreatePerspectiveFieldOfView(MathHelper.ToRadians(Angle), AspectRatio, MinViewRange, MaxViewRange);
        }

        public void CreateView()
        {
            View = Matrix.CreateLookAt(Position, LookAt, UpVector);
        }

        public void CreateProjection()
        {
            if ((MinViewRange < 0.001) || (MaxViewRange < 1))
            {
                return;
            }
         
            Projection = Matrix.CreatePerspectiveFieldOfView(MathHelper.ToRadians(Angle), AspectRatio, MinViewRange, MaxViewRange);
        }

        public void Move(int x, int y, int z)
        {
            Position = new Vector3(Position.X + x, Position.Y + y, Position.Z + z);
            CreateView();
        }

        public void InitFollow(GameModel model)
        {
            MoveTo = false;
            ObjectToFollow = model;
            PreviousPosition = model.Position;
        }

        public void Follow(GameTime gameTime)
        {
            if (ObjectToFollow != null)
            {
                Position += (ObjectToFollow.Position - PreviousPosition);
                LookAt += (ObjectToFollow.Position - PreviousPosition);
                PreviousPosition = ObjectToFollow.Position;
            }
        }

        public void StopFollow()
        {
            ObjectToFollow = null;
        }

        public void InitMoveToPoint(int x, int y)
        {
            StopFollow();
            StartPosition = Position;
            StartLook = LookAt;
            DestinationView = new Vector3(x, y, Position.Z);
            DestinationLook = new Vector3((LookAt.X - Position.X) + x, (LookAt.Y - Position.Y) + y, LookAt.Z);
            var v = DestinationView - Position;
            var v2 = DestinationLook - LookAt;
            _updateVector = v * (MovementSpeed / v.Length());
            _updateVectorLook = v2 * (MovementSpeed / v2.Length());

            MoveTo = true;
        }

        public void Update(GameTime gameTime)
        {
            Follow(gameTime);
            MoveToPoint(gameTime);
            CreateView();
        }

        public float StepToPoint(float start, float end, float step)
        {
            if (start > end)
            {
                return start - step;
            }

            return start + step;
        }

        public void MoveToPoint(GameTime gameTime)
        {
            if (MoveTo)
            {
                Position += _updateVector;
                LookAt += _updateVectorLook;

                if ((Vector3.Distance(Position, DestinationView) <= 1) && (Vector3.Distance(LookAt, DestinationLook) <= 1))
                {
                    Position = DestinationView;
                    LookAt = DestinationLook;
                    MoveTo = false;
                }
            }
        }

        public void MoveCameraX(int x)
        {
            if (!MoveTo)
            {
                Position.X += x;
                LookAt.X += x;
                CreateView();
            }
        }

        public void MoveCameraY(int y)
        {
            if (!MoveTo)
            {
                Position.Y += y;
                LookAt.Y += y;
                CreateView();
            }
        }
    }
}
