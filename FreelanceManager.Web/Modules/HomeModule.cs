using Nancy;
using Nancy.Security;

namespace FreelanceManager.Web.Modules
{
    public class HomeModule : NancyModule
    {
        public HomeModule()
        {
            this.RequiresAuthentication();

            Get["/"] = _ => View["Index"];
        }
    }
}