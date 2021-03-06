﻿using System;
using System.Linq;
using FreelanceManager.ReadModel;
using FreelanceManager.ReadModel.Repositories;
using Nancy.Security;

namespace FreelanceManager.Web.Api.Read
{
    public class TimeRegistrationModule : ApiModule
    {
        public TimeRegistrationModule(ITimeRegistrationRepository timeRegistrationRepository)
            : base("/read/timeregistrations")
        {
            this.RequiresAuthentication();

            Get["/"] = _ => Json(timeRegistrationRepository.ToList());

            Get["/{id:guid}"] = parameters => Json(timeRegistrationRepository.GetById((Guid)parameters.id));

            Get["/getfordate/{date:datetime}"] = parameters => Json(timeRegistrationRepository.GetForPeriod((DateTime)parameters.date, 
                                                                                                            ((DateTime)parameters.date).AddDays(1)));

            Get["/getforperiod/{fromDate}/{toDate}"] = parameters => Json(timeRegistrationRepository.GetForPeriod((DateTime)parameters.fromDate,
                                                                                                                  ((DateTime)parameters.toDate).AddDays(1)));

            Get["/getinfo/{fromDate}/{toDate}"] = parameters =>
            {
                var items = timeRegistrationRepository.GetForPeriod((DateTime)parameters.fromDate, 
                                                                    ((DateTime)parameters.toDate).AddDays(1));

                var summary = new TimeRegistrationPeriodInfo
                {
                    Count = items.Count(),
                    Income = Math.Round(items.Sum(i => i.Minutes.HasValue ? i.CorrectedIncome != null ? i.CorrectedIncome.Value : (i.Minutes.Value * ((decimal)i.Rate / 60)) : 0), 2),
                    BillableMinutes = items.Sum(i => i.Minutes.HasValue && ((i.CorrectedIncome != null && i.CorrectedIncome.Value > 0) || i.Rate > 0) ? i.Minutes.Value : 0),
                    UnbillableMinutes = items.Sum(i => i.Minutes.HasValue && ((i.CorrectedIncome == null || i.CorrectedIncome.Value <= 0) && i.Rate <= 0) ? i.Minutes.Value : 0),
                };

                var perTask = items.GroupBy(r => new { r.ClientId, r.ClientName, r.ProjectId, r.ProjectName, r.Task })
                                   .Select(g =>
                                   {
                                        return new TimeRegistrationPeriodInfoPerTask
                                        {
                                            ClientId = g.Key.ClientId,
                                            Client = g.Key.ClientName,
                                            ProjectId = g.Key.ProjectId,
                                            Project = g.Key.ProjectName,
                                            Task = g.Key.Task,
                                            Count = g.Count(),
                                            Income = Math.Round(g.Sum(i => i.Minutes.HasValue ? i.CorrectedIncome != null ? i.CorrectedIncome.Value : (i.Minutes.Value * ((decimal)i.Rate / 60)) : 0), 2),
                                            BillableMinutes = g.Sum(i => i.Minutes.HasValue && ((i.CorrectedIncome != null && i.CorrectedIncome.Value > 0) || i.Rate > 0) ? i.Minutes.Value : 0),
                                            UnbillableMinutes = g.Sum(i => i.Minutes.HasValue && ((i.CorrectedIncome == null || i.CorrectedIncome.Value <= 0) && i.Rate <= 0) ? i.Minutes.Value : 0),
                                        };
                                   }).ToList();

                return new
                {
                    Summary = summary,
                    PerTask = perTask
                };
            };
        }
    }
}