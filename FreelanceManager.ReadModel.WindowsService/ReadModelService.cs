using System.Configuration;
using System.ServiceProcess;
using Autofac;
using FreelanceManager.Infrastructure;
using FreelanceManager.ReadModel.Tools;
using NLog;

namespace FreelanceManager.ReadModel.WindowsService
{
    public partial class ReadModelService : ServiceBase
    {
        private static readonly Logger _logger = LogManager.GetLogger(Loggers.ReadModel);
        private IContainer _container;

        public ReadModelService()
        {
            InitializeComponent();
        }

        protected override void OnStart(string[] args)
        {
            _logger.Info("Starting read model console");

            _container = new ContainerBuilder().Build();

            RegisterServices(_container);
            RegisterBus(_container);

            _logger.Info("Registering services finished");

            _container.Resolve<IServiceBus>().Start(ConfigurationManager.AppSettings["serviceBusEndpoint"]);
        }

        protected override void OnStop()
        {
            _container.Dispose();
        }

        private void RegisterServices(IContainer container)
        {
            var builder = new ContainerBuilder();
            var readModelAssembly = typeof(FreelanceManager.ReadModel.Account).Assembly;

            builder.RegisterType<DomainUpdateServiceBusHandlerHook>().As<IDomainUpdateServiceBusHandlerHook>();
            builder.RegisterType<ThreadStaticTenantContext>().As<ITenantContext>();
            builder.RegisterType<MongoContext>().As<IMongoContext>().SingleInstance().WithParameter("url", ConfigurationManager.ConnectionStrings["MongoConnectionReadModel"].ConnectionString);

            builder.RegisterAssemblyTypes(readModelAssembly)
                   .Where(t => t.Name.EndsWith("Repository"))
                   .AsImplementedInterfaces();

            builder.Update(container.ComponentRegistry);
        }

        private void RegisterBus(IContainer container)
        {
            var builder = new ContainerBuilder();
            var readModelAssembly = typeof(FreelanceManager.ReadModel.Account).Assembly;

#if DEBUG
            var bus = new RabbitMqServiceBus(container);
#else
            var bus = new AzureServiceBus(container);
#endif
            bus.RegisterHandlers(readModelAssembly);

            builder.RegisterInstance(bus).As<IServiceBus>();

            builder.Update(container.ComponentRegistry);
        }
    }
}
