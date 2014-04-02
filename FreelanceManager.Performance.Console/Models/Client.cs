using System;
using Fluency;
using Fluency.DataGeneration;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace FreelanceManager.Performance.Console.Models
{
    public class Client
    {
        public static DynamicFluentBuilder<Client> builder = new DynamicFluentBuilder<Client>()
            .For(x => x.Name, ARandom.FullName());

        public Guid Id { get; set; }
        public string Name { get; set; }
    }

    public static class ClientExt
    {
        public static Client CreateClient(this CustomWebClient browser)
        {
            var client = Client.builder.build();
            var r = browser.Post("/write/client/create", JsonConvert.SerializeObject(client));

            var response = (dynamic)JObject.Parse(r);

            client.Id = response.Client.Id;

            return client;
        }
    }
}
