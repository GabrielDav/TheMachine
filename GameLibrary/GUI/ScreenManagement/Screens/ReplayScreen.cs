using TheGoo;

namespace GameLibrary.Gui.ScreenManagement.Screens
{
    public class ReplayScreen : LevelCompleteScreen
    {
        protected GameManager _currentManager;

        protected override string BtnLeftText
        {
            get
            {
                return "Exit";
            }
        }

        protected override string BtnRightText
        {
            get
            {
                return "Replay";
            }
        }

        protected override bool DrawStars
        {
            get
            {
                return false;
            }
        }

        protected override int HighScore
        {
            get
            {
                return GameGlobals.SaveData.ArcadeScore;
            }
        }

        public ReplayScreen(GameManager currentManager) : base(0)
        {
            _currentManager = currentManager;
        }

        protected override void NewHighScore(int score)
        {
            GameGlobals.SaveData.ArcadeScore = score;
            SaveData.Save();
        }

        public override void LoadContent()
        {
            base.LoadContent();
           /* foreach (var start in _starts)
            {
                start.IsHidden = true;
            }*/
        }

        protected override void OnBtnLeftPressed(object sender, System.EventArgs eventArgs)
        {
            LoadingScreen.Load(true, false, new MainMenuScreen());
            GameGlobals.GameOver = false;
            GameGlobals.Score = 0;
            GameGlobals.MaxHeight = 0;
        }

        protected override void OnBtnRightPressed(object sender, System.EventArgs eventArgs)
        {
            GameGlobals.GameOver = false;
            GameGlobals.Score = 0;
            GameGlobals.MaxHeight = 0;
            _currentManager.Reset();
            ExitScreen();
            _currentManager.Inactive = false;
        }

    }
}
