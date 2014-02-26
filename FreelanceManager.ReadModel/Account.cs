using System;

namespace FreelanceManager.ReadModel
{
    public class Account : Model
    {
        public string Name { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string FullName { get; set; }
        public string Email { get; set; }
        public bool Admin { get; set; }
        public DateTime CreatedOn { get; set; }
    }
}