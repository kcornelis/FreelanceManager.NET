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
                var accountAdmin = new Domain.Account(Guid.NewGuid(),
                    "Jane Doe BVBA", "Jane", "Doe", "jane@doe.com");

                account.ChangePassword("john");
                accountAdmin.ChangePassword("jane");
                accountAdmin.MakeAdmin();

                container.Resolve<IAggregateRootRepository>().Save(account);
                container.Resolve<IAggregateRootRepository>().Save(accountAdmin);
            }
        }

        public static Browser CreateBrowser()
        {
            return new Browser(_bootstrapper);
        }

        public static Browser CreateBrowserAndAuthenticate()
        {
            return new Browser(_bootstrapper).Post("/account/login", c =>
            {
                c.FormValue("email", "john@doe.com");
                c.FormValue("password", "john");
            }).Then;
        }

        public static Browser CreateBrowserAndAuthenticateWithAdmin()
        {
            return new Browser(_bootstrapper).Post("/account/login", c =>
            {
                c.FormValue("email", "jane@doe.com");
                c.FormValue("password", "jane");
            }).Then;
        }
    }
}
