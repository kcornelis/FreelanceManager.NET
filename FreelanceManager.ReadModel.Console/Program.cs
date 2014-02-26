using System;
using System.Collections.Generic;
using System.Configuration;
using Autofac;
using FreelanceManager.Bus;
using FreelanceManager.Infrastructure;

namespace FreelanceManager.ReadModel.Console
{
    class Program
    {
        static void Main(string[] args)
        {
            var builder = new ContainerBuilder();

            builder.RegisterType<ThreadStaticTenantContext>().As<ITenantContext>();
            builder.RegisterType<MongoContext>().As<IMongoContext>().SingleInstance().WithParameter("url", ConfigurationManager.ConnectionStrings["MongoConnectionReadModel"].ConnectionString);

            var readModelAssembly = typeof(FreelanceManager.ReadModel.Account).Assembly;
            builder.RegisterAssemblyTypes(readModelAssembly)
                   .Where(t => t.Name.EndsWith("Repository"))
                   .AsImplementedInterfaces();

            var container = builder.Build();

            var serviceBus = new MsmqServiceBus(container);
            serviceBus.Start(ConfigurationManager.AppSettings["BusEndpoint"], readModelAssembly);

            System.Console.Read();

            //todo: stop bus
        }
    }
}
