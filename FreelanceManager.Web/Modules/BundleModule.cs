using System.IO;
using System.Text;
using FreelanceManager.Web.Tools;
using Nancy;
using SquishIt.Framework;

namespace FreelanceManager.Web.Modules
{
    public class BundleModule : NancyModule
    {
        private static bool _initialized;
        private static object _lock = new object();

        public BundleModule(IStaticContentResolver staticContentResolver)
            : base("/bundles")
        {
            Initialize(staticContentResolver);

            Get["/js/{name}"] = parameters => CreateResponse(
                Bundle.JavaScript().RenderCached((string)parameters.name), 
                Configuration.Instance.JavascriptMimeType);

            Get["/viewmodels/{name}"] = parameters => CreateResponse(
                Bundle.JavaScript().RenderCached((string)parameters.name),
                Configuration.Instance.JavascriptMimeType);

            Get["/css/{name}"] = parameters => CreateResponse(
                Bundle.Css().RenderCached((string)parameters.name),
                Configuration.Instance.CssMimeType);
        }

        private Response CreateResponse(string content, string contentType)
        {
            return Response.FromStream(() => new MemoryStream(Encoding.UTF8.GetBytes(content)), contentType)
                           .WithHeader("Cache-Control", "max-age=45");
        }

        private static void Initialize(IStaticContentResolver staticContentResolver)
        {
            if (_initialized)
                return;

            lock (_lock)
            {
                if (_initialized)
                    return;

                Bundle.JavaScript().Add(staticContentResolver.GetLocation("/scripts/jquery-2.1.0.js"))
                   .Add(staticContentResolver.GetLocation("/scripts/jquery.validate.js"))
                   .ForceRelease()
                   .AsCached("jquery", "~/bundles/js/jquery");

                Bundle.JavaScript().Add(staticContentResolver.GetLocation("/scripts/modernizr-2.7.2.js"))
                                   .ForceRelease()
                                   .AsCached("modernizr", "~/bundles/js/modernizr");

                Bundle.JavaScript().Add(staticContentResolver.GetLocation("/Scripts/bootstrap.js"))
                                   .Add(staticContentResolver.GetLocation("/Scripts/underscore-min.js"))
                                   .Add(staticContentResolver.GetLocation("/Scripts/bootstrap-select.js"))
                                   .Add(staticContentResolver.GetLocation("/Scripts/bootstrap-datepicker.js"))
                                   .Add(staticContentResolver.GetLocation("/Scripts/respond.js"))
                                   .Add(staticContentResolver.GetLocation("/Scripts/knockout-3.1.0.js"))
                                   .Add(staticContentResolver.GetLocation("/Scripts/knockout.validation.js"))
                                   .Add(staticContentResolver.GetLocation("/Scripts/moment.js"))
                                   .AddMinified(staticContentResolver.GetLocation("/Scripts/freelancemanager.js"))
                                   .AddMinifiedDirectory(staticContentResolver.GetLocation("/ViewModels/"), false)
                                   .ForceRelease()
                                   .AsCached("common", "~/bundles/js/common");

                Bundle.JavaScript().Add(staticContentResolver.GetLocation("/scripts/globalize/globalize.js"))
                                   .Add(staticContentResolver.GetLocation("/scripts/dx.chartjs.js"))
                                   .ForceRelease()
                                   .AsCached("charts", "~/bundles/js/charts");

                Bundle.JavaScript().AddMinifiedDirectory(staticContentResolver.GetLocation("/ViewModels/TimeRegistration"), false)
                                   .ForceRelease()
                                   .AsCached("timeregistration", "~/bundles/viewmodels/timeregistration");

                Bundle.JavaScript().AddMinifiedDirectory(staticContentResolver.GetLocation("/ViewModels/Config"), false)
                                   .ForceRelease()
                                   .AsCached("config", "~/bundles/viewmodels/config");

                Bundle.JavaScript().AddMinifiedDirectory(staticContentResolver.GetLocation("/ViewModels/Admin"), false)
                                   .ForceRelease()
                                   .AsCached("admin", "~/bundles/viewmodels/admin");


                Bundle.Css().Add(staticContentResolver.GetLocation("/Content/normalize.css"))
                            .Add(staticContentResolver.GetLocation("/Content/bootstrap.css"))
                            .Add(staticContentResolver.GetLocation("/Content/bootstrap-select.css"))
                            .Add(staticContentResolver.GetLocation("/Content/bootstrap-datepicker.css"))
                            .Add(staticContentResolver.GetLocation("/Content/font-awesome.css"))
                            .Add(staticContentResolver.GetLocation("/Content/site.css"))
                            .ForceRelease()
                            .AsCached("all", "~/bundles/css/all");

                _initialized = true;
            }
        }
    }
}