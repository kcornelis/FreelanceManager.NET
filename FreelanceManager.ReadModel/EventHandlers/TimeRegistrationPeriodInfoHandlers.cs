using FreelanceManager.Events.TimeRegistration;
using FreelanceManager.ReadModel.Repositories;

namespace FreelanceManager.ReadModel.EventHandlers
{
    public class TimeRegistrationPeriodInfoHandlers /*: IHandleEvent<TimeRegistrationCreated>,
                                                      IHandleEvent<TimeRegistrationDetailsChanged>,
                                                      IHandleEvent<TimeRegistrationCorrectedIncomeCleared>,
                                                      IHandleEvent<TimeRegistrationIncomeCorrected>,
                                                      IHandleEvent<TimeRegistrationRateRefreshed>,
                                                      IHandleEvent<TimeRegistrationDeleted>*/
    {
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

        public void Handle(TimeRegistrationDetailsChanged @event)
        {
            var info = _timeRegistrationPeriodInfoRepository.GetForMonth(@event.Date.Year, @event.Date.Month);

            info.Update(@event.Id, @event.From, @event.To);

            _timeRegistrationPeriodInfoRepository.Update(info);
        }

        public void Handle(TimeRegistrationCorrectedIncomeCleared @event)
        {
            var timeRegistration = _timeRegistrationRepository.GetById(@event.Id);

            var info = _timeRegistrationPeriodInfoRepository.GetForMonth(timeRegistration.Date.Year, timeRegistration.Date.Month);

            info.Correct(@event.Id, null);

            _timeRegistrationPeriodInfoRepository.Update(info);
        }

        public void Handle(TimeRegistrationIncomeCorrected @event)
        {
            var timeRegistration = _timeRegistrationRepository.GetById(@event.Id);

            var info = _timeRegistrationPeriodInfoRepository.GetForMonth(timeRegistration.Date.Year, timeRegistration.Date.Month);

            info.Correct(@event.Id, @event.Amount);

            _timeRegistrationPeriodInfoRepository.Update(info);
        }

        public void Handle(TimeRegistrationRateRefreshed @event)
        {
            var timeRegistration = _timeRegistrationRepository.GetById(@event.Id);

            var info = _timeRegistrationPeriodInfoRepository.GetForMonth(timeRegistration.Date.Year, timeRegistration.Date.Month);

            info.RefreshRate(@event.Id, @event.Rate);

            _timeRegistrationPeriodInfoRepository.Update(info);
        }

        public void Handle(TimeRegistrationDeleted @event)
        {
            var info = _timeRegistrationPeriodInfoRepository.GetForTimeRegistration(@event.Id);

            info.Remove(@event.Id);

            _timeRegistrationPeriodInfoRepository.Update(info);
        }
    }
}
