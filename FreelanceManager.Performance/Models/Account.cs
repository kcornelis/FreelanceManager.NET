using System;
using Fluency;
using Fluency.DataGeneration;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace FreelanceManager.Performance.Models
{
    [Serializable]
    public class Account
    {
        public Account()
        {
            Name = ARandom.FullName();
            FirstName = ARandom.FirstName();
            LastName = ARandom.LastName();
            Email = ARandom.Email();
        }

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
            var account = new Account(); ;
            var r = browser.Post("/write/admin/account/create", JsonConvert.SerializeObject(account));

            var response = (dynamic)JObject.Parse(r);

            account.Password = response.Password;

            return account;
        }
    }
}
