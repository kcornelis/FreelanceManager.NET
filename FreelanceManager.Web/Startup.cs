using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(FreelanceManager.Web.Startup))]
namespace FreelanceManager.Web
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
            app.UseNancy();
        }
    }
}
