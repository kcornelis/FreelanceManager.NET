using Nancy.Testing;
using Newtonsoft.Json.Linq;

namespace FreelanceManager.Web.Tools
{
    public static class Account
    {
        public static dynamic CreateAccount(this Browser browser,
                                      string name = "johny bvba",
                                      string firstName = "johny",
                                      string lastName = "turbo",
                                      string email = "johny@turbo.com")
        {
            return JObject.Parse(browser.Post("/write/admin/account/create", c =>
            {
                c.JsonBody(new
                {
                    Name = name,
                    FirstName = firstName,
                    LastName = lastName,
                    Email = email
                });
            }).Body.AsString());
        }

        public static dynamic ReadAccount(this Browser browser, string id)
        {
            return JObject.Parse(browser.Get("/read/admin/accounts/" + id).Body.AsString());
        }

        public static dynamic ReadAccounts(this Browser browser)
        {
            return JArray.Parse(browser.Get("/read/admin/accounts/").Body.AsString());
        }

        public static dynamic UpdateAccount(this Browser browser, string id,
                                      string name = "jane bvba",
                                      string firstName = "jane",
                                      string lastName = "doe",
                                      string email = "jane@turbo.com")
        {
            return JObject.Parse(browser.Post("/write/admin/account/update/" + id, c =>
            {
                c.JsonBody(new
                {
                    Name = name,
                    FirstName = firstName,
                    LastName = lastName,
                    Email = email
                });
            }).Body.AsString());
        }

        public static void ChangeAccountPassword(this Browser browser, string id, string password = "blabla")
        {
            browser.Post("/write/admin/account/" + id + "/changepassword", c =>
            {
                c.JsonBody(new
                {
                    Password = password
                });
            });
        }
    }
}
