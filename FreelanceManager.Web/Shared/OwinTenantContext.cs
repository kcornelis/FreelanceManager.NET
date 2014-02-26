using System.Security;
using System.Web;

namespace FreelanceManager.Web.Shared
{
    //public class OwinTenantContext : ITenantContext
    //{
    //    public string GetTenantId()
    //    {
    //        var account = HttpContext.Current.GetOwinContext().Authentication.User;

    //        if (account == null)
    //            return "";

    //        if (!account.Identity.IsAuthenticated)
    //            return "";

    //        return account.FindFirst("Id").Value;
    //    }

    //    public void SetTenantId(string tenant)
    //    {
    //        throw new System.NotImplementedException();
    //    }
    //}
}