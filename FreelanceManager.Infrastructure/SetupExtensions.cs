using System.Configuration;
using System.Linq;
using System.Reflection;
using Autofac;
using EventStore;
using EventStore.Dispatcher;
using EventStore.Serialization;
using MongoDB.Bson.Serialization;

namespace FreelanceManager.Infrastructure
{
    public static class SetupExtensions
    {

        public static Setup WithThreadStaticTenantContext(this Setup setup)
        {
            var builder = new ContainerBuilder();

            builder.RegisterType<ThreadStaticTenantContext>().As<ITenantContext>();
            
            builder.Update(setup.Container.ComponentRegistry);

            return setup;
        }

        public static Setup WithGuidGenerator(this Setup setup)
        {
            var builder = new ContainerBuilder();

            builder.RegisterType<GuidGenerator>().As<IIdGenerator>();

            builder.Update(setup.Container.ComponentRegistry);

            return setup;
        }

        public static Setup WithMongo(this Setup setup, string connectionstringKey)
        {
            var builder = new ContainerBuilder();

            builder.RegisterType<MongoContext>().As<IMongoContext>().SingleInstance().WithParameter("url", ConfigurationManager.ConnectionStrings[connectionstringKey].ConnectionString);

            builder.Update(setup.Container.ComponentRegistry);

            return setup;
        }

        public static Setup WithAzureBus(this Setup setup)
        {
            var builder = new ContainerBuilder();

            var bus = new AzureServiceBus(setup.Container);

            builder.RegisterInstance(bus).As<IServiceBus>();

            builder.Update(setup.Container.ComponentRegistry);

            return setup;
        }

        public static Setup WithRabbitMqBus(this Setup setup)
        {
            var builder = new ContainerBuilder();

            var bus = new RabbitMqServiceBus(setup.Container);

            builder.RegisterInstance(bus).As<IServiceBus>();

            builder.Update(setup.Container.ComponentRegistry);

            return setup;
        }

        public static Setup WithInMemoryBus(this Setup setup)
        {
            var builder = new ContainerBuilder();

            var bus = new InMemoryServiceBus(setup.Container);

            builder.RegisterInstance(bus).As<IServiceBus>();

            builder.Update(setup.Container.ComponentRegistry);

            return setup;
        }

        public static void StartBus(this Setup setup, string name)
        {
            var bus = setup.Container.Resolve<IServiceBus>();
            bus.Start(name);
        }

        public static Setup WithMongoEventStore(this Setup setup, string configConnectionKey, IPipelineHook hook, IDispatchCommits dispatcher, bool output)
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

            var eventStoreSetup = Wireup.Init()
                 .UsingMongoPersistence(configConnectionKey, new DocumentObjectSerializer())
                 .InitializeStorageEngine();

            if (output)
                eventStoreSetup.LogToOutputWindow();

            if (hook != null)
                eventStoreSetup.HookIntoPipelineUsing(hook);

            if (dispatcher != null)
                eventStoreSetup.UsingSynchronousDispatchScheduler()
                    .DispatchTo(dispatcher);


            var builder = new ContainerBuilder();
            builder.RegisterInstance<IStoreEvents>(eventStoreSetup.Build()).ExternallyOwned();
            builder.RegisterType<AggregateRootRepository>().As<IAggregateRootRepository>();
            builder.Update(setup.Container.ComponentRegistry);

            return setup;
        }
    }
}
