using System;

namespace FreelanceManager.Events.Project
{
    public class ProjectHidden : Event
    {
        public ProjectHidden(Guid id)
        {
            Id = id;
        }
    }
}
