using System.Configuration;
using Autofac;
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

            using (var container = new ContainerBuilder().Build())
            {
                RegisterServices(container);
                RegisterBus(container);

                _logger.Info("Registering services finished");

                container.Resolve<IServiceBus>().Start(ConfigurationManager.AppSettings["serviceBusEndpoint"]);

                System.Console.Read();
            }
        }

        static void RegisterServices(IContainer container)
        {
            var builder = new ContainerBuilder();
            var readModelAssembly = typeof(FreelanceManager.ReadModel.Account).Assembly;

            builder.RegisterType<ThreadStaticTenantContext>().As<ITenantContext>();
            builder.RegisterType<MongoContext>().As<IMongoContext>().SingleInstance().WithParameter("url", ConfigurationManager.ConnectionStrings["MongoConnectionReadModel"].ConnectionString);

            builder.RegisterAssemblyTypes(readModelAssembly)
                   .Where(t => t.Name.EndsWith("Repository"))
                   .AsImplementedInterfaces();

            builder.Update(container.ComponentRegistry);
        }

        static void RegisterBus(IContainer container)
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
