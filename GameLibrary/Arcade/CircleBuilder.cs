using Microsoft.Xna.Framework;
using System;
using TheGoo;
using C = GameLibrary.Arcade.CircleBuilderConsts;
using B = GameLibrary.Arcade.Consts;

namespace GameLibrary.Arcade
{
    public class CircleBuilder
    {
        public Rectangle PrevHighestCircle;
        public Rectangle CurrentMainCircleRect;
        private readonly Rectangle[] _circleRects;
        private int _circleCount;
        private readonly ObjectPoolManager _manager;

        public CircleBuilder(ObjectPoolManager manager)
        {
            _manager = manager;
            _circleRects = new Rectangle[3];
        }

        private int CalcDeathBallDelay(int level)
        {
            var result = GameGlobals.Random.Next(
                C.CircleLevelConst[level].MinimumDeathBallDelay,
                C.CircleLevelConst[level].MaximumDeathBallDelay);

            result = result*1000;

            return result;
        }

        public Rectangle PlaceMainCircle(int level, int height)
        {
            var r = GameGlobals.Random.Next(C.MinimumBallR, C.MaximumBallR);

            var verticalDistance = GameGlobals.Random.Next(
                C.CircleLevelConst[level].MinimumVerticalDistance,
                C.CircleLevelConst[level].MaximumVerticalDistance);

            var y = PrevHighestCircle.Y - verticalDistance - r;

            var x = GameGlobals.Random.Next(
                C.MinDistanceLeft + r,
                C.MinDistanceRight - r);

            var speed = GameGlobals.Random.Next(
                C.MinimalSpeed + height / C.MinCircleSpeedDivider,
                C.MaximumSpeed + height / C.MaxCircleSpeedDivider);

            Rectangle rect;
            if ((level > 2) && Random.RandomProc(C.CircleLevelConst[level].ChanceMainBallIsDeathBall) &&
                r <= C.MaxRToBeDeathBall)
            {
                rect = _manager.CreateOrGetDeathBall(r, x, y, speed, CalcDeathBallDelay(level));
            }
            else
            {
                rect = _manager.CreateOrGetCircle(r, x, y, speed, 1);
            }

            _circleRects[_circleCount] = rect;
            _circleCount++;

            return rect;
        }

        private void PlaceSetOfInkDotsAroundCircle(int level, int circleX, int circleY, int r)
        {
            var minInkDots = level >= 3 ? 1 : 0;
            var inkDots = GameGlobals.Random.Next(minInkDots, 4);
            var tenthOfR = r/10;

            if (inkDots > 0)
            {
                if (inkDots == 1)
                {
                    _manager.CreateOrGetInkDot(
                        circleX + Random.RandomSign()*(r + B.HalfDistanceBetweenCircles),
                        circleY + GameGlobals.Random.Next(-r, r), Random.RandomTrueFalse());
                }
                else if (inkDots == 2)
                {
                    _manager.CreateOrGetInkDot(
                        circleX + Random.RandomSign() * (r + B.HalfDistanceBetweenCircles),
                        circleY - (GameGlobals.Random.Next(tenthOfR, r - tenthOfR)), false);

                    _manager.CreateOrGetInkDot(
                        circleX + Random.RandomSign() * (r + B.HalfDistanceBetweenCircles),
                        circleY + (GameGlobals.Random.Next(tenthOfR, r - tenthOfR)), false);

                }
                else if (inkDots == 3)
                {
                    _manager.CreateOrGetInkDot(
                        circleX + Random.RandomSign() * (r + B.HalfDistanceBetweenCircles),
                        circleY - (r - GameGlobals.Random.Next(0, tenthOfR)), false);

                    _manager.CreateOrGetInkDot(
                        circleX + Random.RandomSign() * (r + B.HalfDistanceBetweenCircles),
                        circleY,
                        Random.RandomProc(25));

                    _manager.CreateOrGetInkDot(
                        circleX + Random.RandomSign() * (r + B.HalfDistanceBetweenCircles),
                        circleY + (r - GameGlobals.Random.Next(0, tenthOfR)), false);
                    
                }
            }
        }

        /// <summary>
        /// checks if secondary ball fits in the
        /// left side of the screen and if
        /// it does places it left
        /// </summary>
        private bool PlaceSecondaryCircle(Rectangle mainCircleRect, bool left, int level, int height)
        {
            var width =
                left
                    ? ((mainCircleRect.X - C.MinDistanceLeft - C.MinimalDistanceBetweenCircles) / 2)
                    : (C.MinDistanceRight -
                       mainCircleRect.X -
                       mainCircleRect.Width -
                       C.MinimalDistanceBetweenCircles) / 2;

            //gali tilpti maziausias skritulys
            if (width > C.MinimumBallR)
            {
                int r = GameGlobals.Random.Next(
                    C.MinimumBallR,
                    width >= C.MaximumSecondaryBallR ? C.MaximumSecondaryBallR : width);

                int x = left
                            ? C.MinDistanceLeft + r + GameGlobals.Random.Next(0, width - r)
                            : C.MinDistanceRight - r - GameGlobals.Random.Next(0, width - r);

                int y = GameGlobals.Random.Next(
                    mainCircleRect.Y,
                    mainCircleRect.Y + (mainCircleRect.Width/2));

                var speed = GameGlobals.Random.Next(
                    C.MinimalSpeed + height/C.MinCircleSpeedDivider,
                    C.MaximumSpeed + height/C.MaxCircleSpeedDivider);

                Rectangle rect;
                if (level > 1 && Random.RandomProc(C.CircleLevelConst[level].ChanceSecondaryBallIsDeathBall))
                {
                    rect = _manager.CreateOrGetDeathBall(r, x, y, speed, CalcDeathBallDelay(level));
                }
                else
                {
                    rect = _manager.CreateOrGetCircle(r, x, y, speed, 2);
                }

                _circleRects[_circleCount] = rect;
                _circleCount++;
                PlaceSetOfInkDotsAroundCircle(
                    level,
                    rect.Center.X,
                    rect.Center.Y,
                    rect.Width/2);
                return true;
            }

            return false;
        }
        
        ///<summary>
        /// if distance between balls are too large
        /// make minimal ball between them 
        /// </summary>
        private void PlaceMiddleBall(int level, Rectangle highestCircle, Rectangle lowestCircle)
        {
            var ball1R = highestCircle.Width / 2;
            var ball1CenterX = highestCircle.X + ball1R;
            var ball1CenterY = highestCircle.Y + ball1R;

            var ball2R = lowestCircle.Width / 2;
            var ball2CenterX = lowestCircle.X + ball2R;
            var ball2CenterY = lowestCircle.Y + ball2R;

            var distance = Math.Sqrt((Math.Pow((ball1CenterX - ball2CenterX), 2) +
                                      Math.Pow((ball1CenterY - ball2CenterY), 2)))
                           - ball1R
                           - ball2R;

            var x = (ball1CenterX + ball2CenterX) / 2;
            var y = (ball1CenterY + ball2CenterY) / 2;

            if (distance >= 275 && level >= 2)
            {
                _manager.CreateOrGetPowerUp(x, y, GameGlobals.Random.Next(1, 3));
            }
            else
            {
                _manager.CreateOrGetInkDot(x, y, ((level >= 3) || Random.RandomTrueFalse()));
            }
        }

        public void PlaceCircles(int level, int height)
        {
            CurrentMainCircleRect = PlaceMainCircle(level, height);

            PlaceSetOfInkDotsAroundCircle(
                level,
                CurrentMainCircleRect.Center.X,
                CurrentMainCircleRect.Center.Y,
                CurrentMainCircleRect.Width/2);

            var random = GameGlobals.Random.Next(0, 100);

            int circles = 0;
            if (random < 40)
            {
                circles = 1;
            }
            else if (random < 80)
            {
                circles = 2;
            }

            Rectangle highestCircle = _circleRects[0];
            Rectangle lowestCircle = _circleRects[0];

            if (circles > 0)
            {
                if (circles == 1)
                {
                    var left = Random.RandomTrueFalse();
                    var placed = PlaceSecondaryCircle(CurrentMainCircleRect, left, level, height);
                    //jei nepadejo i random puse dar bando i kita puse padeti
                    if (!placed)
                    {
                        PlaceSecondaryCircle(CurrentMainCircleRect, !left, level, height);
                    }
                }
                else
                {
                    PlaceSecondaryCircle(CurrentMainCircleRect, true, level, height);
                    PlaceSecondaryCircle(CurrentMainCircleRect, false, level, height);
                }


                for (int i = 1; i < _circleCount; i++)
                {
                    if ((lowestCircle.Y + lowestCircle.Width) <
                        (_circleRects[i].Y - _circleRects[i].Width))
                    {
                        lowestCircle = _circleRects[i];
                    }
                }

                for (int i = 1; i < _circleCount; i++)
                {
                    if ((highestCircle.Y - highestCircle.Width) >
                        (_circleRects[i].Y - _circleRects[i].Width))
                    {
                        highestCircle = _circleRects[i];
                    }
                }
            }

            PlaceMiddleBall(level, PrevHighestCircle, lowestCircle);

            PrevHighestCircle = highestCircle;
            _circleCount = 0;
        }
    }
}
