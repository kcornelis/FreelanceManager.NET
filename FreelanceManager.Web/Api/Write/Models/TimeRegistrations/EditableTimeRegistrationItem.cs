using System;
using System.Linq;

namespace FreelanceManager.Web.Api.Write.Models.TimeRegistrations
{
    public class EditableTimeRegistration
    {
        public EditableTimeRegistration() { }
        public EditableTimeRegistration(Domain.TimeRegistration timeRegistration,
            Domain.Client client, Domain.Project project)
        {
            Populate(timeRegistration, client, project);
        }

        public Guid ClientId { get; set; }
        public Guid ProjectId { get; set; }
        public string ClientName { get; set; }
        public string ProjectName { get; set; }
        public string Task { get; set; }
        public bool RefreshRate { get; set; }
        public string Description { get; set; }
        public string Date { get; set; }
        public string From { get; set; }
        public string To { get; set; }
        public decimal? CorrectedIncome { get; set; }
        public string CorrectedIncomeMessage { get; set; }

        public void Populate(Domain.TimeRegistration timeRegistration,
            Domain.Client client, Domain.Project project)
        {
            ClientId = client.Id;
            ClientName = client.Name;
            ProjectId = project.Id;
            ProjectName = project.Name;
            Task = timeRegistration.Task;
            Description = timeRegistration.Description;
            Date = timeRegistration.Date.ToString();
            From = timeRegistration.From.ToString();
            To = timeRegistration.To != null ? timeRegistration.To.ToString() : null;
            CorrectedIncome = timeRegistration.CorrectedIncome != null ? (decimal?)timeRegistration.CorrectedIncome.Total : (decimal?)null;
            CorrectedIncomeMessage = timeRegistration.CorrectedIncome != null ? timeRegistration.CorrectedIncome.Message : null;
        }
    }
}