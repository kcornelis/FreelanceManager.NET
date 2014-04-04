using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Nancy.Security;

namespace FreelanceManager.Web.Tools
{
    public class AuthenticatedUser : IUserIdentity
    {
        public string UserName { get; set; }
        public string Name { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string FullName { get; set; }
        public string Id { get; set; }
        public string Email { get; set; }
        public IEnumerable<string> Claims { get; set; }
    }

    public static class AuthenticatedUserExtensions
    {
        public static AuthenticatedUser AsAuthenticatedUser(this IUserIdentity identity)
        {
            if (identity == null)
                return null;

            return identity as AuthenticatedUser;
        }
    }
}