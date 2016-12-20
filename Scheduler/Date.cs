using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ArangoDB.Client;
using NodaTime;

namespace Scheduler
{
    public class Date : PersistableEntity, IComparable
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

        public Date PlusDays(int days)
        {
            return new Scheduler.Date(Value.PlusDays(days));
        }

        public Date PlusMonths(int months)
        {
            return new Scheduler.Date(Value.PlusMonths(months));
        }

        public IsoDayOfWeek IsoDayOfWeek => Value.IsoDayOfWeek;

        int IComparable.CompareTo(object obj)
        {
            return Value.CompareTo(((Scheduler.Date)obj).Value);
        }

        public SaveResult Save(IArangoDatabase db)
        {
            return Save<Date>(db);
        }
    }
}
