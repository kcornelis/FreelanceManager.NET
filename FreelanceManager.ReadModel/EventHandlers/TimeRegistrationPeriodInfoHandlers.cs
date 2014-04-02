using FreelanceManager.Events.TimeRegistration;
using FreelanceManager.ReadModel.Repositories;

namespace FreelanceManager.ReadModel.EventHandlers
{
    public class TimeRegistrationPeriodInfoHandlers : IHandleEvent<TimeRegistrationCreated>,
                                                      IHandleEvent<TimeRegistrationDetailsChanged>,
                                                      IHandleEvent<TimeRegistrationCorrectedIncomeCleared>,
                                                      IHandleEvent<TimeRegistrationIncomeCorrected>,
                                                      IHandleEvent<TimeRegistrationRateRefreshed>,
                                                      IHandleEvent<TimeRegistrationDeleted>
    {
        // TODO fix this, this doesn't make sense if the readmodel is handled on 2 servers
        //  - or create a group by query on the time registration period model
        //  - or create a new Report/Statistics service (with sql database)
        private readonly static object _lock = new object();

        private readonly ITimeRegistrationPeriodInfoRepository _timeRegistrationPeriodInfoRepository;
        private readonly ITimeRegistrationRepository _timeRegistrationRepository;
        private readonly ITenantContext _tenantContext;

        public TimeRegistrationPeriodInfoHandlers(ITimeRegistrationPeriodInfoRepository timeRegistrationPeriodInfoRepository,
            ITimeRegistrationRepository timeRegistrationRepository, ITenantContext tenantContext)
        {
            _timeRegistrationPeriodInfoRepository = timeRegistrationPeriodInfoRepository;
            _timeRegistrationRepository = timeRegistrationRepository;
            _tenantContext = tenantContext;
        }

        public void Handle(TimeRegistrationCreated @event)
        {
            lock (_lock)
            {
                var update = true;
                var tenant = _tenantContext.GetTenantId();

                var info = _timeRegistrationPeriodInfoRepository.GetForMonth(@event.Date.Year, @event.Date.Month);

                if (info == null)
                {
                    update = false;
                    info = new TimeRegistrationPeriodInfo
                    {
                        Tenant = tenant,
                        Year = @event.Date.Year,
                        Month = @event.Date.Month
                    };
                }

                info.Add(@event.Id, @event.From, @event.To, @event.Rate);

                if (update)
                    _timeRegistrationPeriodInfoRepository.Update(info);
                else
                    _timeRegistrationPeriodInfoRepository.Add(info);
            }
        }

        public void Handle(TimeRegistrationDetailsChanged @event)
        {
            lock (_lock)
            {
                var info = _timeRegistrationPeriodInfoRepository.GetForMonth(@event.Date.Year, @event.Date.Month);

                info.Update(@event.Id, @event.From, @event.To);

                _timeRegistrationPeriodInfoRepository.Update(info);
            }
        }

        public void Handle(TimeRegistrationCorrectedIncomeCleared @event)
        {
            lock (_lock)
            {
                var timeRegistration = _timeRegistrationRepository.GetById(@event.Id);

                var info = _timeRegistrationPeriodInfoRepository.GetForMonth(timeRegistration.Date.Year, timeRegistration.Date.Month);

                info.Correct(@event.Id, null);

                _timeRegistrationPeriodInfoRepository.Update(info);
            }
        }

        public void Handle(TimeRegistrationIncomeCorrected @event)
        {
            lock (_lock)
            {
                var timeRegistration = _timeRegistrationRepository.GetById(@event.Id);

                var info = _timeRegistrationPeriodInfoRepository.GetForMonth(timeRegistration.Date.Year, timeRegistration.Date.Month);

                info.Correct(@event.Id, @event.Amount);

                _timeRegistrationPeriodInfoRepository.Update(info);
            }
        }

        public void Handle(TimeRegistrationRateRefreshed @event)
        {
            lock (_lock)
            {
                var timeRegistration = _timeRegistrationRepository.GetById(@event.Id);

                var info = _timeRegistrationPeriodInfoRepository.GetForMonth(timeRegistration.Date.Year, timeRegistration.Date.Month);

                info.RefreshRate(@event.Id, @event.Rate);

                _timeRegistrationPeriodInfoRepository.Update(info);
            }
        }

        public void Handle(TimeRegistrationDeleted @event)
        {
            lock (_lock)
            {
                var info = _timeRegistrationPeriodInfoRepository.GetForTimeRegistration(@event.Id);

                info.Remove(@event.Id);

                _timeRegistrationPeriodInfoRepository.Update(info);
            }
        }
    }
}
