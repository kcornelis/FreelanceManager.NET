using System;
using System.Configuration;
using Autofac;
using FreelanceManager.Infrastructure;
using Nancy.Testing;

namespace FreelanceManager.Web
{
    public static class Context
    {
        static NancyTestBootstrapper _bootstrapper;

        static Context()
        {
            // drop old databases

            new MongoContext(ConfigurationManager.ConnectionStrings["MongoConnectionReadModel"].ConnectionString)
                .GetDatabase().Drop();

            new MongoContext(ConfigurationManager.ConnectionStrings["MongoConnectionEventStore"].ConnectionString)
                .GetDatabase().Drop();

            _bootstrapper = new NancyTestBootstrapper();
            new Browser(_bootstrapper);

            using (var container = _bootstrapper.CreateContainer())
            {
                var account = new Domain.Account(Guid.NewGuid(),
                    "John Doe BVBA", "John", "Doe", "john@doe.com");

                account.ChangePassword("john");

                container.Resolve<IAggregateRootRepository>().Save(account);
            }
        }

        public static Browser CreateBrowser()
        {
            return new Browser(_bootstrapper);
        }

        public static Browser CreateBrowserAndAuthenticate()
        {
            return new Browser(_bootstrapper).AuthenticateWithTestUser().Then;
        }
    }
}
