using System;
using Autofac;
using EventStore;
using NLog;

namespace FreelanceManager.Web.Tools
{
    public class AuthorizationPipelineHook : IPipelineHook
    {
        private static readonly Logger _logger = LogManager.GetLogger(Loggers.Web);
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

                _logger.Warn(string.Format("{0} is trying to access a document with no tenant but is not allowed.", loggedInUser));
                return null;
            }
            else
            {
                if (loggedInUser == tenant)
                    return committed;

                _logger.Warn(string.Format("{0} is trying to access document with tenant id {1} but is not allowed.", loggedInUser, tenant));
                return null;
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