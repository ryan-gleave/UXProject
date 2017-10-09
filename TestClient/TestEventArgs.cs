using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TestClient
{
    public class TestEventArgs
    {
        public bool exitOnEnd;

        public TestEventArgs(bool exitOnEnd = false)
        {
            this.exitOnEnd = exitOnEnd;
        }
    }
}
