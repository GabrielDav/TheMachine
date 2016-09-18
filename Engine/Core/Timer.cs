using System;

namespace Engine.Core
{
    public class Timer : IDisposable
    {
        protected long _duration;
        protected long _elapsed;

#if DEBUG_TIMER
        protected long _lastUpdate;
#endif

        public bool Paused { get; protected set; }
        public bool Finished { get; protected set; }
        public bool Ticking { get; private set; }
        public bool Stoped { get; private set; }
        public bool Started { get; protected set; }
        public bool InGameTime { get; protected set; }

        public bool Loop;

        public event SimpleEvent OnFinish;

        public long Duration
        {
            get
            {
                return _duration; 
            }
        }

        public long TimeElapsed
        {
            get
            {
                return _elapsed;
            }
        }

        public long TimeLeft
        {
            get
            {
                return _duration - _elapsed;
            }
        }
        

        public Timer()
        {
            Init();
        }

        public Timer(bool useInGameTime)
        {
            InGameTime = useInGameTime;
        }

        protected void Init()
        {
            _duration = 0;
            _elapsed = 0;
            Stoped = false;
            Paused = false;
            Finished = false;
            Ticking = false;
            Started = false;
        }

        public void Start(long duration, bool loop)
        {
            Init();
            _duration = duration;
            Loop = loop;
            Ticking = true;
            Started = true;
        }

        public void Start(long duration)
        {
            Start(duration, false);
        }

        public void Update()
        {
            if (Finished || Stoped || Paused || !Started)
                return;
#if DEBUG_TIMER
            if (_lastUpdate == Controller.CurrentUpdateId)
                throw new Exception("Timer already have been updated");
            _lastUpdate = Controller.CurrentUpdateId;
#endif
            if (InGameTime)
                _elapsed += (long)EngineGlobals.GetElapsedInGameTime();
            else
                _elapsed += EngineGlobals.GameTime.ElapsedGameTime.Milliseconds;
            if (_elapsed >= _duration)
            {
                Finished = true;
                Ticking = false;
                Started = false;
                if (OnFinish != null)
                    OnFinish(this);
                if (Loop)
                    Restart();
            }
        }

        public void Stop()
        {
            Stoped = true;
            Ticking = false;
            Paused = false;
            Finished = true;
            Started = false;
            if (OnFinish != null)
                OnFinish(this);
        }

        public void Pause()
        {
            if (!Ticking)
                return;
            Ticking = false;
            Paused = true;
        }

        public void Resume()
        {
            if (!Paused)
                return;
            if (Finished)
                return;
            if (Stoped)
                return;
            if (Ticking)
                return;
            Paused = false;
            Ticking = true;
        }

        public void Restart()
        {
            Start(_duration, Loop);
        }

        public void Dispose()
        {
            OnFinish = null;
        }

    }
}
