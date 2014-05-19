using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autofac;

namespace FreelanceManager
{
    public class Setup : IDisposable
    {
        private ILifetimeScope _container;

       public Setup()
           :this(new ContainerBuilder().Build())
       {
       }

       public Setup(ILifetimeScope container)
       {
           _container = container;

           // register the container
           var builder = new ContainerBuilder();
           builder.RegisterInstance<ILifetimeScope>(_container).ExternallyOwned();
           builder.Update(_container.ComponentRegistry);
       }

       public ILifetimeScope Container
        {
            get { return _container; }
        }

        public void Dispose()
        {
            if (_container != null)
                _container.Dispose();
        }
    }
}
