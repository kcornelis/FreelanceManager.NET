using System;

namespace FreelanceManager.Events.Project
{
    public class ProjectUnhidden : Event
    {
        public ProjectUnhidden(Guid id)
        {
            Id = id;
        }
    }
}
