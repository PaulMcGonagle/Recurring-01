using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NodaTime;

namespace Scheduler
{
    public static class Extensions
    {
        public static IEnumerable<Scheduler.Date> Exclude(this IEnumerable<Scheduler.Date> inputDates, IEnumerable<Scheduler.Date> exclusions)
        {
            foreach (var inputDate in inputDates)
            {
                var enumerable = exclusions as Date[] ?? exclusions.ToArray();
                if (!enumerable.Select(e => e.Value).Contains(inputDate.Value))
                    yield return inputDate;
            }
        }

        public static IEnumerable<Scheduler.Date> Include(this IEnumerable<Scheduler.Date> inputDates, IEnumerable<Scheduler.Date> exclusions)
        {
            return inputDates.Union(exclusions);
        }

        public static IEnumerable<Scheduler.Date> Exclude(this IEnumerable<Scheduler.Date> inputDates, IEnumerable<Range> exclusions)
        {
            foreach (var exclusion in exclusions)
            {
                inputDates =
                    inputDates
                        .Where(inputDate => exclusion.From == null || inputDate.Value < exclusion.From.Value
                        || exclusion.To == null || inputDate.Value > exclusion.To.Value);
            }

            return inputDates;
        }

        public static IEnumerable<Scheduler.Date> Include(this IEnumerable<Scheduler.Date> inputDates, IEnumerable<Range> inclusions)
        {
            foreach (var exclusion in inclusions)
            {
                inputDates =
                    inputDates
                        .Where(inputDate => exclusion.From == null || inputDate.Value >= exclusion.From.Value
                        || exclusion.To == null || inputDate.Value <= exclusion.To.Value);
            }

            return inputDates;
        }
    }
}
