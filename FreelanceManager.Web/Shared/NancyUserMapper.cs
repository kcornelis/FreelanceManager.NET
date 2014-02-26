using System;
using System.Collections.Generic;
using FreelanceManager.ReadModel.Repositories;
using Nancy;
using Nancy.Authentication.Forms;
using Nancy.Security;

namespace FreelanceManager.Web.Shared
{
    public class NancyUserMapper : IUserMapper
    {
        private readonly IAccountRepository _repository;

        public NancyUserMapper(IAccountRepository repository)
        {
            _repository = repository;
        }

        public IUserIdentity GetUserFromIdentifier(Guid identifier, NancyContext context)
        {
            var account = _repository.GetById(identifier);

            if (account == null)
                return null;

            return new AuthenticatedUser
            {
                UserName = account.FullName,
                Name = account.Name,
                FirstName = account.FirstName,
                LastName = account.LastName,
                FullName = account.FullName,
                Id = account.Id.ToString(),
                Email = account.Email,
                Claims = new[]
                {
                    account.Admin ? "Admin" : "User"
                }
            };
        }
    }
}