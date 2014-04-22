using System;
using System.Collections.Generic;
using System.Linq;

namespace FreelanceManager.ReadModel
{
    public class TimeRegistrationPeriodInfo : Model
    {
        public TimeRegistrationPeriodInfo()
        {
            Items = new List<Info>();
        }

        public List<Info> Items { get; set; }

        public int Year { get; set; }
        public int Month { get; set; }

        public int Count { get; set; }
        public decimal Income { get; set; }
        public int UnbillableMinutes { get; set; }
        public int BillableMinutes { get; set; }

        public void Add(Guid id, Time from, Time to, decimal rate) 
        {
            Info info = GetForTimeRegistration(id);

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

        public void Update(Guid id, Time from, Time to)
        {
            Info info = GetForTimeRegistration(id);

            if (info == null)
            {
                info = new Info { Id = id };
                Items.Add(info);
            }

            if (to == null)
                info.Minutes = null;
            else info.Minutes = from.TotalMinutes(to);

            RefreshState();
        }

        public void Correct(Guid id, decimal? amount)
        {
            var info = GetForTimeRegistration(id);
            info.Corrected = amount;

            RefreshState();
        }

        public void Remove(Guid id)
        {
            Items.Remove(GetForTimeRegistration(id));
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
            BillableMinutes = Items.Sum(i => i.Minutes.HasValue && ((i.Corrected != null && i.Corrected.Value > 0) || i.Rate > 0) ? i.Minutes.Value : 0);
            UnbillableMinutes = Items.Sum(i => i.Minutes.HasValue && ((i.Corrected == null || i.Corrected.Value <= 0) && i.Rate <= 0) ? i.Minutes.Value : 0);
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