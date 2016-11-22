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
        public static IEnumerable<LocalDate> Exclude(this IEnumerable<LocalDate> inputDates, IEnumerable<LocalDate> exclusions)
        {
            return inputDates.Except(exclusions);
        }

        public static IEnumerable<LocalDate> Include(this IEnumerable<LocalDate> inputDates, IEnumerable<LocalDate> exclusions)
        {
            return inputDates.Union(exclusions);
        }

        public static IEnumerable<LocalDate> Exclude(this IEnumerable<LocalDate> inputDates, IEnumerable<Range> exclusions)
        {
            foreach (var exclusion in exclusions)
            {
                inputDates =
                    inputDates
                        .Where(inputDate => !exclusion.From.HasValue || inputDate < exclusion.From.Value
                        || !exclusion.To.HasValue || inputDate > exclusion.To.Value);
            }

            return inputDates;
        }

        public static IEnumerable<LocalDate> Include(this IEnumerable<LocalDate> inputDates, IEnumerable<Range> inclusions)
        {
            foreach (var exclusion in inclusions)
            {
                inputDates =
                    inputDates
                        .Where(inputDate => !exclusion.From.HasValue || inputDate >= exclusion.From.Value
                        || !exclusion.To.HasValue || inputDate <= exclusion.To.Value);
            }

            return inputDates;
        }
    }
}
