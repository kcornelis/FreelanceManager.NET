using Nancy.Testing;
using Newtonsoft.Json.Linq;

namespace FreelanceManager.Web.Tools
{
    public static class Client
    {
        public static dynamic CreateClient(this Browser browser, string name = "johny bvba")
        {
            var body = browser.Post("/write/client/create", c =>
            {
                c.JsonBody(new
                {
                    Name = name
                });
            }).Body.AsString();

            return JObject.Parse(body);
        }

        public static dynamic ReadClient(this Browser browser, string id)
        {
            return JObject.Parse(browser.Get("/read/clients/" + id).Body.AsString());
        }

        public static dynamic ReadClients(this Browser browser)
        {
            return JArray.Parse(browser.Get("/read/clients").Body.AsString());
        }

        public static dynamic UpdateClient(this Browser browser, string id, string name = "jane bvba")
        {
            return JObject.Parse(browser.Post("/write/client/update/" + id, c =>
            {
                c.JsonBody(new
                {
                    Name = name
                });
            }).Body.AsString());
        }
    }
}
