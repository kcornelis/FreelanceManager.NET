using System;

namespace FreelanceManager.ReadModel
{
    public class TimeRegistration : Model
    {
        public Guid ClientId { get; set; }
        public Guid ProjectId { get; set; }
        public string ClientName { get; set; }
        public string ProjectName { get; set; }
        public string Task { get; set; }
        public Money Rate { get; set; }
        public string Description { get; set; }
        public Money Income { get; set; }
        public Date Date { get; set; }
        public Time From { get; set; }
        public int? Minutes { get; set; }
        public Time To { get; set; }
        public DateTime CreatedOn { get; set; }
        public Money CorrectedIncome { get; set; }
        public string CorrectedIncomeMessage { get; set; }
    }
}