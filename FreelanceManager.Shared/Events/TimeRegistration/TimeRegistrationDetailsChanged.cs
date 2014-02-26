using System;

namespace FreelanceManager.Events.TimeRegistration
{
    public class TimeRegistrationDetailsChanged : Event
    {
        public TimeRegistrationDetailsChanged(Guid id,
            Guid clientId, Guid projectId, string task,
            string description, Date date, Time from, Time to)
        {
            Id = id;
            ClientId = clientId;
            ProjectId = projectId;
            Task = task;
            Description = description;
            Date = date;
            From = from;
            To = to;
        }

        public Guid ClientId { get; private set; }
        public Guid ProjectId { get; private set; }
        public string Task { get; private set; }
        public string Description { get; private set; }
        public Date Date { get; private set; }
        public Time From { get; private set; }
        public Time To { get; private set; }
    }
}
