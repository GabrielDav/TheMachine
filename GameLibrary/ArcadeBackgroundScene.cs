using System;
using System.Collections.Generic;
using System.Linq;
using Engine.Core;
using GameLibrary.Objects;
using Microsoft.Xna.Framework;
using TheGoo;

namespace GameLibrary
{
    public class ArcadeBackgroundScene
    {
        public string[] Objects;
        public Vector2 Offset;

        public void Init()
        {
            foreach (var backgroundObject in GameGlobals.Map.BackgroundObjects)
            {
                if (backgroundObject.Name.Contains("Scene_"))
                {
                    var found = false;
                    foreach (var obj in Objects)
                    {
                        if (backgroundObject.Name == obj)
                        {
                            backgroundObject.Pos = (backgroundObject.Pos.ToVector() + Offset).ToPoint();
                            found = true;
                            break;
                        }
                    }
                    if (!found)
                        Controller.RemoveBackgroundObject(backgroundObject);
                    
                }
            }
        }

    }
}
