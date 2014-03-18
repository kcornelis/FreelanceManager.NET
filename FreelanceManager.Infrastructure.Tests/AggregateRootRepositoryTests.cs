using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using EventStore;
using EventStore.Serialization;
using FreelanceManager.Events.Client;
using MongoDB.Bson.Serialization;
using Xunit;

namespace FreelanceManager.Infrastructure
{
    public class AggregateRootRepositoryTests
    {
        [Fact]
        public void Basic_Save_And_Get_Test()
        {
            var repo = Repository();

            var id = Guid.NewGuid();

            repo.Save(new Client(id, "Johny"));

            Assert.Equal("Johny", repo.GetById<Client>(id).Name);
        }

        [Fact]
        public void An_AggregateRoot_Has_A_Version()
        {
            var repo = Repository();

            var id = Guid.NewGuid();

            repo.Save(new Client(id, "Johny"));

            Assert.Equal(1, repo.GetById<Client>(id).Version);

            var client = repo.GetById<Client>(id);
            client.ChangeDetails("Jane");
            client.ChangeDetails("Johny and Jane");

            repo.Save(client);

            Assert.Equal(3, repo.GetById<Client>(id).Version);
        }

        [Fact]
        public void Concurrency_SameInstance_Test()
        {
            var repo = Repository();

            var id = Guid.NewGuid();

            repo.Save(new Client(id, "Johny"));

            Assert.Equal(1, repo.GetById<Client>(id).Version);

            var client1 = repo.GetById<Client>(id);
            var client2 = repo.GetById<Client>(id);

            client1.ChangeDetails("Jane");
            client2.ChangeDetails("Johny and Jane");

            repo.Save(client1);
            repo.Save(client2);

            Assert.Equal("Johny and Jane", repo.GetById<Client>(id).Name);
            Assert.Equal(3, repo.GetById<Client>(id).Version);
        }

        [Fact]
        public void Concurrency_Other_Store_Instance_Test()
        {
            var repo1 = Repository();
            var repo2 = Repository();

            var id = Guid.NewGuid();

            repo1.Save(new Client(id, "Johny"));

            Assert.Equal(1, repo1.GetById<Client>(id).Version);

            var client1 = repo1.GetById<Client>(id);
            var client2 = repo2.GetById<Client>(id);

            client1.ChangeDetails("Jane");
            client2.ChangeDetails("Johny and Jane");

            repo1.Save(client1);

            Assert.Throws<EventStore.ConcurrencyException>(() =>
            {
                repo2.Save(client2);
            });
        }

        class Client : AggregateRoot
        {
            public Client() { }
            public Client(Guid id, string name)
            {
                ApplyChange(new ClientCreated(id, name, DateTime.Now));
            }

            public string Name { get; private set; }

            public void ChangeDetails(string name)
            {
                if (Name != name)
                {
                    ApplyChange(new ClientDetailsChanged(Id, name));
                }
            }

            public void Apply(ClientCreated e)
            {
                Id = e.Id;
                Name = e.Name;
                CreatedOn = e.CreatedOn;
            }

            public void Apply(ClientDetailsChanged e)
            {
                Name = e.Name;
            }
        }

        public IAggregateRootRepository Repository()
        {
            var types = Assembly.GetAssembly(typeof(FreelanceManager.Events.Event))
                                .GetTypes()
                                .Where(type => type.IsClass && !type.ContainsGenericParameters)
                                .Where(type => type.IsSubclassOf(typeof(FreelanceManager.Events.Event)) ||
                                               type.Namespace.Contains("FreelanceManager.Dtos"));

            foreach (var t in types)
                BsonClassMap.LookupClassMap(t);

            BsonClassMap.LookupClassMap(typeof(Date));
            BsonClassMap.LookupClassMap(typeof(Time));
            BsonClassMap.LookupClassMap(typeof(Money));

            var eventStore = Wireup.Init()
                .LogToOutputWindow()
                .UsingMongoPersistence("MongoConnectionEventStore", new DocumentObjectSerializer())
                .InitializeStorageEngine()
                .Build();

            return new AggregateRootRepository(eventStore, new ThreadStaticTenantContext(), new GuidGenerator());
        }
    }
}
