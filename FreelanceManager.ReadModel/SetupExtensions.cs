using Autofac;
using FreelanceManager.ReadModel.Tools;

namespace FreelanceManager.ReadModel
{
    public static class SetupExtensions
    {
        public static Setup RegisterReadModelServices(this Setup setup)
        {
            var builder = new ContainerBuilder();

            builder.RegisterType<DomainUpdateServiceBusHandlerHook>().As<IDomainUpdateServiceBusHandlerHook>();

            builder.Update(setup.Container.ComponentRegistry);

            return setup;
        }

        public static Setup RegisterReadModelRepositories(this Setup setup)
        {
            var builder = new ContainerBuilder();

            var readModelAssembly = typeof(Account).Assembly;

            builder.RegisterAssemblyTypes(readModelAssembly)
                   .Where(t => t.Name.EndsWith("Repository"))
                   .AsImplementedInterfaces();

            builder.Update(setup.Container.ComponentRegistry);

            return setup;
        }

        public static Setup RegisterReadModelHandlers(this Setup setup)
        {
            var readModelAssembly = typeof(Account).Assembly;
            var bus = setup.Container.Resolve<IServiceBus>();

            bus.RegisterHandlers(readModelAssembly);

            return setup;
        }
    }
}
