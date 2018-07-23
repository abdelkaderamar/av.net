using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.Concurrent;
using System.Threading;
using log4net;

namespace Av.API
{
    public abstract class AvRequestManager<REQUEST_TYPE, REQUEST_DATA>
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(AvRequestManager<REQUEST_TYPE, REQUEST_DATA>));

        protected bool _run;

        protected Thread _thread;

        // Delay betwwen two successive requests (ms)
        public static readonly int DEFAULT_DELAY = 2000;

        // Sleep time if requests queue is empty (ms)
        public static readonly int YIELD_DELAY = 100;

        // Delay increment if the request is rejected for high usage (ms)
        public static readonly int DELAY_INCREMENT = 1000;

        public static readonly int DEFAULT_MAX_RETRY = 8;

        protected int _currentRetry = 0;

        protected ConcurrentQueue<REQUEST_DATA> _requests;

        public AvRequestManager()
        {
            _run = false;
            _thread = new Thread(Run);
            Delay = DEFAULT_DELAY;
            CurrentDelay = Delay;
            EnabledRetry = false;
            MaxRetry = DEFAULT_MAX_RETRY;
            _requests = new ConcurrentQueue<REQUEST_DATA>();
        }

        public int Delay { get; set; }

        public int CurrentDelay { get; set; }

        public bool EnabledRetry { get; set; }

        public int MaxRetry { get; set; }

        public void Start()
        {
            _run = true;
            _thread.Start();
        }

        protected void Run()
        {
            while (_run)
            {
                REQUEST_DATA requestData;
                if (_requests.TryDequeue(out requestData))
                {
                    Execute(requestData);
                    Thread.Sleep(Delay);
                }
                else
                {
                    Thread.Sleep(YIELD_DELAY);
                }
            }
        }

        public void Stop(bool finish)
        {
            _run = false;
            if (finish) Finish();
            _thread.Join();
        }

        protected void Finish()
        {
            REQUEST_DATA requestData;
            while (_requests.TryDequeue(out requestData))
            {
                Execute(requestData);
                Thread.Sleep(Delay);
            }
        }

        protected void IncrementDelay()
        {
            CurrentDelay += DELAY_INCREMENT;
        }

        protected bool CanRetry()
        {
            return (_currentRetry < MaxRetry);
        }

        protected void ReExecute(REQUEST_DATA requestData)
        {
            ++_currentRetry;
            IncrementDelay();
            log.WarnFormat("Reexecuting the request with delay = {0} / Attempt = {1}",
                CurrentDelay, _currentRetry);
            Thread.Sleep(CurrentDelay);
            Execute(requestData);
        }

        protected void ResetDelay()
        {
            CurrentDelay = Math.Max(Delay, CurrentDelay - DELAY_INCREMENT);
            _currentRetry = 0;
        }

        protected abstract void Execute(REQUEST_DATA requestData);

    }
}
