using System;
using System.Collections.Generic;
using Engine.Core;
using Engine.Graphics;
using Microsoft.Xna.Framework;

namespace Engine.Mechanics
{
    public enum CollidingObjectType { Rectangle, Circle }

    public enum CollisionResponce { GoTrought, Stop, Hit}

    public enum FixPosition
    {
        TopLeft,
        Top,
        TopRight,
        Right,
        BottomRight,
        Bottom,
        BottomLeft,
        Left
    }

    public delegate void CollisionEvent(CollisionResult collision);

    public static class VectorExt
    {
        public static Vector2 GetNormal(this Vector2 vector)
        {
            return (new Vector2(vector.Y * -1, vector.X));
        }

        public static Vector2 Projection(this Vector2 vector, Vector2 target)
        {
            var loc = Vector2.Dot(target, target);
            if (Math.Abs(loc - 0) < EngineGlobals.Epsilon)
            {
                return vector;
            }
            var dot = Vector2.Dot(vector, target);
            return target * (dot / loc);
        }
    }

    public static class MathExt
    {
        public static float Sqrt2 = (float)Math.Sqrt(2);

        public static float NormalizeRotation(float rotation)
        {
            rotation = rotation%MathHelper.TwoPi;
            if (rotation < 0)
                rotation += MathHelper.TwoPi;
            return rotation;
        }
    }

    public class CollisionResult
    {
        public Vector2 CollisionPos;
        public PhysicalObject Source;
        public PhysicalObject Target;
        public Vector2 NewPos;
        public Vector2 Penetration;
        public int VerticalAxis;
        public int HorizontalAxis;
        public bool DiagonalCollision;
        public FixPosition FixPosition;
    }

    public class PhysicsManager : IDisposable
    {
        protected List<PhysicalObject> _physicalObjects;
        protected readonly List<PhysicalObject> _movingObjects;
        protected readonly List<PhysicalObject> _objectsQueue;

        public PhysicsManager()
        {
            _movingObjects = new List<PhysicalObject>();
            _objectsQueue = new List<PhysicalObject>();
            _physicalObjects = new List<PhysicalObject>();
        }

        public void LoadMapObjects(List<PhysicalObject> collidingObjects)
        {
            foreach (var collidingObject in collidingObjects)
            {
                if (!collidingObject.IgnoresPhysics)
                    _physicalObjects.Add(collidingObject);
            }
        }

        public List<CollisionResult> CheckMove(PhysicalObject gameObject, Vector2 newPos)
        {
            var collisions = new List<CollisionResult>();
            var source = gameObject;
            var oldPos = gameObject.HalfPos;
            gameObject.HalfPos = newPos;
            switch (source.CollidingType)
            {
                case CollidingObjectType.Rectangle:
                    throw new Exception("Unsupported exception check type(Rectangle)");
                case CollidingObjectType.Circle:
                    foreach (var collidingObject in _physicalObjects)
                    {
                        if (collidingObject.IgnoreCollision)
                        {
                            continue;
                        }
                        if (collidingObject != source)
                        {
                            var result = CheckBaseCircleCollision(source, collidingObject, newPos - oldPos);
                            if (result != null)
                            {
                                collisions.Add(result);
                            }
                        }
                    }
                    break;
            }
            gameObject.HalfPos = oldPos;
            return collisions;
        }

        public CollisionResult CheckBaseCircleCollision(PhysicalObject source, PhysicalObject target, Vector2 moveVector)
        {
            var sourceRect = source.Rectangle.GetRectangle();
            if (sourceRect.Width != sourceRect.Height)
            {
                throw new Exception("Its not a circle!");
            }
            var sourceR = sourceRect.Width / 2;
            var sourceCenterX = sourceRect.X + sourceR;

            var targetRect = target.Rectangle.GetRectangle();
            var targetHalfWidth = targetRect.Width / 2;
            var targetCenterX = targetRect.X + targetHalfWidth;
            
            var dx = sourceCenterX - targetCenterX; // object -> circle delta x
            var px = (targetHalfWidth + sourceR) - Math.Abs(dx); // penetration depth in x

            if (0 < px)
            {
                
                var sourceCenterY = sourceRect.Y + sourceR;
                var targetHalfHeight = targetRect.Height / 2;
                var targetCenterY = targetRect.Y + targetHalfHeight;

                var dy = sourceCenterY - targetCenterY; // object -> circle delta y
                var py = (targetHalfHeight + sourceR) - Math.Abs(dy); // penetration depth y

                if (0 < py)
                {
                    // at this point circle may be colliding with object
                    // determinate vornoi region of the object
                    var oH = 0;
                    var oV = 0;
                    if (dx < -target.HalfSize.X)
                    {
                        oH = -1; // circle is on the left side of object
                    }
                    else if (target.HalfSize.X < dx)
                    {
                        oH = 1; // circle is on the right side of object
                    }

                    if (dy < -target.HalfSize.Y)
                    {
                        oV = -1; // circle is on the top side of object
                    }
                    else if (target.HalfSize.Y < dy)
                    {
                        oV = 1; // circle is on the bottom side of object
                    }
                    switch (target.CollidingType)
                    {
                        case CollidingObjectType.Rectangle:
                            return CollideCircleVsRect(
                                px,
                                py,
                                oH,
                                oV,
                                source,
                                target,
                                sourceRect,
                                targetRect,
                                sourceR,
                                new Point(sourceCenterX, sourceCenterY),
                                moveVector);

                        case CollidingObjectType.Circle:
                            return CollideCircleVsCircle(px, py, oH, oV, source, target);
                    }
                }
            }

            return null;
        }

        public void CheckCollisions(PhysicalObject player)
        {
            
        }

        protected CollisionResult CorrnerCollision(
            PhysicalObject source,
            PhysicalObject target,
            Point sourceCenter,
            Vector2 moveVector)
        {
            var startPos = (sourceCenter.ToVector() - moveVector).ToPoint();
            var targetRect = target.Rectangle.GetRectangle();

            var result = new CollisionResult
                {
                    Source = source,
                    Target = target
                };

            var path = new Ray2D(startPos, sourceCenter);
            var targetIntersection = path.Intersects(target.Rectangle.GetRectangle());

            if (targetIntersection.Count == 1)
            {
                result.NewPos = targetIntersection[0].ToVector();
            }

            var vertical = 0;
            var horizontal = 0;
            if (Math.Abs(result.NewPos.X - targetRect.X) < EngineGlobals.Epsilon)
            {
                vertical = -1;
                result.FixPosition = FixPosition.Left;
            }
            else if (Math.Abs(result.NewPos.X - (targetRect.X + targetRect.Width)) < EngineGlobals.Epsilon)
            {
                vertical = 1;
                result.FixPosition = FixPosition.Right;
            }

            if (Math.Abs(result.NewPos.Y - targetRect.Y) < EngineGlobals.Epsilon)
            {
                horizontal = 1;
                result.FixPosition = FixPosition.Top;

            }
            else if (Math.Abs(result.NewPos.Y - (targetRect.Y + targetRect.Height)) < EngineGlobals.Epsilon)
            {
                horizontal = -1;
                result.FixPosition = FixPosition.Bottom;
            }

            result.VerticalAxis = vertical;
            result.HorizontalAxis = horizontal;

            return result;

            ////WTF?
            //var v4 = targetIntersection[0].ToVector() - startPos.ToVector();
            //path = new Ray2D(startPos, (startPos.ToVector() + v4).ToPoint());
            //var innerIntersection =
            //    path.Intersects(new Rectangle(startPos.X - sourceRect.Width,
            //                                  startPos.Y - sourceRect.Height,
            //                                  sourceRect.Width,
            //                                  sourceRect.Height));
            //var innerSigX = startPos.X > innerIntersection[0].X ? -1 : 1;
            //var innerSigY = startPos.Y > innerIntersection[0].Y ? -1 : 1;
            //var v2 = (startPos.ToVector() - innerIntersection[0].ToVector());
            //var v3 = new Vector2(v2.X * innerSigX, v2.Y * innerSigY);
            //var res = startPos.ToVector() + v4 - v3;
        }

        protected int AdjustHorizontal(int y, int sourceR, Rectangle targetRect)
        {
            EngineGlobals.Debug.PlayerY = y - sourceR;
            EngineGlobals.Debug.TargetY = targetRect.Y;

            if (y - sourceR < targetRect.Y)
            {
                EngineGlobals.Debug.FixedY = targetRect.Y + sourceR;
                return (targetRect.Y + sourceR);
            }

            if (y + sourceR > targetRect.Y + targetRect.Height)
            {
                EngineGlobals.Debug.FixedY = 666;
                return (targetRect.Y + targetRect.Height - sourceR);
            }

            return y;
        }

        protected int AdjustVerticaly(int x, int sourceR, Rectangle targetRect)
        {
            if (x - sourceR < targetRect.X)
            {
                return (targetRect.X + sourceR);
            }

            if (x + sourceR > targetRect.X + targetRect.Width)
            {
                return (targetRect.X + targetRect.Width - sourceR);
            }

            return x;
        }

        protected CollisionResult CollideCircleVsRect(
            float x,
            float y,
            int oH,
            int oV,
            PhysicalObject source,
            PhysicalObject target,
            Rectangle sourceRect,
            Rectangle targetRect,
            int sourceR,
            Point sourceCenter,
            Vector2 moveVector)
        {
            EngineGlobals.Debug.FixedY = -1;
            if (oH == 0)
            {
                if (oV == 0)
                {
                    return CorrnerCollision(source, target, sourceCenter, moveVector);
                }

                // Collision with vertical axis
                var result = new CollisionResult
                                 {
                                     Source = source,
                                     Target = target,
                                     Penetration = new Vector2(0, y*-oV),
                                     NewPos = new Vector2(
                                         AdjustVerticaly(sourceCenter.X, sourceR, targetRect),
                                         sourceCenter.Y + y*oV),
                                     VerticalAxis = oV,
                                     HorizontalAxis = oH,
                                     FixPosition = oV == 1 ? FixPosition.Bottom : FixPosition.Top
                                 };

                return result;
            }
            if (oV == 0)
            {
                // Collision with horizontal axis
                EngineGlobals.Debug.TargetName = target.Name;
                var result = new CollisionResult
                                 {
                                     Source = source,
                                     Target = target,
                                     Penetration = new Vector2(x*-oH, 0),
                                     NewPos = new Vector2(
                                         sourceCenter.X + x*oH,
                                         AdjustHorizontal(sourceCenter.Y, sourceR, targetRect)),
                                     VerticalAxis = oV,
                                     HorizontalAxis = oH,
                                     FixPosition = oH == 1 ? FixPosition.Right : FixPosition.Left
                                 };

                return result;
            }

            return DiagonalCollision(target, source, sourceCenter, oH, oV);
        }

        protected Vector2 CorrnerPos()
        {
            return new Vector2();
        }

        /// <summary>
        /// Checks if collision occured with either
        /// vertical or horizontal point
        /// </summary>
        /// <returns>
        /// 1 if vertical collision
        /// -1 if horizontal collision
        /// 0 if no collisions
        /// </returns>
        protected int CollidesWithSidePoint(
            Point pointVerical,
            Point pointHorizontal,
            PhysicalObject source,
            PhysicalObject target)
        {
            foreach (var physicalObject in _physicalObjects)
            {
                if ((physicalObject != source) && (physicalObject != target))
                {
                    var rect = physicalObject.Rectangle.GetRectangle();
                    if (rect.Contains(pointVerical))
                    {
                        return 1;
                    }

                    if (rect.Contains(pointHorizontal))
                    {
                        return -1;
                    }
                }
            }

            return 0;
        }

        protected CollisionResult DiagonalCollision(PhysicalObject target,
            PhysicalObject source,
            Point sourceCenter,
            int oH,
            int oV)
        {
            var targetRect = target.Rectangle.GetRectangle();
            var sourceR = source.Rectangle.GetRectangle().Width/2;

            var result = new CollisionResult
                {
                    Target = target,
                    Source = source
                };

            if (oV == -1)
            {
                //top left |-
                if (oH == -1)
                {
                    result.NewPos = new Vector2(targetRect.X, targetRect.Y);

                    var pointV = new Point(targetRect.X - sourceR, targetRect.Y);
                    var pointH = new Point(targetRect.X, targetRect.Y - sourceR);
                    
                    // patikrinam ar is kaires arba is virsaus kartais nera objektui
                    // jei yra, kolizija nera diagonali, negalima leisti sokti i objekta
                    var sideCollisions = CollidesWithSidePoint(pointV, pointH, source, target);
                    if (sideCollisions == 1)
                    {
                        result.FixPosition = FixPosition.Top;
                    }
                    else if(sideCollisions == -1)
                    {
                        result.FixPosition = FixPosition.Left;
                    }
                    else
                    {
                        result.FixPosition = FixPosition.TopLeft;
                        result.DiagonalCollision = true;
                    }
                }
                //top right -|
                else
                {
                    result.NewPos = new Vector2(
                        target.Rectangle.X + targetRect.Width,
                        targetRect.Y);

                    var pointV = new Point(
                        targetRect.X + targetRect.Width + sourceR,
                        targetRect.Y);
                    var pointH = new Point(
                        targetRect.X + targetRect.Width,
                        targetRect.Y - sourceR);

                    // patikrinam ar is desnes arba is virsaus kartais nera objektui
                    // jei yra, kolizija nera diagonali, negalima leisti sokti i objekta
                    var sideCollisions = CollidesWithSidePoint(pointV, pointH, source, target);
                    if (sideCollisions == 1)
                    {
                        result.FixPosition = FixPosition.Top;
                    }
                    else if (sideCollisions == -1)
                    {
                        result.FixPosition = FixPosition.Right;
                    }
                    else
                    {
                        result.FixPosition = FixPosition.TopRight;
                        result.DiagonalCollision = true;
                    }
                }
            }
            else
            {
                //botom right _|
                if (oH == 1)
                {
                    result.NewPos = new Vector2(
                        target.Rectangle.X + targetRect.Width,
                        targetRect.Y + targetRect.Height);

                    var pointV = new Point(
                        targetRect.X + targetRect.Width + sourceR,
                        targetRect.Y + targetRect.Height);
                    var pointH = new Point(
                        targetRect.X + targetRect.Width,
                        targetRect.Y + targetRect.Height + sourceR);

                    // patikrinam ar is desnes arba is apacios kartais nera objektui
                    // jei yra, kolizija nera diagonali, negalima leisti sokti i objekta
                    var sideCollisions = CollidesWithSidePoint(pointV, pointH, source, target);
                    if (sideCollisions == 1)
                    {
                        result.FixPosition = FixPosition.Bottom;
                    }
                    else if (sideCollisions == -1)
                    {
                        result.FixPosition = FixPosition.Right;
                    }
                    else
                    {
                        result.FixPosition = FixPosition.BottomRight;
                        result.DiagonalCollision = true;
                    }
                }
                //bottom left |_
                else
                {
                        result.NewPos = new Vector2(
                            target.Rectangle.X,
                            targetRect.Y + targetRect.Height);

                        var pointV = new Point(
                            targetRect.X - sourceR,
                            targetRect.Y + targetRect.Height);
                        var pointH = new Point(
                            targetRect.X,
                            targetRect.Y + targetRect.Height + sourceR);

                        // patikrinam ar is kaires arba is apacios kartais nera objektui
                        // jei yra, kolizija nera diagonali, negalima leisti sokti i objekta
                        var sideCollisions = CollidesWithSidePoint(pointV, pointH, source, target);
                        if (sideCollisions == 1)
                        {
                            result.FixPosition = FixPosition.Bottom;
                        }
                        else if (sideCollisions == -1)
                        {
                            result.FixPosition = FixPosition.Left;
                        }
                        else
                        {
                            result.FixPosition = FixPosition.BottomLeft;
                            result.DiagonalCollision = true;
                        }
                }
            }

            return result;
        }

        protected CollisionResult CollideCircleVsCircle(
            float x,
            float y,
            int oH,
            int oV,
            PhysicalObject source,
            PhysicalObject target)
        {
            var v = source.HalfPos - target.HalfPos;
            var len = v.Length();
            var pen = (target.HalfSize.X + source.HalfSize.X) - len;

            if (0 < pen)
            {
                var v2 = new Vector2(v.X/len, v.Y/len);
                var result = new CollisionResult
                    {
                        Source = source,
                        Target = target,
                        Penetration = new Vector2(v2.X*-pen, v2.Y*-pen),
                        NewPos = new Vector2(
                            source.HalfPos.X + v2.X*pen,
                            source.HalfPos.Y + v2.Y*pen),
                        VerticalAxis = oV,
                        HorizontalAxis = oH
                    };
                var v3 = v;
                v3.Normalize();
                result.CollisionPos = target.HalfPos + v3*target.HalfSize.X;
                var v4 = v.GetNormal();
                v4.Normalize();
                //result.NormalVectorA = result.CollisionPos - v4*source.HalfSize.X;
                //result.NormalVectorB = result.CollisionPos + v4*source.HalfSize.X;
                return result;
            }
            return null;
        }

        public void PushObject(PhysicalObject physicalObject, float startSpeed, float angle)
        {
            var x = -(float)Math.Round(Math.Cos(MathHelper.ToRadians(angle)), 4);
            var y = (float)Math.Round(Math.Sin(MathHelper.ToRadians(angle)), 4);
            var direction = new Vector2(x * -(startSpeed), y * -(startSpeed));
            PushObject(physicalObject, direction);
        }

        public void PushObject(PhysicalObject physicalObject, Vector2 destination, float startSpeed)
        {
            var direction = Vector2.Normalize(destination - physicalObject.HalfPos);
            direction *= startSpeed;
            PushObject(physicalObject, direction);
        }

        public void PushObject(PhysicalObject physicalObject, Vector2 direction)
        {
            physicalObject.Direction = direction;
            if (!_movingObjects.Contains(physicalObject))
            {
                _movingObjects.Add(physicalObject);
            }
        }

        public void AddObjectToQueue(PhysicalObject obj)
        {
            _objectsQueue.Add(obj);
        }

        public void CommitQueue()
        {
            foreach (var physicalObject in _objectsQueue)
            {
                _physicalObjects.Add(physicalObject);
            }
            _objectsQueue.Clear();
        }

        public void Update()
        {
            List<PhysicalObject> stoped = null;
            foreach (var physicalObject in _movingObjects)
            {
                var gravity = physicalObject.IgnoreGravity ? Vector2.Zero : EngineGlobals.Gravity;
                var direction = physicalObject.Direction;
                if (physicalObject.IgnoreInGameTime)
                {
                    gravity *= (float)EngineGlobals.GameTime.ElapsedGameTime.TotalMilliseconds / 1000;
                    direction *= (float) EngineGlobals.GameTime.ElapsedGameTime.TotalMilliseconds / 1000;
                }
                else
                {
                    gravity *= EngineGlobals.GetElapsedInGameTime() / 1000;
                    direction *= EngineGlobals.GetElapsedInGameTime() / 1000;
                }
                direction *= EngineGlobals.PixelsPerMeter;
                direction += gravity;
                if (direction.X > EngineGlobals.MaxSpeed)
                {
                    direction.X = EngineGlobals.MaxSpeed;
                }
                else if (direction.X < -EngineGlobals.MaxSpeed)
                {
                    direction.X = -EngineGlobals.MaxSpeed;
                }

                if (direction.Y > EngineGlobals.MaxSpeed)
                {
                    direction.Y = EngineGlobals.MaxSpeed;
                }
                else if (direction.Y < -EngineGlobals.MaxSpeed)
                {
                    direction.Y = -EngineGlobals.MaxSpeed;
                }

                var collisions = CheckMove(physicalObject, physicalObject.HalfPos + (direction));
                if (collisions.Count < 1)
                {
                    physicalObject.Direction += gravity;
                    physicalObject.Move(direction);
                }
                else
                {
                    var response = physicalObject.Collide(collisions[0]);
                    switch (response)
                    {
                        case CollisionResponce.Stop:
                            physicalObject.Direction = Vector2.Zero;
                            if (stoped == null)
                            {
                                stoped = new List<PhysicalObject>();
                            }
                            stoped.Add(physicalObject);
                            break;
                        case CollisionResponce.GoTrought:
                            physicalObject.Direction += gravity;
                            physicalObject.Move(direction);
                            break;
                        case CollisionResponce.Hit:
                            physicalObject.Direction = Vector2.Zero;
                            break;
                    }
                }
            }
            if (stoped != null)
            {
                foreach (var physicalObject in stoped)
                {
                    _movingObjects.Remove(physicalObject);
                }
            }
        }

        public void Dispose()
        {
            _physicalObjects.Clear();
            _movingObjects.Clear();
            _objectsQueue.Clear();
        }
    }
}
