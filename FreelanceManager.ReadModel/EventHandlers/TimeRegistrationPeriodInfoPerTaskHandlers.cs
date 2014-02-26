using System.Linq;
using FreelanceManager.Events.Client;
using FreelanceManager.Events.Project;
using FreelanceManager.Events.TimeRegistration;
using FreelanceManager.ReadModel.Repositories;

namespace FreelanceManager.ReadModel.EventHandlers
{
    public class TimeRegistrationPeriodInfoPerTaskHandlers : IHandleEvent<TimeRegistrationCreated>,
                                                             IHandleEvent<TimeRegistrationDetailsChanged>,
                                                             IHandleEvent<TimeRegistrationCorrectedIncomeCleared>,
                                                             IHandleEvent<TimeRegistrationIncomeCorrected>,
                                                             IHandleEvent<TimeRegistrationRateRefreshed>,
                                                             IHandleEvent<ClientDetailsChanged>,
                                                             IHandleEvent<ProjectDetailsChanged>,
                                                             IHandleEvent<TimeRegistrationDeleted>
    {
        private readonly ITimeRegistrationPeriodInfoPerTaskRepository _timeRegistrationPeriodInfoPerTaskRepository;
        private readonly ITimeRegistrationRepository _timeRegistrationRepository;
        private readonly IProjectRepository _projectRepository;
        private readonly IClientRepository _clientRepository;
        private readonly ITenantContext _tenantContext;

        public TimeRegistrationPeriodInfoPerTaskHandlers(ITimeRegistrationPeriodInfoPerTaskRepository timeRegistrationPeriodInfoPerTaskRepository,
            ITimeRegistrationRepository timeRegistrationRepository, ITenantContext tenantContext,
            IProjectRepository projectRepository, IClientRepository clientRepository)
        {
            _timeRegistrationPeriodInfoPerTaskRepository = timeRegistrationPeriodInfoPerTaskRepository;
            _timeRegistrationRepository = timeRegistrationRepository;
            _projectRepository = projectRepository;
            _clientRepository = clientRepository;
            _tenantContext = tenantContext;
        }

        public void Handle(TimeRegistrationCreated @event)
        {
            var update = true;
            var tenant = _tenantContext.GetTenantId();
            var client = _clientRepository.GetById(@event.ClientId);
            var project = _projectRepository.GetById(@event.ProjectId);

            var info = _timeRegistrationPeriodInfoPerTaskRepository.GetForProjectAndMonth(@event.ProjectId, @event.Task, @event.Date.Year, @event.Date.Month);

            if (info == null)
            {
                update = false;
                info = new TimeRegistrationPeriodInfoPerTask
                {
                    Tenant = tenant,
                    Year = @event.Date.Year,
                    Month = @event.Date.Month,
                    ClientId = @event.ClientId,
                    ProjectId = @event.ProjectId,
                    Task = @event.Task,
                    Client = client.Name,
                    Project = project.Name
                };
            }

            info.Add(@event.Id, @event.From, @event.To, @event.Rate);

            if (update)
                _timeRegistrationPeriodInfoPerTaskRepository.Update(info);
            else
                _timeRegistrationPeriodInfoPerTaskRepository.Add(info);
        }

        public void Handle(TimeRegistrationDetailsChanged @event)
        {
            var allInfosForMonth = _timeRegistrationPeriodInfoPerTaskRepository.GetForMonth(@event.Date.Year, @event.Date.Month);
            var oldContainer = allInfosForMonth.FirstOrDefault(i => i.Contains(@event.Id));
            TimeRegistrationPeriodInfoPerTask.Info oldInfo = null;

            if (oldContainer == null)
                throw new ModelNotFoundException();

            oldInfo = oldContainer.Remove(@event.Id);

            _timeRegistrationPeriodInfoPerTaskRepository.Update(oldContainer);

            if (oldContainer.Count == 0)
            {
                _timeRegistrationPeriodInfoPerTaskRepository.Delete(oldContainer);
            }
            
            var newContainer = allInfosForMonth.Where(i => i.ProjectId == @event.ProjectId && i.Task == @event.Task).FirstOrDefault();

            var update = true;
            if (newContainer == null)
            {
                update = false;
                var tenant = _tenantContext.GetTenantId();
                var client = _clientRepository.GetById(@event.ClientId);
                var project = _projectRepository.GetById(@event.ProjectId);
                newContainer = new TimeRegistrationPeriodInfoPerTask
                {
                    Tenant = tenant,
                    Year = @event.Date.Year,
                    Month = @event.Date.Month,
                    ClientId = @event.ClientId,
                    ProjectId = @event.ProjectId,
                    Task = @event.Task,
                    Client = client.Name,
                    Project = project.Name
                };
            }

            newContainer.Attach(@event.Id, oldInfo, @event.From, @event.To);

            if (update)
                _timeRegistrationPeriodInfoPerTaskRepository.Update(newContainer);
            else
                _timeRegistrationPeriodInfoPerTaskRepository.Add(newContainer);
        }

        public void Handle(TimeRegistrationCorrectedIncomeCleared @event)
        {
            var timeRegistration = _timeRegistrationRepository.GetById(@event.Id);

            var info = _timeRegistrationPeriodInfoPerTaskRepository.GetForProjectAndMonth(timeRegistration.ProjectId, timeRegistration.Task, timeRegistration.Date.Year, timeRegistration.Date.Month);

            info.Correct(@event.Id, null);

            _timeRegistrationPeriodInfoPerTaskRepository.Update(info);
        }

        public void Handle(TimeRegistrationIncomeCorrected @event)
        {
            var timeRegistration = _timeRegistrationRepository.GetById(@event.Id);

            var info = _timeRegistrationPeriodInfoPerTaskRepository.GetForProjectAndMonth(timeRegistration.ProjectId, timeRegistration.Task, timeRegistration.Date.Year, timeRegistration.Date.Month);

            info.Correct(@event.Id, @event.Amount);

            _timeRegistrationPeriodInfoPerTaskRepository.Update(info);
        }

        public void Handle(TimeRegistrationRateRefreshed @event)
        {
            var timeRegistration = _timeRegistrationRepository.GetById(@event.Id);

            var info = _timeRegistrationPeriodInfoPerTaskRepository.GetForProjectAndMonth(timeRegistration.ProjectId, timeRegistration.Task, timeRegistration.Date.Year, timeRegistration.Date.Month);

            info.RefreshRate(@event.Id, @event.Rate);

            _timeRegistrationPeriodInfoPerTaskRepository.Update(info);
        }

        public void Handle(ClientDetailsChanged @event)
        {
            var timeRegistrationInfos = _timeRegistrationPeriodInfoPerTaskRepository.FindForClient(@event.Id);

            foreach (var timeRegistrationInfo in timeRegistrationInfos)
            {
                timeRegistrationInfo.Client = @event.Name;

                _timeRegistrationPeriodInfoPerTaskRepository.Update(timeRegistrationInfo);
            }
        }

        public void Handle(ProjectDetailsChanged @event)
        {
            var timeRegistrationInfos = _timeRegistrationPeriodInfoPerTaskRepository.FindForProject(@event.Id);

            foreach (var timeRegistrationInfo in timeRegistrationInfos)
            {
                timeRegistrationInfo.Project = @event.Name;

                _timeRegistrationPeriodInfoPerTaskRepository.Update(timeRegistrationInfo);
            }
        }

        public void Handle(TimeRegistrationDeleted @event)
        {
            var info = _timeRegistrationPeriodInfoPerTaskRepository.GetForTimeRegistration(@event.Id);

            info.Remove(@event.Id);

            if (info.Count == 0)
            {
                _timeRegistrationPeriodInfoPerTaskRepository.Delete(info);
            }
            else
            {
                _timeRegistrationPeriodInfoPerTaskRepository.Update(info);
            }
        }
    }
}
