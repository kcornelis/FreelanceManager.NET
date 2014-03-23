using Nancy;

namespace FreelanceManager.Web.Api
{
    public class ApiModule : NancyModule
    {
        public ApiModule(string path)
            : base(path)
        {

        }

        protected Response Json<T>(T model)
        {
            return Response.AsJson<T>(model);
        }
    }
}