using System.Collections.Generic;
using System.Linq;
using Scheduler.Ranges;

namespace Scheduler
{
    public static class Extensions
    {
        public static IEnumerable<Date> Exclude(this IEnumerable<Date> inputDates, IEnumerable<Date> exclusions)
        {
            foreach (var inputDate in inputDates)
            {
                var enumerable = exclusions as Date[] ?? exclusions.ToArray();
                if (!enumerable.Select(e => e.Value).Contains(inputDate.Value))
                    yield return inputDate;
            }
        }

        public static IEnumerable<Date> Include(this IEnumerable<Date> inputDates, IEnumerable<Date> exclusions)
        {
            return inputDates.Union(exclusions);
        }

        public static IEnumerable<Date> Exclude(this IEnumerable<Date> inputDates, IEnumerable<DateRange> exclusions)
        {
            foreach (var exclusion in exclusions)
            {
                inputDates =
                    inputDates
                        .Where(inputDate => exclusion.From == null || inputDate.Value < exclusion.From.Date.Value
                        || exclusion.To == null || inputDate.Value > exclusion.To.Date.Value);
            }

            return inputDates;
        }

        public static IEnumerable<Date> Include(this IEnumerable<Date> inputDates, IEnumerable<DateRange> inclusions)
        {
            foreach (var exclusion in inclusions)
            {
                inputDates =
                    inputDates
                        .Where(inputDate => exclusion.From == null || inputDate.Value >= exclusion.From.Date.Value
                        || exclusion.To == null || inputDate.Value <= exclusion.To.Date.Value);
            }

            return inputDates;
        }
    }
}
