using System;
using FreelanceManager.Events.TimeRegistration;
using FreelanceManager.Domain.ValueObjects;

namespace FreelanceManager.Domain
{
    public class TimeRegistration : AggregateRoot
    {
        public TimeRegistration() { }
        public TimeRegistration(Guid id, Client client, Project project, Task task,
                                string description, Date date, Time from, Time to)
        {
            ApplyChange(new TimeRegistrationCreated(id, client.Id, project.Id, 
                                                    task.Name, task.Rate,
                                                    description, date, from, to, 
                                                    DateTime.UtcNow));
        }

        public Guid ClientId { get; private set; }
        public Guid ProjectId { get; private set; }
        public string Task { get; private set; }
        public Money Rate { get; private set; }
        public string Description { get; private set; }
        public Date Date { get; private set; }
        public Time From { get; private set; }
        public Time To { get; private set; }
        public CorrectedIncome CorrectedIncome { get; private set; }
        public bool Deleted { get; private set; }
        public DateTime? DeletedOn { get; private set; }

        public bool Billable
        {
            get { return Income > 0; }
        }

        public int TotalMinutes
        {
            get
            {
                return Deleted ? 0 : From.TotalMinutes(To);
            }
        }

        public Money Income
        {
            get
            {
                return Deleted ? 0 : (CorrectedIncome != null ? CorrectedIncome.Total : (((decimal)TotalMinutes * Rate) / 60));
            }
        }

        public void CorrectIncome(decimal amount, string message)
        {
            if (Deleted)
                throw new AggregateDeletedException();

            if (CorrectedIncome == null ||
                CorrectedIncome.Total != amount ||
                CorrectedIncome.Message != message)
            {
                ApplyChange(new TimeRegistrationIncomeCorrected(Id, amount, message));
            }
        }

        public void ClearCorrectedIncome()
        {
            if (Deleted)
                throw new AggregateDeletedException();

            if (CorrectedIncome != null)
            {
                ApplyChange(new TimeRegistrationCorrectedIncomeCleared(Id));
            }
        }

        public void ChangeDetails(Client client, Project project, Task task,
                                  string description, Date date, Time from, Time to)
        {
            if (Deleted)
                throw new AggregateDeletedException();

            if (ClientId != client.Id || ProjectId != project.Id ||
                Task != task.Name || Description != description ||
                Date != date || From != from || To != to)
            {
                ApplyChange(new TimeRegistrationDetailsChanged(Id, client.Id, project.Id,
                                                               task.Name, description, date, from, to));
            }
        }

        public void RefreshRate(Task task)
        {
            if (Deleted)
                throw new AggregateDeletedException();

            if (Rate != task.Rate)
            {
                ApplyChange(new TimeRegistrationRateRefreshed(Id, task.Rate));
            }
        }

        public void Delete()
        {
            if (Deleted)
                throw new AggregateDeletedException();


            ApplyChange(new TimeRegistrationDeleted(Id, DateTime.UtcNow));
        }

        public void Apply(TimeRegistrationCreated e)
        {
            Id = e.Id;
            ClientId = e.ClientId;
            ProjectId = e.ProjectId;
            Description = e.Description;
            Task = e.Task;
            Rate = e.Rate;
            Date = e.Date;
            From = e.From;
            To = e.To;
            CreatedOn = e.CreatedOn;
        }

        public void Apply(TimeRegistrationDetailsChanged e)
        {
            ClientId = e.ClientId;
            ProjectId = e.ProjectId;
            Description = e.Description;
            Task = e.Task;
            Date = e.Date;
            From = e.From;
            To = e.To;
        }

        public void Apply(TimeRegistrationIncomeCorrected e)
        {
            CorrectedIncome = new CorrectedIncome(e.Amount, e.Message);
        }

        public void Apply(TimeRegistrationCorrectedIncomeCleared e)
        {
            CorrectedIncome = null;
        }

        public void Apply(TimeRegistrationRateRefreshed e)
        {
            Rate = e.Rate;
        }

        public void Apply(TimeRegistrationDeleted e)
        {
            Deleted = true;
            DeletedOn = e.DeletedOn;
        }
    }
}