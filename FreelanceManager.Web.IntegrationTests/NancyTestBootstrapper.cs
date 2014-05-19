using System.Configuration;
using Autofac;
using FreelanceManager.Infrastructure;
using FreelanceManager.ReadModel;
using FreelanceManager.Web.Tools;
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
            var setup = new Setup((IContainer)existingContainer);

            // defaults
            setup.WithGuidGenerator();
            setup.WithThreadStaticTenantContext();
            setup.WithMongo("MongoConnectionReadModel");
            setup.RegisterReadModelRepositories();

            // web specific
            var builder = new ContainerBuilder();
            builder.RegisterType<NancyUserMapper>().As<IUserMapper>();
            builder.RegisterType<StaticContentResolverForInMemory>().As<IStaticContentResolver>();
            builder.RegisterType<ExcelService>().As<IExcelService>();
            builder.Update(setup.Container.ComponentRegistry);

            // bus
            setup.WithInMemoryBus();
            setup.RegisterReadModelHandlers();

            // eventstore
            setup.WithMongoEventStore("MongoConnectionEventStore",
                new AuthorizationPipelineHook(setup.Container),
                new MessageDispatcher(setup.Container),
                false);

            // start the bus
            setup.Container.Resolve<IServiceBus>().Start(ConfigurationManager.AppSettings["serviceBusEndpoint"]);
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
    }
}
