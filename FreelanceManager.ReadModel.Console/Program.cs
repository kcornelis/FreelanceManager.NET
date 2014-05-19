using System.Configuration;
using FreelanceManager.Infrastructure;
using NLog;

namespace FreelanceManager.ReadModel.Console
{
    class Program
    {
        private static readonly Logger _logger = LogManager.GetLogger(Loggers.ReadModel);

        static void Main(string[] args)
        {
            _logger.Info("Starting read model console");

            using (var setup = new Setup())
            {
                setup.WithThreadStaticTenantContext();

                setup.RegisterReadModelServices();
                setup.WithMongo("MongoConnectionReadModel");
                setup.RegisterReadModelRepositories();

#if DEBUG
                setup.WithRabbitMqBus();
#else
                setup.WithAzureBus();
#endif
                setup.RegisterReadModelHandlers();

                setup.WithMongoEventStore("MongoConnectionEventStore", null, null, true);


                //if (args.Any(a => a.ToLower().Contains("rebuild")))
                //{
                //    using (var scope = container.BeginLifetimeScope())
                //    {
                //        RegisterEventStore(scope);
                //        scope.Resolve<IRebuild>().FromEventStore();
                //    }
                //}

                _logger.Info("Registering services finished");

                _logger.Info("Starting bus");

                setup.StartBus(ConfigurationManager.AppSettings["serviceBusEndpoint"]);

                System.Console.Read();

                _logger.Info("Ending the read model console");
            }
        }
    }
}
