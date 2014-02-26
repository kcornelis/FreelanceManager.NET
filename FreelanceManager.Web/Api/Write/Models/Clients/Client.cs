using System;
using System.Linq;
using System.Collections.Generic;

namespace FreelanceManager.Web.Api.Write.Models.Clients
{
    public class Client
    {
        public Client() { }
        public Client(Domain.Client client)
        {
            Populate(client);
        }

        public Guid Id { get; set; }
        public string Name { get; set; }
        public DateTime CreatedOn { get; set; }

        public void Populate(Domain.Client client)
        {
            Id = client.Id;
            Name = client.Name;
            CreatedOn = client.CreatedOn;
        }
    }
}