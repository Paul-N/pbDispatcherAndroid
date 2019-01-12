using System;
using System.Timers;

namespace Dispatcher.Android.Utils
{
    public class TimerHolder
    {
        private Timer _timer;
        
        private readonly Action _callback;
        private readonly double _interval;
        
        public TimerHolder(double interval, Action callback)
        {
            _interval = interval;
            _callback = callback ?? throw new ArgumentNullException(nameof(callback));
        }
        
        public void Start()
        {
            if (_timer == null)
                Create();
            
            _timer.Start();
        }
        
        public void Stop()
        {
            if (_timer == null) return;
            
            _timer.Stop();
            _timer.Dispose();
            _timer = null;
        }

        private void Create()
        {
            _timer = new Timer { AutoReset = true, Interval = _interval };
            _timer.Elapsed += delegate { _callback.Invoke(); };
        }
    }
}