using Nancy.Testing;
using Newtonsoft.Json.Linq;

namespace FreelanceManager.Web.Tools
{
    public static class WorkItem
    {
        public static dynamic CreateWorkItem(this Browser browser, string description = "a test work item", Date dueDate = null, Time dueTime = null)
        {
            var body = browser.Post("/write/workitem/create", c =>
            {
                c.JsonBody(new
                {
                    Description = description,
                    DueDate = dueDate != null ? dueDate.ToString() : "",
                    DueTime = dueTime != null ? dueTime.ToString() : ""
                });
            }).Body.AsString();

            return JObject.Parse(body);
        }

        public static dynamic ReadWorkItem(this Browser browser, string id)
        {
            return JObject.Parse(browser.Get("/read/workitems/" + id).Body.AsString());
        }

        public static dynamic ReadWorkItems(this Browser browser)
        {
            return JArray.Parse(browser.Get("/read/workitems").Body.AsString());
        }

        public static dynamic UpdateWorkItem(this Browser browser, string id, string description = "work item 2", Date dueDate = null, Time dueTime = null)
        {
            return JObject.Parse(browser.Post("/write/workitem/update/" + id, c =>
            {
                c.JsonBody(new
                {
                    Description = description,
                    DueDate = dueDate != null ? dueDate.ToString() : "",
                    DueTime = dueTime != null ? dueTime.ToString() : ""
                });
            }).Body.AsString());
        }
    }
}