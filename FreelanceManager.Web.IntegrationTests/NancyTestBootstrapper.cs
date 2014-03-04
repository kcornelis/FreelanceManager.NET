using System.Configuration;
using System.Linq;
using System.Reflection;
using Autofac;
using EventStore;
using EventStore.Serialization;
using FreelanceManager.Infrastructure;
using FreelanceManager.Web.Shared;
using MongoDB.Bson.Serialization;
using Nancy;
using Nancy.Authentication.Forms;
using Nancy.Bootstrapper;
using Nancy.Bootstrappers.Autofac;
using Nancy.Conventions;
using Nancy.Security;

namespace FreelanceManager.Web
{
    public class NancyTestBootstrapper : AutofacNancyBootstrapper
    {
        public ILifetimeScope CreateContainer()
        {
            return base.CreateRequestContainer();
        }

        protected override void ConfigureConventions(Nancy.Conventions.NancyConventions nancyConventions)
        {
            base.ConfigureConventions(nancyConventions);

            nancyConventions.StaticContentsConventions.Add(StaticContentConventionBuilder.AddDirectory("fonts"));
        }

        protected override void ApplicationStartup(ILifetimeScope container, IPipelines pipelines)
        {
            // No registrations should be performed in here, however you may
            // resolve things that are needed during application startup.
        }

        protected override void ConfigureApplicationContainer(ILifetimeScope existingContainer)
        {
            RegisterServices(existingContainer);
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
        }

        private void RegisterServices(ILifetimeScope container)
        {
            var builder = new ContainerBuilder();

            builder.RegisterType<NancyUserMapper>().As<IUserMapper>();
            builder.RegisterType<GuidGenerator>().As<IIdGenerator>();
            builder.RegisterType<ThreadStaticTenantContext>().As<ITenantContext>();
            builder.RegisterType<MongoContext>().As<IMongoContext>().SingleInstance().WithParameter("url", ConfigurationManager.ConnectionStrings["MongoConnectionReadModel"].ConnectionString);
            builder.RegisterType<InMemoryServiceBus>().As<IServiceBus>().SingleInstance().WithParameter("container", container);

            builder.RegisterType<AggregateRootRepository>().As<IAggregateRootRepository>();
            builder.RegisterType<StaticContentResolverForInMemory>().As<IStaticContentResolver>();

            var readModelAssembly = typeof(FreelanceManager.ReadModel.Account).Assembly;
            builder.RegisterAssemblyTypes(readModelAssembly)
                   .Where(t => t.Name.EndsWith("Repository"))
                   .AsImplementedInterfaces();
            builder.RegisterAssemblyTypes(readModelAssembly)
                   .Where(t => t.Name.EndsWith("Handlers"))
                   .AsSelf();

            builder.Update(container.ComponentRegistry);

            ((InMemoryServiceBus)container.Resolve<IServiceBus>()).RegisterHandlersFromAssembly(readModelAssembly);
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
            BsonClassMap.LookupClassMap(typeof(Named));

            var eventStore = Wireup.Init()
                .LogToOutputWindow()
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
