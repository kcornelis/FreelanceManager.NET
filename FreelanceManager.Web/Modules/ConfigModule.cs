using Nancy;
using Nancy.Security;

namespace FreelanceManager.Web.Modules
{
    public class ConfigModule : NancyModule
    {
        public ConfigModule() 
            : base("/config")
        {
            this.RequiresAuthentication();

            Get["/"] = _ => Response.AsRedirect("/config/clients");

            Get["/clients"] = _ => View["Clients"];
        }
    }
}