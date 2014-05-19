using System.Configuration;
using System.ServiceProcess;
using FreelanceManager.Infrastructure;
using NLog;

namespace FreelanceManager.ReadModel.WindowsService
{
    public partial class ReadModelService : ServiceBase
    {
        private static readonly Logger _logger = LogManager.GetLogger(Loggers.ReadModel);
        private Setup _setup;

        public ReadModelService()
        {
            InitializeComponent();
        }

        protected override void OnStart(string[] args)
        {
            _logger.Info("Starting read model service");

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
        }

        protected override void OnStop()
        {
            _logger.Info("Stopping read model service");
            _setup.Dispose();
        }
    }
}
