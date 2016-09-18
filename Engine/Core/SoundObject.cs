using Engine.Mechanics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;

namespace Engine.Core
{
    public class SoundObject
    {
        public SoundEffectInstance Instance;
        public string Id;
        public bool Started;

        protected string _effectName;
        protected PhysicalObject _source;
        protected float _volume;

        public SoundObject(string id, SoundEffectInstance instance, PhysicalObject source, float volume, bool loop, float pan, float pitch)
        {
            Id = id;
            Instance = instance;
            _source = source;
            Instance.Pan = pan;
            Instance.Pitch = pitch;
            Instance.IsLooped = loop;
            _volume = volume;
        }

        public void Stop()
        {
            Instance.Stop();
        }

        public void Dispose()
        {
            Instance.Dispose();
        }

        public void Pause()
        {
            Instance.Pause();
        }

        public void Resume()
        {
            Instance.Resume();
        }

        public bool Play(Vector2 listenerPos)
        {
            Started = true;

            var volume = 1f;

            if (_source != null)
            {
                volume = VolumeByPosition(listenerPos);
            }
            else
            {
                volume = _volume*SoundManager.GlobalSoundVolume;
            }

            if (volume > 0)
            {
                Instance.Play();

                return true;
            }

            Instance.Dispose();

            return false;
        }

        public void AdjustVolumeByDistance(Vector2 listenerPos)
        {
            if (_source != null)
            {
                Instance.Volume = VolumeByPosition(listenerPos);
            }
        }

        private float VolumeByPosition(Vector2 listenerPos)
        {
            var distance = Vector2.Distance(_source.HalfPos, listenerPos);
            var volume = (1*SoundManager.GlobalSoundVolume*_volume) -
                         (SoundManager.GlobalSoundVolume*_volume*(distance/EngineGlobals.MaximumSoundDistance));

            if (volume > 1)
            {
                return 1;
            }

            if (volume < 0)
            {
                return 0;
            }

            return volume;
        }
    }
}
