using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Threading;
using Microsoft.ServiceBus;
using Microsoft.ServiceBus.Messaging;
using Microsoft.WindowsAzure;
using Microsoft.WindowsAzure.ServiceRuntime;

namespace FreelanceManager.ReadModel.WorkerRole
{
    public class WorkerRole : RoleEntryPoint
    {
        ManualResetEvent CompletedEvent = new ManualResetEvent(false);

        public override void Run()
        {
            Trace.WriteLine("Starting processing of messages");

            CompletedEvent.WaitOne();
        }

        public override bool OnStart()
        {
            // Set the maximum number of concurrent connections 
            ServicePointManager.DefaultConnectionLimit = 12;

            //// Create the queue if it does not exist already
            //string connectionString = CloudConfigurationManager.GetSetting("Microsoft.ServiceBus.ConnectionString");
            //var namespaceManager = NamespaceManager.CreateFromConnectionString(connectionString);
            //if (!namespaceManager.QueueExists(QueueName))
            //{
            //    namespaceManager.CreateQueue(QueueName);
            //}

            //// Initialize the connection to Service Bus Queue
            //Client = QueueClient.CreateFromConnectionString(connectionString, QueueName);
            return base.OnStart();
        }

        public override void OnStop()
        {
            // Close the connection to Service Bus Queue
            //Client.Close();
            CompletedEvent.Set();
            base.OnStop();
        }
    }
}
