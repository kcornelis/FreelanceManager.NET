using System;

namespace FreelanceManager.Events.TimeRegistration
{
    public class TimeRegistrationCreated : Event
    {
        public TimeRegistrationCreated(Guid id,
            Guid clientId, Guid projectId, string task, Money taskRate,
            string description, Date date, Time from, Time to, DateTime createdOn)
        {
            Id = id;
            ClientId = clientId;
            ProjectId = projectId;
            Task = task;
            Rate = taskRate;
            Description = description;
            Date = date;
            From = from;
            To = to;
            CreatedOn = createdOn;
        }

        public Guid ClientId { get; private set; }
        public Guid ProjectId { get; private set; }
        public string Task { get; private set; }
        public Money Rate { get; private set; }
        public string Description { get; private set; }
        public Date Date { get; private set; }
        public Time From { get; private set; }
        public Time To { get; private set; }
        public DateTime CreatedOn { get; private set; }
    }
}
