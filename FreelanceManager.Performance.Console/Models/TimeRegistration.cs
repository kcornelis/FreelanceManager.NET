using System;
using Fluency.DataGeneration;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace FreelanceManager.Performance.Console.Models
{
    public class TimeRegistration
    {
        public TimeRegistration(Guid clientId, Guid projectId,
                                string date, string from, string to)
        {
            ClientId = clientId;
            ProjectId = projectId;
            Date = date;
            From = from;
            To = to;

            Task = "Development";
            Description = ARandom.String(40);
            CorrectedIncome = 50;
            CorrectedIncomeMessage = ARandom.String(40);
        }

        public Guid ClientId { get; set; }
        public Guid ProjectId { get; set; }
        public string Task { get; set; }
        public string From { get; set; }
        public string To { get; set; }
        public string Date { get; set; }
        public string Description { get; set; }
        public decimal? CorrectedIncome { get; set; }
        public string CorrectedIncomeMessage { get; set; }
    }

    public static class TimeRegistrationExt
    {
        public static TimeRegistration CreateTimeRegistration(this CustomWebClient browser, Guid clientId, Guid projectId,
                                                              string date, string from, string to)
        {
            var timeRegistration = new TimeRegistration(clientId, projectId, date, from, to);

            var r = browser.Post("/write/timeregistration/create", JsonConvert.SerializeObject(timeRegistration));

            var response = (dynamic)JObject.Parse(r);

            return timeRegistration;
        }
    }
}
