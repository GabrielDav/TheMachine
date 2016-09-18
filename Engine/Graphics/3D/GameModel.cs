using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Engine.Graphics._3D
{
    public class GameModel
    {
        #region Fields

        private float _size;

        private float _rotationX;
        private float _rotationY;
        private float _rotationZ;

        private float _positionX;
        private float _positionY;
        private float _positionZ;

        public Model Model;

        public Matrix WorldMatrix;

        public Vector3 Position
        {
            get { return new Vector3(_positionX, _positionY, _positionZ); }
            set
            {
                _positionX = value.X;
                _positionY = value.Y;
                _positionZ = value.Z;
            }
        }

        public Vector3 Rotation
        {
            get { return new Vector3(_rotationX, _rotationY, _rotationZ); }
            set
            {
                _rotationX = value.X;
                _rotationY = value.Y;
                _rotationZ = value.Z;
            }
        }

        public float Size { get { return _size; } set { _size = value; UpdateWorldMatrix(); } }

        #endregion

        public GameModel(Model model, Vector3 position)
        {
            Model = model;
            Position = position;
            Size = 1;
            Rotation = Vector3.Zero;
            UpdateWorldMatrix();
        }

        #region Transformations

        public void UpdateWorldMatrix()
        {
            WorldMatrix = Matrix.CreateFromYawPitchRoll(_rotationX, _rotationY, _rotationZ) *
                          Matrix.CreateScale(Size) *
                          Matrix.CreateTranslation(_positionX, _positionY, _positionZ);
        }

        public void Translate(Vector3 position)
        {
            WorldMatrix *= Matrix.CreateTranslation(position);
        }

        public void Move2D(int x, int y)
        {
            _positionX += x;
            _positionY += y;
            UpdateWorldMatrix();
        }

        public void Scale(float scale)
        {
            WorldMatrix *= Matrix.CreateScale(scale);
        }

        public void RotateX(float degree)
        {
            _rotationX += MathHelper.ToRadians(degree);
            UpdateWorldMatrix();
        }

        public void RotateY(float degree)
        {
            _rotationY += MathHelper.ToRadians(degree);
            UpdateWorldMatrix();
        }

        public void RotateZ(float degree)
        {
            _rotationZ += MathHelper.ToRadians(degree);
            UpdateWorldMatrix();
        }

        #endregion

        public virtual void Draw()
        {
            //var modelTransforms = new Matrix[Model.Bones.Count];
            //Model.CopyAbsoluteBoneTransformsTo(modelTransforms);

            //foreach (ModelMesh mesh in Model.Meshes)
            //{
            //    foreach (BasicEffect effect in mesh.Effects)
            //    {
            //        effect.EnableDefaultLighting();
            //        effect.World = modelTransforms[mesh.ParentBone.Index] * WorldMatrix;
            //        effect.Projection = EngineGlobals.Camera3D.Projection;
            //        effect.View = EngineGlobals.Camera3D.View;
            //    }

            //    mesh.Draw();
            //}
            //Model.Draw(WorldMatrix, EngineGlobals.Camera3D.View, EngineGlobals.Camera3D.Projection);
        }
    }
}
