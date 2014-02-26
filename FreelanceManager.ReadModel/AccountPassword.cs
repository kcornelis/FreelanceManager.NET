using System;

namespace FreelanceManager.ReadModel
{
    public class AccountPassword : Model
    {
        public string Password { get; set; }
        public int PasswordSalt { get; set; }
    }
}
