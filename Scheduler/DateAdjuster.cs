using System;
using NodaTime;

namespace Scheduler
{
    public class DateAdjuster
    {
        public static LocalDate Adjust(LocalDate input, string adjuster)
        {
            if (adjuster.Length < 3)
                throw new ArgumentException($"adjuster with invalid length '{adjuster}'");

            var right = adjuster.Substring(adjuster.Length - 1);
            var left = adjuster.Substring(0, adjuster.Length - 1);
            int offset;
            
            if (int.TryParse(left, out offset))
                throw new ArgumentException($"adjuster with invalid offset '{adjuster}'");

            switch (right)
            {
                case "d":
                    return input.PlusDays(offset);

                case "w":
                    return input.PlusWeeks(offset);

                case "m":
                    return input.PlusMonths(offset);

                case "y":
                    return input.PlusYears(offset);
            }

            throw new ArgumentException($"invalid adjuster '{adjuster}'");
        }
    }
}
