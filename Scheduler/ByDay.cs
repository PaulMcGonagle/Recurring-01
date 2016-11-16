using NodaTime;
using System.Collections.Generic;

namespace Scheduler
{
    public class ByDay : ScheduleBase
    {

        public LocalDate? DateFrom;
        public LocalDate? DateTo;
        public int? CountPreceding;
        public int? CountFollowing;
        public int IncrementMonths;
        public int Day;
        public LocalDate? SeedDate;

        protected ByDay(int? day, LocalDate? seedDate, int incrementMonths = 1, int? countPreceding = null, int? countFollowing = null)
        {
            Day = day ?? (seedDate.HasValue ? seedDate.Value.Day : SystemClock.Instance.Now.InUtc().Date.Day);
            SeedDate = seedDate;
            IncrementMonths = incrementMonths;
            CountPreceding = countPreceding;
            CountFollowing = countFollowing;
        }

        public override IEnumerable<LocalDate> Occurrences()
        {
            if (DateFrom.HasValue)
                yield return DateFrom.Value; ;

            if (DateTo.HasValue)
                yield return DateTo.Value;
        }
    }
}
