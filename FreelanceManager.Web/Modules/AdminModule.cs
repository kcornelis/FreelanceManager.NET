using Nancy;

namespace FreelanceManager.Web.Modules
{
    public class AdminModule : NancyModule
    {
        public AdminModule()
            :base("/admin")
        {
            Get["/accounts"] = _ => View["Accounts"];
        }
    }
}