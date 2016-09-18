using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace Engine.Graphics
{
    public enum LineIntersection
    {
        Left,
        Right,
        Top,
        Bottom
    }

    public class Ray2D
    {

        public Point StartPos { get; protected set; }
        public Point EndPos { get; protected set; }

        public Ray2D(Point start, Point end)
        {
            StartPos = start;
            EndPos = end;
        }

        protected bool SameSigns(int a, int b)
        {
            if (a < 0 && b < 0)
                return true;
            if (a > 0 && b > 0)
                return true;
            return false;
        }

        protected Point? LinesIntersect(int x1, int y1, int x2, int y2)
        {

            var a1 = EndPos.Y - StartPos.Y;
            var b1 = StartPos.X - EndPos.X;
            var c1 = EndPos.X*StartPos.Y - StartPos.X*EndPos.Y;


            var r3 = a1*x1 + b1*y1 + c1;
            var r4 = a1*x2 + b1*y2 + c1;

            if (r3 != 0 &&
                r4 != 0 &&
                SameSigns(r3, r4))
                return null;

            /* Compute a2, b2, c2 */

            var a2 = y2 - y1;
            var b2 = x1 - x2;
            var c2 = x2*y1 - x1*y2;

            /* Compute r1 and r2 */

            var r1 = a2*StartPos.X + b2*StartPos.Y + c2;
            var r2 = a2*EndPos.X + b2*EndPos.Y + c2;

            /* Check signs of r1 and r2.  If both point 1 and point 2 lie
             * on same side of second line segment, the line segments do
             * not intersect.
             */

            if (r1 != 0 &&
                r2 != 0 &&
                SameSigns(r1, r2))
                return null;

            /* Line segments intersect: compute intersection point. 
             */

            var denom = a1*b2 - a2*b1;
            if (denom == 0)
                return null;
            int offset = denom < 0 ? -denom/2 : denom/2;

            /* The denom/2 is to get rounding instead of truncating.  It
             * is added or subtracted to the numerator, depending upon the
             * sign of the numerator.
             */

            var num = b1*c2 - b2*c1;
            var x = (num < 0 ? num - offset : num + offset)/denom;

            num = a2*c1 - a1*c2;
            var y = (num < 0 ? num - offset : num + offset)/denom;

            return new Point(x, y);
        }

        public Point? Intersects(Rectangle rectangle, LineIntersection intersection)
        {
            if (intersection == LineIntersection.Left)
                return LinesIntersect(rectangle.X, rectangle.Y, rectangle.X, rectangle.Y + rectangle.Height);
            if (intersection == LineIntersection.Right)
                return LinesIntersect(rectangle.X + rectangle.Width, rectangle.Y, rectangle.X + rectangle.Width,
                                      rectangle.Y + rectangle.Height);
            if (intersection == LineIntersection.Top)
                return LinesIntersect(rectangle.X, rectangle.Y, rectangle.X + rectangle.Width, rectangle.Y);
            if (intersection == LineIntersection.Bottom)
                return LinesIntersect(rectangle.X, rectangle.Y + rectangle.Height, rectangle.X + rectangle.Width,
                                      rectangle.Y + rectangle.Height);
            return null;
        }

        public List<Point> Intersects(Rectangle rectangle)
        {
            var result = new List<Point>();
            var i = Intersects(rectangle, LineIntersection.Top);
            if (i.HasValue)
                result.Add(i.Value);
            i = Intersects(rectangle, LineIntersection.Right);
            if (i.HasValue)
                result.Add(i.Value);
            i = Intersects(rectangle, LineIntersection.Bottom);
            if (i.HasValue)
                result.Add(i.Value);
            i = Intersects(rectangle, LineIntersection.Left);
            if (i.HasValue)
                result.Add(i.Value);
            return result;
        }

        public Point? IntersectsCircle(Rectangle circle)
        {
            Point? point1 = null;
            Point? point2 = null;

            var r = circle.Width/2;
            var startPos = new Vector2(StartPos.X, StartPos.Y);
            var endPos = new Vector2(EndPos.X, EndPos.Y);

            var center = new Vector2(circle.X + r, circle.Y + r);
            var direction = startPos - endPos;
            var f = startPos - center;

            float a = Vector2.Dot(direction, direction);
            float b = 2*Vector2.Dot(f, direction);
            float c = Vector2.Dot(f, f) - r*r;

            float discriminant = b*b - 4*a*c;
            if (discriminant < 0)
            {
                return null;
            }

            // ray didn't totally miss sphere,
            // so there is a solution to
            // the equation.
            discriminant = (float) Math.Sqrt(discriminant);
            float t1 = (-b + discriminant)/(2*a);
            float t2 = (-b - discriminant)/(2*a);

            if (t1 >= 0 && t1 <= 1)
            {
                // solution on is ON THE RAY.
                point1 = new Point((int) (startPos.X + t1*direction.X), (int) (startPos.Y + t1*direction.Y));
            }

            if (t2 >= 0 && t1 <= 2)
            {
                // solution on is ON THE RAY.
                point2 = new Point((int) (startPos.X + t2*direction.X), (int) (startPos.Y + t2*direction.Y));
            }

            if(point1.HasValue && !point2.HasValue)
            {
                return point1;
            }
            if(!point1.HasValue && point2.HasValue)
            {
                return point2;
            }
            if(point1.HasValue)
            {
                var distance1 = Vector2.Distance(startPos, new Vector2(point1.Value.X, point1.Value.Y));
                var distance2 = Vector2.Distance(startPos, new Vector2(point2.Value.X, point2.Value.Y));

                if (distance1 > distance2)
                {
                    return point2;
                }

                return point1;
            }

            return null;
        }

    }
}
