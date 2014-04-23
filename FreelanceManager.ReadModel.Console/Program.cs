using System.Configuration;
using System.Reflection;
using System.Linq;
using Autofac;
using FreelanceManager.Infrastructure;
using FreelanceManager.ReadModel.Tools;
using MongoDB.Bson.Serialization;
using NLog;
using EventStore;
using EventStore.Serialization;
using System;

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

                if (args.Any(a => a.ToLower().Contains("rebuild")))
                {
                    using (var scope = container.BeginLifetimeScope())
                    {
                        RegisterEventStore(scope);
                        scope.Resolve<IRebuild>().FromEventStore();
                    }
                }

                _logger.Info("Registering services finished");

                container.Resolve<IServiceBus>().Start(ConfigurationManager.AppSettings["serviceBusEndpoint"]);

                System.Console.Read();
            }
        }

        static void RegisterServices(IContainer container)
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

        static void RegisterEventStore(ILifetimeScope container)
        {
            var types = Assembly.GetAssembly(typeof(FreelanceManager.Events.Event))
                                .GetTypes()
                                .Where(type => type.IsClass && !type.ContainsGenericParameters)
                                .Where(type => type.IsSubclassOf(typeof(FreelanceManager.Events.Event)) ||
                                               type.Namespace.Contains("FreelanceManager.Dtos"));

            foreach (var t in types)
                BsonClassMap.LookupClassMap(t);

            BsonClassMap.LookupClassMap(typeof(Date));
            BsonClassMap.LookupClassMap(typeof(Time));
            BsonClassMap.LookupClassMap(typeof(Money));

            var eventStore = Wireup.Init()
                .UsingMongoPersistence("MongoConnectionEventStore", new DocumentObjectSerializer())
                .InitializeStorageEngine()
                .Build();

            var builder = new ContainerBuilder();
            builder.RegisterInstance<IStoreEvents>(eventStore).ExternallyOwned();
            builder.RegisterInstance<ILifetimeScope>(container).ExternallyOwned();
            builder.RegisterType<Rebuild>().As<IRebuild>();
            builder.Update(container.ComponentRegistry);
        }
    }
}
