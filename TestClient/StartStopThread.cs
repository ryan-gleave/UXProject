using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace TestClient
{
    abstract class StartStopThread
    {
        private Thread thread;
        protected bool running;

        public void Join()
        {
            thread.Join();
        }

        public virtual void Start()
        {
            this.running = true;
            thread = new Thread(Run);
            thread.Start();
        }

        public virtual void Stop()
        {
            this.running = false;
        }

        protected abstract void Run();
    }
}
