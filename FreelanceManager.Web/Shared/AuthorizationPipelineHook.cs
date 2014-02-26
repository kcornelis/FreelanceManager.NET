using System;
using System.Web;
using Autofac;
using EventStore;

namespace FreelanceManager.Web.Shared
{
    public class AuthorizationPipelineHook : IPipelineHook
    {
        private readonly ILifetimeScope _container;

        public AuthorizationPipelineHook(ILifetimeScope container)
        {
            _container = container;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        public Commit Select(Commit committed)
        {
            // return null if the user isn't authorized to see this commit

            var loggedInUser = _container.Resolve<ITenantContext>().GetTenantId();
            var loggedInUserIsAdmin = _container.Resolve<ITenantContext>().IsTenantAdmin();

            if (string.IsNullOrWhiteSpace(loggedInUser))
                return null;

            var tenant = committed.Headers.ContainsKey("Tenant") ?
                committed.Headers["Tenant"].ToString() : null;

            if(string.IsNullOrEmpty(tenant))
            {
                if (loggedInUserIsAdmin)
                    return committed;
                return null;
            }
            else
            {
                if (loggedInUser == tenant)
                    return committed;
                else return null;
            }
        }

        public void PostCommit(Commit committed)
        {
            // anything to do after the commit has been persisted.
        }

        protected virtual void Dispose(bool disposing)
        {
            // no op
        }

        public bool PreCommit(Commit attempt)
        {
            // Can easily do logging or other such activities here
            return true; // true == allow commit to continue, false = stop.
        }
    }
}