using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FreelanceManager.Events.WorkItem;
using FreelanceManager.ReadModel.Repositories;

namespace FreelanceManager.ReadModel.EventHandlers
{
    public class WorkItemHandlers : IHandleEvent<WorkItemCreated>,
                                    IHandleEvent<WorkItemDetailsChanged>,
                                    IHandleEvent<WorkItemDueDateChanged>,
                                    IHandleEvent<WorkItemDeleted>
    {
        private readonly IWorkItemRepository _workItemRepository;

        public WorkItemHandlers(IWorkItemRepository workItemRepository)
        {
            _workItemRepository = workItemRepository;
        }

        public void Handle(WorkItemCreated @event)
        {
            var workItem = new WorkItem
            {
                Id = @event.Id,
                CreatedOn = @event.CreatedOn,
                Description = @event.Description,
                DueDate = @event.DueDate,
                DueTime = @event.DueTime
            };

            _workItemRepository.Add(workItem, @event.Version);
        }

        public void Handle(WorkItemDetailsChanged @event)
        {
            var workItem = _workItemRepository.GetById(@event.Id);

            if (workItem != null)
            {
                workItem.Description = @event.Description;

                _workItemRepository.Update(workItem, @event.Version);
            }
            else
            {
                throw new ModelNotFoundException();
            }
        }

        public void Handle(WorkItemDueDateChanged @event)
        {
            var workItem = _workItemRepository.GetById(@event.Id);

            if (workItem != null)
            {
                workItem.DueDate = @event.DueDate;
                workItem.DueTime = @event.DueTime;

                _workItemRepository.Update(workItem, @event.Version);
            }
            else
            {
                throw new ModelNotFoundException();
            }
        }

        public void Handle(WorkItemDeleted @event)
        {
            var workItem = _workItemRepository.GetById(@event.Id);

            if (workItem != null)
            {
                _workItemRepository.Delete(workItem, @event.Version);
            }
        }
    }
}
