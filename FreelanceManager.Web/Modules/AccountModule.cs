using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using FreelanceManager.ReadModel;
using FreelanceManager.ReadModel.Repositories;
using FreelanceManager.Tools;
using FreelanceManager.Web.Models.Account;
using Nancy;
using Nancy.Authentication.Forms;
using Nancy.ModelBinding;

namespace FreelanceManager.Web.Modules
{
    public class AccountModule : NancyModule
    {
        public AccountModule(IAccountRepository accountRepository,
                             IAccountPasswordRepository accountPasswordRepository) 
            : base("/account")
        {
            Get["/login"] = parameters => View["Login", new LoginModel { ReturnUrl = parameters.returnUrl }];

            Get["/logout"] = parameters => this.LogoutAndRedirect("/account/login");

            Post["/login"] = parameters =>
            {
                var model = this.BindAndValidate<LoginModel>();
           
                if (ModelValidationResult.IsValid)
                {
                    var account = accountRepository
                        .Where(a => a.Email == model.Email)
                        .FirstOrDefault();

                    if (account != null)
                    {
                        var accountPassword = accountPasswordRepository.GetById(account.Id);

                        if (accountPassword != null && VerifyPassword(accountPassword, model.Password))
                        {
                            
                            if (!string.IsNullOrWhiteSpace(model.ReturnUrl))
                                return this.LoginAndRedirect(account.Id, fallbackRedirectUrl: model.ReturnUrl);

                            return this.LoginAndRedirect(account.Id);
                        }
                    }
                }

                return View["Login", model];
            };
        }

        private bool VerifyPassword(AccountPassword account, string password)
        {
            var givenPassword = new Password(password, account.PasswordSalt);
            return givenPassword.EqualsSaltedHash(account.Password);
        }
    }
}