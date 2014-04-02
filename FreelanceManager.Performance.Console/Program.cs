using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using FreelanceManager.Performance.Console.Models;

namespace FreelanceManager.Performance.Console
{
    class Program
    {
        static readonly List<Account> _accounts = new List<Account>();

        static void Main(string[] args)
        {
            using (var client = new CustomWebClient(Config.Url))
            {
                client.Authenticate(Config.AdminEmail, Config.AdminPassword);

                for (int i = 0; i < Config.Accounts; i++)
                {
                    _accounts.Add(client.CreateAccount());
                }
            }

            Thread.Sleep(5000);

            var workers = _accounts.Select(a =>
            {
                return Task.Factory.StartNew(() => new Worker().Start(a));
            }).ToArray();

            Task.WaitAll(workers);

            System.Console.WriteLine("Finished");
            System.Console.Read();
        }

        class Worker
        {
            private Account _account;
            private readonly List<Client> _clients = new List<Client>();
            private readonly List<Project> _projects = new List<Project>();

            public void Start(Account account)
            {
                _account = account;

                using (var client = new CustomWebClient(Config.Url))
                {
                    client.Authenticate(_account.Email, _account.Password);

                    for (int i = 0; i < Config.Clients; i++)
                    {
                        var c = client.CreateClient();

                        _clients.Add(c);
                       
                        for (int j = 0; j < Config.Projects; j++)
                        {
                            _projects.Add(client.CreateProject(c.Id));
                        }
                    }
                }

                for (int i = Config.DurationInMonths; i >= 0; i--)
                {
                    CreateTimeRegistrationItemsForMonth(i);
                }
            }

            public void CreateTimeRegistrationItemsForMonth(int monthDelta)
            {
                var year = DateTime.UtcNow.AddMonths(-monthDelta).Year;
                var month = DateTime.UtcNow.AddMonths(-monthDelta).Month;
                var days = DateTime.DaysInMonth(year, month);
                var interval = ((18 - 8) * 60) / Config.TimeRegistrationsPerDay;
                var random = new Random();

                for (int day = 1; day <= days; day++)
                {
                    using (var client = new CustomWebClient(Config.Url))
                    {
                        client.Authenticate(_account.Email, _account.Password);

                        var start = new DateTime(year, month, day, 8, 0, 0);
                        for (int i = 0; i < Config.TimeRegistrationsPerDay; i++)
                        {
                            var next = start.AddMinutes(interval);
                            var project = _projects[random.Next(0, _projects.Count - 1)];

                            client.CreateTimeRegistration(project.ClientId, project.Id,
                                string.Format("{0}-{1}-{2}", year, month, day),
                                start.ToString("HH:mm"), next.ToString("HH:mm"));

                            start = next;
                        }
                    }
                }
            }
        }
    }
}
