namespace FreelanceManager
{
    public interface ITenantContext
    {
        string GetTenantId();
        bool IsTenantAdmin();
        void SetTenantId(string tenant, bool admin);
    }
}
