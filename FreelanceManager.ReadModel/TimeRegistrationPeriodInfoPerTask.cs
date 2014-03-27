using System;
using System.Collections.Generic;
using System.Linq;

namespace FreelanceManager.ReadModel
{
    public class TimeRegistrationPeriodInfoPerTask : Model
    {
        public TimeRegistrationPeriodInfoPerTask()
        {
            Items = new List<Info>();
        }

        public List<Info> Items { get; set; }

        public int Year { get; set; }
        public int Month { get; set; }
        public Guid ClientId { get; set; }
        public string Client { get; set; }
        public Guid ProjectId { get; set; }
        public string Project { get; set; }
        public string Task { get; set; }

        public int Count { get; set; }
        public decimal Income { get; set; }
        public decimal UnbillableHours { get; set; }
        public decimal BillableHours { get; set; }

        public Info Remove(Guid id)
        {
            var toRemove = GetForTimeRegistration(id);
            Items.Remove(toRemove);
            RefreshState();
            return toRemove;
        }

        public bool Contains(Guid id)
        {
            return GetForTimeRegistration(id) != null;
        }

        public void Add(Guid id, Time from, Time to, decimal rate)
        {
            var info = GetForTimeRegistration(id);

            if (info == null)
            {
                info = new Info { Id = id };
                Items.Add(info);
            }

            if (to == null)
                info.Minutes = null;
            else info.Minutes = from.TotalMinutes(to);

            info.Rate = rate;

            RefreshState();
        }

        public void Attach(Guid id, Info attach, Time from, Time to)
        {
            Items.Add(attach);

            if (to == null)
                attach.Minutes = null;
            else attach.Minutes = from.TotalMinutes(to);

            RefreshState();
        }

        public void Correct(Guid id, decimal? amount)
        {
            var info = GetForTimeRegistration(id);
            info.Corrected = amount;

            RefreshState();
        }

        public void RefreshRate(Guid id, decimal rate)
        {
            var info = GetForTimeRegistration(id);
            info.Rate = rate;

            RefreshState();
        }

        private void RefreshState()
        {
            Count = Items.Count;
            Income = Math.Round(Items.Sum(i => i.Minutes.HasValue ? i.Corrected != null ? i.Corrected.Value : (i.Minutes.Value * ((decimal)i.Rate / 60)) : 0), 2);
            BillableHours = Math.Round((decimal)Items.Sum(i => i.Minutes.HasValue && ((i.Corrected != null && i.Corrected.Value > 0) || i.Rate > 0) ? i.Minutes.Value : 0) / 60, 2);
            UnbillableHours = Math.Round((decimal)Items.Sum(i => i.Minutes.HasValue && ((i.Corrected == null || i.Corrected.Value <= 0) && i.Rate <= 0) ? i.Minutes.Value : 0) / 60, 2);
        }

        private Info GetForTimeRegistration(Guid id)
        {
            return Items.FirstOrDefault(i => i.Id == id);
        }

        public class Info
        {
            public Guid Id { get; set; }
            public int? Minutes { get; set; }
            public Money Rate { get; set; }
            public Money Corrected { get; set; }
        }
    }
}