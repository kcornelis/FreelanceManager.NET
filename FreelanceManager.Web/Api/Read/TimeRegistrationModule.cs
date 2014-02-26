using System;
using System.Linq;
using FreelanceManager.ReadModel.Repositories;
using Nancy;
using Nancy.Security;

namespace FreelanceManager.Web.Areas.Read
{
    public class TimeRegistrationModule : NancyModule
    {
        public TimeRegistrationModule(ITimeRegistrationRepository timeRegistrationRepository,
                                      ITimeRegistrationPeriodInfoRepository timeRegistrationInfoRepository,
                                      ITimeRegistrationPeriodInfoPerTaskRepository timeRegistrationInfoPerTaskRepository)
            : base("/read/timeregistrations")
        {
            this.RequiresAuthentication();

            Get["/"] = _ => Response.AsJson(timeRegistrationRepository.ToList());

            Get["/{id:guid}"] = parameters => Response.AsJson(timeRegistrationRepository.GetById((Guid)parameters.id));

            Get["/getfordate/{date:datetime}"] = parameters => {
                var min = ((DateTime)parameters.date).GetNumericValue();
                var max = ((DateTime)parameters.date).AddDays(1).GetNumericValue();

                return Response.AsJson(timeRegistrationRepository
                    .Where(t => t.Date.Numeric >= min && t.Date.Numeric < max)
                    .OrderBy(t => t.From.Numeric)
                    .ToList());
            };

            Get["/getinfoformonth/{year:int}/{month:int}"] = parameters => Response.AsJson(timeRegistrationInfoRepository.GetForMonth((int)parameters.year, (int)parameters.month));

            Get["/getinfopertaskformonth/{year:int}/{month:int}"] = parameters => Response.AsJson(timeRegistrationInfoPerTaskRepository.GetForMonth((int)parameters.year, (int)parameters.month));
        }
    }
}