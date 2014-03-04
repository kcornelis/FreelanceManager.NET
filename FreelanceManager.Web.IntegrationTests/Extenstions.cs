using Nancy.Testing;

namespace FreelanceManager.Web
{
    public static class Extenstions
    {
        public static BrowserResponse AuthenticateWithTestUser(this Browser b)
        {
            return b.Post("/account/login", c =>
            {
                c.FormValue("email", "john@doe.com");
                c.FormValue("password", "john");
            });
        }
    }
}
