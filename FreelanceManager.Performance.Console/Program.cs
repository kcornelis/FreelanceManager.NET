using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Security.Policy;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using FreelanceManager.Performance.Models;
using NLog;

namespace FreelanceManager.Performance.Console
{
    class Program
    {
        private static readonly Logger _logger = LogManager.GetLogger("FreelanceManager.Performance");

        static void Main(string[] args)
        {
            var tasks = new List<Task>();

            for (int i = 0; i < 5; i++)
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

            _logger.Info("Finished");

            System.Console.Read();
        }

        
    }
}
