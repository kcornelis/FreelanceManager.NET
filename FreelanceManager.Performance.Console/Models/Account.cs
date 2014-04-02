using Fluency;
using Fluency.DataGeneration;
using FreelanceManager.Performance.Console;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace FreelanceManager.Performance.Console.Models
{
    public class Account
    {
        public static DynamicFluentBuilder<Account> builder = new DynamicFluentBuilder<Account>()
            .For(x => x.Name, ARandom.FullName())
            .For(x => x.FirstName, ARandom.FirstName())
            .For(x => x.LastName, ARandom.LastName())
            .For(x => x.Email, ARandom.Email());

        public string Name { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
    }

    public static class AccountExt
    {
        public static Account CreateAccount(this CustomWebClient browser)
        {
            var account = Account.builder.build();
            var r = browser.Post("/write/admin/account/create", JsonConvert.SerializeObject(account));

            var response = (dynamic)JObject.Parse(r);

            account.Password = response.Password;

            return account;
        }
    }
}
