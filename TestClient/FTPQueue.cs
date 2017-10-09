using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace TestClient
{
    static class FTPQueue
    {
        static Thread thread;
        static List<string> queue = new List<string>();
        
        public static void Add(string path)
        {
            queue.Add(path);
            if (thread == null || thread.ThreadState != ThreadState.Running) Init();
        }

        private static void Init()
        {
            thread = new Thread(Run);
            thread.Start();
        }

        public static bool Running
        {
            get
            {
                if (thread == null)
                    return false;

                return thread.ThreadState == ThreadState.Running || queue.Count > 0;
            }
        }

        static void Run()
        {
            while(queue.Count > 0)
            {
                string s = queue.First();
                FTPClient.Upload(s, true);
                queue.Remove(s);
            }
        }

    }
}
