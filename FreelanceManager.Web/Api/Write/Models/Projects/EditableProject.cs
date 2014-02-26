using System;
using System.Collections.Generic;

namespace FreelanceManager.Web.Api.Write.Models.Projects
{
    public class EditableProject
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public Guid ClientId { get; set; }
        public string ClientName { get; set; }
        public string Description { get; set; }
        public bool Hidden { get; set; }
    }
}