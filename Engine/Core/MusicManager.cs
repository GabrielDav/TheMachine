using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Media;

namespace Engine.Core
{
    public static class MusicManager
    {
        private static float _globalMusicVolume;

        private static bool _muteMusic;

        private static bool _playAfterPause;

        public static bool IsPlaying { get; private set; }

        private static string _lastPlayed;

        private static bool _lastPlayedLooping;

        private static bool _paused;

        public static float GlobalMusicVolume
        {
            get { return _globalMusicVolume; }
            set
            {
                _globalMusicVolume = value;
                if (Math.Abs(_globalMusicVolume - 0.0f) < EngineGlobals.Epsilon)
                {
                    MuteMusic = true;
                }
                else
                {
                    if (MediaPlayer.GameHasControl)
                    {
                        MediaPlayer.Volume = value;
                        if (MuteMusic && value > 0)
                            MuteMusic = false;
                    }
                }
            }
        }

        public static bool MuteMusic
        {
            get { return _muteMusic; }
            set
            {
                _muteMusic = value;
                if (MediaPlayer.GameHasControl)
                {
                    if (_muteMusic)
                        Stop();
                }
            }
        }

        public static void Play(string songName, bool loop)
        {
            _lastPlayed = songName;
            _lastPlayedLooping = loop;

            if (!string.IsNullOrEmpty(songName) && !MuteMusic && MediaPlayer.GameHasControl)
            {
                FrameworkDispatcher.Update();
                MediaPlayer.Play(EngineGlobals.Resources.Songs[songName]);
                MediaPlayer.IsRepeating = loop;
                IsPlaying = true;
            }
            _paused = false;
        }

        public static void Pause()
        {
            if (IsPlaying && MediaPlayer.GameHasControl)
            {
                MediaPlayer.Pause();
                _playAfterPause = true;
            }

            IsPlaying = false;
            _paused = true;
        }

        public static void Unpause()
        {
            if (!IsPlaying &&  MediaPlayer.GameHasControl && _paused)
            {
                if (_playAfterPause)
                {
                    _playAfterPause = false;
                    Play(_lastPlayed, _lastPlayedLooping);
                }

                MediaPlayer.Resume();
                IsPlaying = true;
                _paused = false;
            }
        }

        public static void Resume()
        {
            if (!_paused)
            {
                Play(_lastPlayed, _lastPlayedLooping);
            }
        }

        public static void Stop()
        {
            if (IsPlaying && MediaPlayer.GameHasControl)
            {
                MediaPlayer.Stop();
                IsPlaying = false;
            }
        }
    }
}
