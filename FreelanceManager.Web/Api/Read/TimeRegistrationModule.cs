using System;
using System.Linq;
using FreelanceManager.ReadModel.Repositories;
using Nancy;
using Nancy.Security;

namespace FreelanceManager.Web.Api.Read
{
    public class TimeRegistrationModule : ApiModule
    {
        public TimeRegistrationModule(ITimeRegistrationRepository timeRegistrationRepository,
                                      ITimeRegistrationPeriodInfoRepository timeRegistrationInfoRepository,
                                      ITimeRegistrationPeriodInfoPerTaskRepository timeRegistrationInfoPerTaskRepository)
            : base("/read/timeregistrations")
        {
            this.RequiresAuthentication();

            Get["/"] = _ => Json(timeRegistrationRepository.ToList());

            Get["/{id:guid}"] = parameters => Json(timeRegistrationRepository.GetById((Guid)parameters.id));

            Get["/getfordate/{date:datetime}"] = parameters => {
                var min = ((DateTime)parameters.date).GetNumericValue();
                var max = ((DateTime)parameters.date).AddDays(1).GetNumericValue();

                return Json(timeRegistrationRepository
                    .Where(t => t.Date.Numeric >= min && t.Date.Numeric < max)
                    .OrderBy(t => t.From.Numeric)
                    .ToList());
            };

            Get["/getinfoformonth/{year:int}/{month:int}"] = parameters => Json(timeRegistrationInfoRepository.GetForMonth((int)parameters.year, (int)parameters.month));

            Get["/getinfopertaskformonth/{year:int}/{month:int}"] = parameters => Json(timeRegistrationInfoPerTaskRepository.GetForMonth((int)parameters.year, (int)parameters.month));
        }
    }
}