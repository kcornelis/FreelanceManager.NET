using Nancy.Testing;
using Newtonsoft.Json.Linq;

namespace FreelanceManager.Web.Tools
{
    public static class TimeRegistration
    {
        public static dynamic CreateTimeRegistration(this Browser browser, string clientId, string projectId, string task = "Development",
                                                     string description = "worked a bit", bool refreshRate = false,
                                                     string date = "2014-02-20", string from = "10:00", string to = "12:00", 
                                                     decimal? correctedIncome = null, string correctedIncomeMessage = null)
        {
            return JObject.Parse(browser.Post("/write/timeregistration/create", c =>
            {
                c.JsonBody(new
                {
                    ClientId = clientId,
                    ProjectId = projectId,
                    Task = task,
                    Description = description,
                    RefreshRate = refreshRate,
                    Date = date,
                    From = from,
                    To = to,
                    CorrectedIncome = correctedIncome,
                    CorrectedIncomeMessage = correctedIncomeMessage
                });
            }).Body.AsString());
        }

        public static void DeleteTimeRegistration(this Browser browser, string id)
        {
            browser.Post("/write/timeregistration/delete/" + id);
        }

        public static dynamic ReadTimeRegistration(this Browser browser, string id)
        {
            return JObject.Parse(browser.Get("/read/timeregistrations/" + id).Body.AsString());
        }

        public static dynamic ReadTimeRegistrations(this Browser browser)
        {
            return JArray.Parse(browser.Get("/read/timeregistrations").Body.AsString());
        }

        public static dynamic UpdateTimeRegistration(this Browser browser, string id, string clientId, string projectId,
                                                     string task = "Meeting",
                                                     string description = "worked some more", bool refreshRate = false,
                                                     string date = "2014-02-20", string from = "12:00", string to = "13:00",
                                                     decimal? correctedIncome = 50, string correctedIncomeMessage = "override rate")
        {
            return JObject.Parse(browser.Post("/write/timeregistration/update/" + id, c =>
            {
                c.JsonBody(new
                {
                    ClientId = clientId,
                    ProjectId = projectId,
                    Task = task,
                    Description = description,
                    RefreshRate = refreshRate,
                    Date = date,
                    From = from,
                    To = to,
                    CorrectedIncome = correctedIncome,
                    CorrectedIncomeMessage = correctedIncomeMessage
                });
            }).Body.AsString());
        }
    }
}
