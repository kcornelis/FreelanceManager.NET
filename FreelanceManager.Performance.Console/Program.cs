using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using FreelanceManager.Performance.Console.Models;
using NLog;

namespace FreelanceManager.Performance.Console
{
    class Program
    {
        static readonly List<Account> _accounts = new List<Account>();
        private static readonly Logger _logger = LogManager.GetLogger("FreelanceManager.Performance");

        static void Main(string[] args)
        {
            ServicePointManager.ServerCertificateValidationCallback += (sender, cert, chain, sslPolicyErrors) => true;

            using (var client = new CustomWebClient(Config.Url))
            {
                client.Authenticate(Config.AdminEmail, Config.AdminPassword);

                for (int i = 0; i < Config.Accounts; i++)
                {
                    var account = client.CreateAccount();

                    _accounts.Add(account);

                    _logger.Info(string.Format("Email: {0}, Password: {1}", account.Email, account.Password));
                }
            }

            Thread.Sleep(5000);

            var workers = _accounts.Select(a =>
            {
                return Task.Factory.StartNew(() => new Worker().Start(a));
            }).ToArray();

            Task.WaitAll(workers);

            _logger.Info("Finished");
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
                List<long> times = new List<long>();
                Stopwatch sw = new Stopwatch();

                for (int day = 1; day <= days; day++)
                {
                    using (var client = new CustomWebClient(Config.Url))
                    {
                        client.Authenticate(_account.Email, _account.Password);

                        var start = new DateTime(year, month, day, 8, 0, 0);
                        for (int i = 0; i < Config.TimeRegistrationsPerDay; i++)
                        {
                            try
                            {
                                var next = start.AddMinutes(interval);
                                var project = _projects[random.Next(0, _projects.Count - 1)];

                                sw.Reset();
                                sw.Start();
                                client.CreateTimeRegistration(project.ClientId, project.Id,
                                    string.Format("{0}-{1}-{2}", year, month, day),
                                    start.ToString("HH:mm"), next.ToString("HH:mm"));
                                sw.Stop();

                                times.Add(sw.ElapsedMilliseconds);

                                start = next;
                            }
                            catch (Exception ex)
                            {
                                _logger.Error(ex);
                                Thread.Sleep(1000);                                
                            }
                        }
                    }
                }

                _logger.Info(string.Format("Generated {0} items for {1}-{2} (min {3}ms) (max {4}ms) (avg {5}ms)", 
                    Config.TimeRegistrationsPerDay * days,
                    month, year,
                    times.Min(), times.Max(), times.Average()));
            }
        }
    }
}
