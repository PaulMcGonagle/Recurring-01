﻿using System;
using System.Runtime.Serialization;
using ArangoDB.Client;
using NodaTime;
using Scheduler.Persistance;

namespace Scheduler
{
    public class Date : Vertex, IComparable
    {
        public Date(int year, YearMonth.MonthValue month, int day)
        {
            Value = new LocalDate(year, (int)month, day);
        }

        public Date(LocalDate d)
        {
            Value = d;
        }

        public LocalDate Value { get; set; }

        [IgnoreDataMember]
        public IsoDayOfWeek IsoDayOfWeek => Value.IsoDayOfWeek;

        public Date PlusDays(int days)
        {
            return new Date(Value.PlusDays(days));
        }

        public Date PlusMonths(int months)
        {
            return new Date(Value.PlusMonths(months));
        }

        int IComparable.CompareTo(object obj)
        {
            return Value.CompareTo(((Date)obj).Value);
        }

        public override string ToString()
        {
            return Value.ToString();
        }

        public override SaveResult Save(IArangoDatabase db, IClock clock)
        {
            return Save(new Func<SaveResult>[]
            {
                () => Save<Date>(db),
                () => base.Save(db, clock),
            });
        }
    }
}
