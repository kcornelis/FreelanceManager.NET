using System;

namespace FreelanceManager.Web.Api.Write.Models.TimeRegistrations
{
    public class TimeRegistration
    {
        public TimeRegistration() { }
        public TimeRegistration(Domain.TimeRegistration timeRegistration,
            Domain.Client client, Domain.Project project)
        {
            Populate(timeRegistration, client, project);
        }

        public Guid Id { get; set; }
        public Guid ClientId { get; set; }
        public Guid ProjectId { get; set; }
        public string ClientName { get; set; }
        public string ProjectName { get; set; }
        public string Task { get; set; }
        public Money Rate { get; set; }
        public string Description { get; set; }
        public Date Date { get; set; }
        public Time From { get; set; }
        public Time To { get; set; }
        public DateTime CreatedOn { get; set; }
        public Money CorrectedIncome { get; set; }
        public string CorrectedIncomeMessage { get; set; }

        public void Populate(Domain.TimeRegistration timeRegistration,
            Domain.Client client, Domain.Project project)
        {
            Id = timeRegistration.Id;
            ClientId = client.Id;
            ClientName = client.Name;
            ProjectId = project.Id;
            ProjectName = project.Name;
            Task = timeRegistration.Task;
            Rate = timeRegistration.Rate;
            Description = timeRegistration.Description;
            Date = timeRegistration.Date;
            From = timeRegistration.From;
            To = timeRegistration.To;
            CreatedOn = timeRegistration.CreatedOn;
            CorrectedIncome = timeRegistration.CorrectedIncome != null ? timeRegistration.CorrectedIncome.Total : null;
            CorrectedIncomeMessage = timeRegistration.CorrectedIncome != null ? timeRegistration.CorrectedIncome.Message : null;
        }
    }
}