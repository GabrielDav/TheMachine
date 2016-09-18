using Engine.Core;
using Engine.Mechanics;
using GameLibrary.Gui.ScreenManagement.NewScreens;
using Microsoft.Xna.Framework;
using TheGoo;

namespace GameLibrary.Objects
{
    public class ArcadePowerUp : PowerUp
    {
        public ArcadePowerUp()
        {
            _ignoreInkCollect = true;
        }

        public override void Merge()
        {
            Mask.IsHidden = true;
            IsActivated = false;
            Follow = false;
            if (GameGlobals.Player.CurrentPowerUp != PowerUpType.None)
            {
                if (GameGlobals.Player.CurrentPowerUp == PowerUpType.TimeWarp)
                {
                    GameGlobals.Player.StopTimeWarpPowerup();
                }
            }
            GameGlobals.Player.CurrentPowerUp = PowerUpType;
            if (PowerUpType == PowerUpType.TimeWarp)
            {
                GameGlobals.Player.StartTimeWarp();
                ((ArcadeScreen)EngineGlobals.ScreenManager.CurrentScreen).SetArcadePowerup(PowerUpType);
            }
            else if (PowerUpType == PowerUpType.InkCollect)
            {
                ((ArcadeScreen)EngineGlobals.ScreenManager.CurrentScreen).SetArcadePowerup(PowerUpType);
            }
        }

        public override void Update()
        {
            base.Update();
        }

        public override void StartFollow()
        {
            base.StartFollow();
        }
    }
}
