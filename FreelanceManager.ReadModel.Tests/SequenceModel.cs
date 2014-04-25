using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FreelanceManager.ReadModel.Repositories;

namespace FreelanceManager.ReadModel
{

    public class SequenceNumberAdded : IEvent
    {
        public SequenceNumberAdded(Guid id, int number)
        {
            Id = id;
            Number = number;
        }

        public int Number { get; set; }

        public Guid Id { get; set; }
        public int Version { get; set; }
    }

    public class Sequence : Model
    {
        public string Result { get; set; }
    }

    public interface ISequenceRepository : IRepository<Sequence> { }


    public class SequenceRepository : Repository<Sequence>, ISequenceRepository
    {
        public SequenceRepository(IMongoContext context, ITenantContext tenantContext)
            : base(context, tenantContext)
        {
        }

        protected override string GetCollectionName()
        {
            return "Sequence";
        }
    }

    public class SequenceAddNumberHandlers : IHandleEvent<SequenceNumberAdded>
    {
        private ISequenceRepository _repository;

        public SequenceAddNumberHandlers(ISequenceRepository repository)
        {
            _repository = repository;
        }

        public void Handle(SequenceNumberAdded @event)
        {
            var model = _repository.GetById(@event.Id);

            if (model == null)
            {
                model = new Sequence
                {
                    Id = @event.Id,
                    Result = @event.Number.ToString()
                };

                _repository.Add(model);
            }
            else
            {
                model.Result += @event.Number.ToString();

                _repository.Update(model, @event.Version);
            }
        }
    }
}
