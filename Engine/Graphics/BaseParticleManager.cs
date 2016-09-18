using System;
using System.ComponentModel;
using Engine.Core;
using Engine.Mechanics;

namespace Engine.Graphics
{
    #if EDITOR
    public abstract class BaseParticleManager : IDisposable, ICloneable
    #else
    public abstract class BaseParticleManager : IDisposable
    #endif
    {
        public bool Enabled { get; set; }
        protected int _lifeTime;
        protected int _creationRateMin;
        protected int _creationRateMax;
        protected int _waveAmountMin;
        protected int _waveAmountMax;
        protected Timer _timer;
        protected string _particleName;
        protected int _maxWaves;
        private int _currentWave;

#if EDITOR
        public event PropertyChangingEventHandler PropertyChanging;
        public event PropertyChangedEventHandler PropertyChanged;
#endif

        public int LifeTime { get { return _lifeTime; } set { _lifeTime = value; } }
        public int CreationRateMin
        {
            get { return _creationRateMin; }
            set { _creationRateMin = value; }
        }

        public int CreationRateMax
        {
            get { return _creationRateMax; }
        
            set { _creationRateMax = value; }
        }

        public int WaveAmountMin
        {
            get { return _waveAmountMin; }
            set { _waveAmountMin = value; }
        }

        public int WaveAmountMax
        {
            get { return _waveAmountMax; }
            set { _waveAmountMax = value; }
        }

#if EDITOR
        protected void ExecuteNotify(string name, bool changed)
        {
            if (changed)
            {
                if (PropertyChanged != null)
                    PropertyChanged(this, new PropertyChangedEventArgs(name));
            }
            else
            {
                if (PropertyChanging != null)
                    PropertyChanging(this, new PropertyChangingEventArgs(name));
            }
        }
#endif
        

        protected BaseParticleManager()
        {
            Enabled = true;
        }

        public abstract Particle CreateNewParticle();

        public abstract void LoadDefault();

        public virtual void Load(PhysicalObject owner)
        {
            _timer = new Timer();
            _timer.Start(0, false);
        }

        protected abstract void InitParticle(Particle particle);

        public virtual void Dispose()
        {
            if (_timer != null)
            {
                _timer.Dispose();
                _timer = null;
            }
        }

        public object Clone()
        {
            return MemberwiseClone();
        }

        public void Update()
        {
            if (Enabled)
            {
                
                _timer.Update();
                if (_timer.Finished)
                {
                    if (_maxWaves > 0)
                    {
                        if (_currentWave > _maxWaves)
                        {
                            Enabled = false;
                            return;
                        }
                        _currentWave++;
                    }
                    var amount = (_waveAmountMin == _waveAmountMax)
                                      ? _waveAmountMin
                                      : EngineGlobals.Random.Next(_waveAmountMin, _waveAmountMax);
                    for (var i = 0; i < amount; i++)
                    {
                        var particle = EngineGlobals.ParticleStorageManager.GetParticle(this, _particleName);
                        particle.Activate(_lifeTime);
                        InitParticle(particle);
                    }
                    _timer.Start(
                        _creationRateMin == _creationRateMax
                            ? _creationRateMin
                            : EngineGlobals.Random.Next(_creationRateMin, _creationRateMax), false);
                }
            }
        }

        public void Reset()
        {
            _currentWave = 0;
            Enabled = true;
        }
    }
}
