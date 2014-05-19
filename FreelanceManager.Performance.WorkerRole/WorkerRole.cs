using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.WindowsAzure;
using Microsoft.WindowsAzure.Diagnostics;
using Microsoft.WindowsAzure.ServiceRuntime;
using Microsoft.WindowsAzure.Storage;
using NLog;

namespace FreelanceManager.Performance.WorkerRole
{
    public class WorkerRole : RoleEntryPoint
    {
        private static readonly Logger _logger = LogManager.GetLogger("FreelanceManager.Performance");

        public override void Run()
        {
            // This is a sample worker implementation. Replace with your logic.
            Trace.TraceInformation("FreelanceManager.Performance.WorkerRole entry point called");

            while (true)
            {
                var tasks = new List<Task>();

                for (int i = 0; i < 2; i++)
                {
                    tasks.Add(Task.Factory.StartNew(() =>
                    {
                        var appdomainSetup = AppDomain.CurrentDomain.SetupInformation;

                        var appdomain = AppDomain.CreateDomain("Worker_" + i, null, appdomainSetup);

                        dynamic worker = appdomain.CreateInstanceAndUnwrap(typeof(Worker).Assembly.FullName, typeof(Worker).FullName);
                        worker.Start();
                    }));
                }

                Task.WaitAll(tasks.ToArray());
            }
        }

        public override bool OnStart()
        {
            // Set the maximum number of concurrent connections 
            ServicePointManager.DefaultConnectionLimit = 12;

            return base.OnStart();
        }
    }
}
