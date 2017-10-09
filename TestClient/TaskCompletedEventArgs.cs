using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UXProject.Data;

namespace TestClient
{
    public class TaskCompletedEventArgs
    {
        public Task Task;
        public bool Abandoned = false;
    }
}
