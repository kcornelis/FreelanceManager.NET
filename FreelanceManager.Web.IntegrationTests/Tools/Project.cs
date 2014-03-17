using Nancy.Testing;
using Newtonsoft.Json.Linq;

namespace FreelanceManager.Web.Tools
{
    public static class Project
    {
        public static dynamic CreateProject(this Browser browser, string clientId, 
                                            string name = "windows 8 app",
                                            string description = "a windows 8 app")
        {
            return JObject.Parse(browser.Post("/write/project/create", c =>
            {
                c.JsonBody(new
                {
                    ClientId = clientId,
                    Name = name,
                    Description = description
                });
            }).Body.AsString());
        }

        public static dynamic ReadProject(this Browser browser, string id)
        {
            return JObject.Parse(browser.Get("/read/projects/" + id).Body.AsString());
        }

        public static dynamic ReadProjects(this Browser browser)
        {
            return JArray.Parse(browser.Get("/read/projects").Body.AsString());
        }

        public static dynamic UpdateProject(this Browser browser, string id,
                                            string name = "win8app",
                                            string description = "-",
                                            bool hidden = true)
        {
            return JObject.Parse(browser.Post("/write/project/update/" + id, c =>
            {
                c.JsonBody(new
                {
                    Name = name,
                    Description = description,
                    Hidden = hidden
                });
            }).Body.AsString());
        }

        public static dynamic UpdateProjectTasks(this Browser browser, string id)
        {
            return JObject.Parse(browser.Post("/write/project/updatetasks/" + id, c =>
            {
                c.JsonBody(new
                {
                    Tasks = new [] {
                        new { Name = "Development", Rate = 20M },
                        new { Name = "Blabla", Rate = 10M }
                    }
                });
            }).Body.AsString());
        }
    }
}
