using System.Collections.Generic;
using System.Linq;
using Scheduler.Ranges;

namespace Scheduler
{
    public static class Extensions
    {
        public static IEnumerable<IDate> Exclude(this IEnumerable<IDate> inputDates, IEnumerable<IDate> exclusions)
        {
            var exclusionsArray = exclusions.ToArray();

            foreach (var inputDate in inputDates)
            {
                var enumerable = exclusions as IDate[] ?? exclusionsArray.ToArray();
                if (!enumerable.Select(e => e.Value).Contains(inputDate.Value))
                    yield return inputDate;
            }
        }

        public static IEnumerable<IDate> Include(this IEnumerable<IDate> inputDates, IEnumerable<IDate> exclusions)
        {
            return inputDates.Union(exclusions);
        }

        public static IEnumerable<IDate> Exclude(this IEnumerable<IDate> inputDates, IEnumerable<RangeDate> exclusions)
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

        public static IEnumerable<IDate> Include(this IEnumerable<IDate> inputDates, IEnumerable<RangeDate> inclusions)
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
