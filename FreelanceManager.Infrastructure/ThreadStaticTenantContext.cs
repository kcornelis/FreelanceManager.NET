using System;

namespace FreelanceManager.Infrastructure
{
    public class ThreadStaticTenantContext : ITenantContext
    {
        [ThreadStatic]
        private static string _tenant;

        [ThreadStatic]
        private static bool _admin;

        public string GetTenantId()
        {
            return _tenant ?? "";
        }

        public bool IsTenantAdmin()
        {
            return _admin;
        }

        public void SetTenantId(string tenant, bool isAdmin = false)
        {
            _tenant = tenant;
            _admin = isAdmin;
        }
    }
}