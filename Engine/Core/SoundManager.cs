using System;
using System.Collections.Generic;
using Engine.Mechanics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;

namespace Engine.Core
{
    public class SoundManager : IDisposable
    {
        protected IList<SoundObject> _soundInstances;
        protected IList<SoundObject> _toRemove;
        protected static float _globalSoundVolume;

        public static float GlobalSoundVolume
        {
            get { return _globalSoundVolume; }
            set
            {
                _globalSoundVolume = value;
                if (Math.Abs(_globalSoundVolume - 0.0f) < EngineGlobals.Epsilon)
                {
                    MuteSound = true;
                }
            }
        }

        public static bool MuteSound { get; set; }

        public SoundManager()
        {
            _toRemove = new List<SoundObject>();
            _soundInstances = new List<SoundObject>();
        }

        public bool InstanceDuplicate(string id)
        {
            for (int i = 0; i < _soundInstances.Count; i++)
            {
                if (_soundInstances[i].Id == id)
                {
                    return true;
                }
            }

            return false;
        }

        public void Pause(string id)
        {
            for (int i = 0; i < _soundInstances.Count; i++)
            {
                if (_soundInstances[i].Id == id)
                {
                    _soundInstances[i].Pause();
                }
            }
        }

        public void Stop(string id)
        {
            for (int i = 0; i < _soundInstances.Count; i++)
            {
                if (_soundInstances[i].Id == id)
                {
                    _soundInstances[i].Stop();
                }
            }
        }

        public void StopAllSounds()
        {
            for (int i = 0; i < _soundInstances.Count; i++)
            {
                _soundInstances[i].Stop();
            }
        }

        public void Resume(string id)
        {
            for (int i = 0; i < _soundInstances.Count; i++)
            {
                if (_soundInstances[i].Id == id)
                {
                    _soundInstances[i].Resume();
                }
            }
        }

        public void Play(string id, string name, PhysicalObject source, float volume, bool loop, float pitch, float pan)
        {
            //var effect = GetEffect(name);

            if (!InstanceDuplicate(id))
            {
                var sound = new SoundObject(id, EngineGlobals.Resources.Sounds[name][0].CreateInstance(), source, volume, loop, pan, pitch);

                _soundInstances.Add(sound);
            }
        }


        public void Play(string id, string name, float volume, bool loop, float pitch, float pan)
        {
            Play(id, name, null, volume, loop, pitch, pan);
        }

        public void Update(Vector2 listenerPos)
        {
            if(!MuteSound)
            {
                for (int i = 0; i < _soundInstances.Count; i++)
                {
                   
                    _soundInstances[i].AdjustVolumeByDistance(listenerPos);

                    if (!_soundInstances[i].Started)
                    {
                        if (!_soundInstances[i].Play(listenerPos))
                        {
                            _toRemove.Add(_soundInstances[i]);
                        }
                    }
                    else
                    {
                        if (_soundInstances[i].Instance.State == SoundState.Stopped)
                        {
                            _soundInstances[i].Dispose();
                            _toRemove.Add(_soundInstances[i]);
                        }
                    }
                }

                for (int i = 0; i < _toRemove.Count; i++)
                {
                    _soundInstances.Remove(_toRemove[i]);
                }

                _toRemove.Clear();
            }
        }

        /*public void PlayMusic(string songName, bool loop)
        {
            if(MediaPlayer.GameHasControl && !MuteMusic)
            {
                FrameworkDispatcher.Update();
                //MediaPlayer.Play(_songs[songName]);
                MediaPlayer.Play(EngineGlobals.Resources.Songs[songName]);
                MediaPlayer.IsRepeating = loop;
            }
        }*/

       /* public void PauseMusic()
        {
            MediaPlayer.Pause();
        }

        public void StopMusic()
        {
            MediaPlayer.Stop();
        }

        public void ResumeMusic()
        {
            MediaPlayer.Resume();
        }
        */

        public void Dispose()
        {
            foreach (SoundObject t in _soundInstances)
            {
                t.Stop();
                t.Dispose();
            }

            _soundInstances.Clear();
            _toRemove.Clear();
            //foreach (var song in _songs)
            //{
            //    song.Value.Dispose();
            //}

            //_songs.Clear();
            
        }
    }
}
