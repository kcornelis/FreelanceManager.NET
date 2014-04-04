using System.Configuration;
using System.Linq;
using System.Reflection;
using Autofac;
using EventStore;
using EventStore.Serialization;
using FreelanceManager.Infrastructure;
using FreelanceManager.Web.Tools;
using MongoDB.Bson.Serialization;
using Nancy;
using Nancy.Authentication.Forms;
using Nancy.Bootstrapper;
using Nancy.Bootstrappers.Autofac;
using Nancy.Conventions;
using Nancy.Security;
using NLog;

namespace FreelanceManager.Web
{
    public class Bootstrapper : AutofacNancyBootstrapper
    {
        private static readonly Logger _logger = LogManager.GetLogger(Loggers.Web);

        protected override void ConfigureConventions(Nancy.Conventions.NancyConventions nancyConventions)
        {
            base.ConfigureConventions(nancyConventions);

            nancyConventions.StaticContentsConventions.Add(StaticContentConventionBuilder.AddDirectory("fonts"));
        }

        protected override void ApplicationStartup(ILifetimeScope container, IPipelines pipelines)
        {
        }

        protected override void ConfigureApplicationContainer(ILifetimeScope existingContainer)
        {
            RegisterServices(existingContainer);
            RegisterBus(existingContainer);
            RegisterEventStore(existingContainer);
        }

        protected override void ConfigureRequestContainer(ILifetimeScope container, NancyContext context)
        {
            // Perform registrations that should have a request lifetime
        }

        protected override void RequestStartup(ILifetimeScope container, IPipelines pipelines, NancyContext context)
        {
            // No registrations should be performed in here, however you may
            // resolve things that are needed during request startup.

            FormsAuthentication.Enable(pipelines, new FormsAuthenticationConfiguration()
            {
                RedirectUrl = "~/account/login",
                UserMapper = container.Resolve<IUserMapper>(),
            });

            pipelines.BeforeRequest.AddItemToEndOfPipeline(c =>
            {
                if (c.CurrentUser.IsAuthenticated())
                {
                    container.Resolve<ITenantContext>().SetTenantId(c.CurrentUser.AsAuthenticatedUser().Id, c.CurrentUser.HasClaim("Admin"));
                    c.ViewBag.UserName = c.CurrentUser.AsAuthenticatedUser().FullName;
                    c.ViewBag.IsAdmin = c.CurrentUser.HasClaim("Admin");
                }
                else
                    container.Resolve<ITenantContext>().SetTenantId(null, false);

                return null;
            });

            pipelines.OnError.AddItemToEndOfPipeline((z, a) =>
            {
                _logger.Error("Unhandled error on request: " + context.Request.Url + " : " + a.Message, a);
                return a.Message;
            });
        }

        private void RegisterServices(ILifetimeScope container)
        {
            var builder = new ContainerBuilder();
            var readModelAssembly = typeof(FreelanceManager.ReadModel.Account).Assembly;

            builder.RegisterType<NancyUserMapper>().As<IUserMapper>();
            builder.RegisterType<GuidGenerator>().As<IIdGenerator>();
            builder.RegisterType<ThreadStaticTenantContext>().As<ITenantContext>();
            builder.RegisterType<MongoContext>().As<IMongoContext>().SingleInstance().WithParameter("url", ConfigurationManager.ConnectionStrings["MongoConnectionReadModel"].ConnectionString);

            builder.RegisterType<AggregateRootRepository>().As<IAggregateRootRepository>();
            builder.RegisterType<StaticContentResolverForWeb>().As<IStaticContentResolver>();
            builder.RegisterType<ExcelService>().As<IExcelService>();

            builder.RegisterAssemblyTypes(readModelAssembly)
                   .Where(t => t.Name.EndsWith("Repository"))
                   .AsImplementedInterfaces();

            builder.Update(container.ComponentRegistry);
        }

        private void RegisterBus(ILifetimeScope container)
        {
            var builder = new ContainerBuilder();

            var readModelAssembly = typeof(FreelanceManager.ReadModel.Account).Assembly;

#if DEBUG
            var bus = new RabbitMqServiceBus(container);
#else
            var bus = new AzureServiceBus(container);
#endif

            bus.Start(ConfigurationManager.AppSettings["serviceBusEndpoint"]);

            builder.RegisterInstance(bus).As<IServiceBus>();

            builder.Update(container.ComponentRegistry);
        }

        private void RegisterEventStore(ILifetimeScope container)
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
                .HookIntoPipelineUsing(new[] { new AuthorizationPipelineHook(container) })
                .UsingSynchronousDispatchScheduler()
                    .DispatchTo(new MessageDispatcher(container))
                .Build();

            var builder = new ContainerBuilder();
            builder.RegisterInstance<IStoreEvents>(eventStore).ExternallyOwned();
            builder.Update(container.ComponentRegistry);
        }
    }
}