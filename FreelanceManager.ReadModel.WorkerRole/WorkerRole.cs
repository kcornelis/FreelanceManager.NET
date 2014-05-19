using System.Configuration;
using System.Diagnostics;
using System.Net;
using System.Threading;
using FreelanceManager.Infrastructure;
using Microsoft.WindowsAzure.ServiceRuntime;
using NLog;

namespace FreelanceManager.ReadModel.WorkerRole
{
    public class WorkerRole : RoleEntryPoint
    {
        private static readonly Logger _logger = LogManager.GetLogger(Loggers.ReadModel);
        private readonly ManualResetEvent CompletedEvent = new ManualResetEvent(false);
        private Setup _setup;
        
        public override void Run()
        {
            CompletedEvent.WaitOne();
        }

        public override bool OnStart()
        {
            // Set the maximum number of concurrent connections 
            ServicePointManager.DefaultConnectionLimit = 12;

            _logger.Info("Starting read model workerrole");

            _setup = new Setup();

            _setup.WithThreadStaticTenantContext();

            _setup.RegisterReadModelServices();
            _setup.WithMongo("MongoConnectionReadModel");
            _setup.RegisterReadModelRepositories();

#if DEBUG
            _setup.WithRabbitMqBus();
#else
            _setup.WithAzureBus();
#endif
            _setup.RegisterReadModelHandlers();

            _logger.Info("Registering services finished");

            _logger.Info("Starting bus");

            _setup.StartBus(ConfigurationManager.AppSettings["serviceBusEndpoint"]);

            return base.OnStart();
        }

        public override void OnStop()
        {
            _logger.Info("Stopping read model service");
            _setup.Dispose();

            CompletedEvent.Set();

            base.OnStop();
        }
    }
}
