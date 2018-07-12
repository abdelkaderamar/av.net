using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.Concurrent;
using System.Threading;

namespace Av.API
{
    public abstract class AvRequestManager<REQUEST_TYPE, REQUEST_DATA>
    {
        protected bool _run;

        protected Thread _thread;

        // Delay betwwen two successive requests
        public static int DEFAULT_DELAY = 2000;

        // Sleep time if requests queue is empty
        public static int YIELD_DELAY = 100;

        protected ConcurrentQueue<REQUEST_DATA> _requests;

        public AvRequestManager()
        {
            _run = false;
            _thread = new Thread(Run);
            Delay = DEFAULT_DELAY;
            _requests = new ConcurrentQueue<REQUEST_DATA>();
        }

        public int Delay { get; set; }

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

        protected abstract void Execute(REQUEST_DATA requestData);

    }
}
