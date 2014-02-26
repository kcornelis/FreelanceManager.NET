using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FreelanceManager.Web.Api.Write.Models.Projects
{
    public class EditableProjectTasks
    {
        public Guid Id { get; set; }

        public IEnumerable<EditableTask> Tasks { get; set; }
    }
}