using NodaTime;
using Scheduler;
using Scheduler.ScheduleInstances;
using System.Collections.Generic;

namespace TestData
{
    public class DataRetrieval
    {
        private static Dictionary<string, Scheduler.ScheduleBase> scheduleArchive;
        private static Dictionary<string, IEnumerable<IsoDayOfWeek>> dateTypes;
        private static Dictionary<string, LocalDate> dates;

        public static Dictionary<string, Scheduler.ScheduleBase> ScheduleArchive
        {
            get
            {
                if (scheduleArchive == null)
                {
                    scheduleArchive = new Dictionary<string, Scheduler.ScheduleBase>();

                    scheduleArchive.Add("Schools.Term.Autumn",
                        new ByWeekdays()
                        {
                            DateFrom = Dates["Schools.Term.Autumn.Start"],
                            DateTo = Dates["Schools.Term.Autumn.Start"],
                            Days = DateTypes["Weekends"],
                        }
                    );

                    scheduleArchive.Add("Schools.Term.Autumn.1",
                        new ByWeekdays()
                        {
                            DateFrom = Dates["Schools.Term.Autumn.Start"],
                            DateTo = Dates["Schools.Term.Autumn.HalfTerm.Start"].PlusDays(-1),
                            Days = DateTypes["Weekends"],
                        }
                    );

                    scheduleArchive.Add("Schools.Term.Autumn.HalfTerm",
                        new ByWeekdays()
                        {
                            DateFrom = Dates["Schools.Term.Autumn.HalfTerm.Start"],
                            DateTo = Dates["Schools.Term.Autumn.HalfTerm.End"],
                            Days = DateTypes["Weekends"],
                        }
                    );

                    scheduleArchive.Add("Schools.Term.Autumn.2",
                        new ByWeekdays()
                        {
                            DateFrom = Dates["Schools.Term.Autumn.HalfTerm.End"].PlusDays(01),
                            DateTo = Dates["Schools.Term.Autumn.End"],
                            Days = DateTypes["Weekends"],
                        }
                    );

                    scheduleArchive.Add("BankHolidays.2016.NewYearsDay",            new SingleDay() { Date = DateTimeHelper.GetLocalDate(2016, YearMonth.MonthValue.January, 01) });
                    scheduleArchive.Add("BankHolidays.2016.GoodFriday",             new SingleDay() { Date = DateTimeHelper.GetLocalDate(2016, YearMonth.MonthValue.March, 25) });
                    scheduleArchive.Add("BankHolidays.2016.EasterMonday",           new SingleDay() { Date = DateTimeHelper.GetLocalDate(2016, YearMonth.MonthValue.March, 28) });
                    scheduleArchive.Add("BankHolidays.2016.EarlyMayBankHoliday",    new SingleDay() { Date = DateTimeHelper.GetLocalDate(2016, YearMonth.MonthValue.May, 01) });
                    scheduleArchive.Add("BankHolidays.2016.SpringBankHoliday",      new SingleDay() { Date = DateTimeHelper.GetLocalDate(2016, YearMonth.MonthValue.May, 30) });
                    scheduleArchive.Add("BankHolidays.2016.SummerBankHoliday",      new SingleDay() { Date = DateTimeHelper.GetLocalDate(2016, YearMonth.MonthValue.August, 29) });
                    scheduleArchive.Add("BankHolidays.2016.Boxing",                 new SingleDay() { Date = DateTimeHelper.GetLocalDate(2016, YearMonth.MonthValue.December, 26) });
                    scheduleArchive.Add("BankHolidays.2016.ChristmasDaySubstitute", new SingleDay() { Date = DateTimeHelper.GetLocalDate(2016, YearMonth.MonthValue.December, 27) });
                }

                return scheduleArchive;
            }
        }

        public static Dictionary<string, IEnumerable<IsoDayOfWeek>> DateTypes
        {
            get
            {
                if (dateTypes == null)
                {
                    dateTypes = new Dictionary<string, IEnumerable<IsoDayOfWeek>>();

                    dateTypes.Add("Weekdays", new List<IsoDayOfWeek>() { IsoDayOfWeek.Monday, IsoDayOfWeek.Tuesday, IsoDayOfWeek.Wednesday, IsoDayOfWeek.Thursday, IsoDayOfWeek.Friday });
                    dateTypes.Add("Weekends", new List<IsoDayOfWeek>() { IsoDayOfWeek.Saturday, IsoDayOfWeek.Sunday });
                }

                return dateTypes;
            }
        }

        public static Dictionary<string, LocalDate> Dates
        {
            get
            {
                if (dates == null)
                {
                    dates = new Dictionary<string, LocalDate>();

                    dates.Add("Schools.Term.Autumn.Start", Scheduler.DateTimeHelper.GetLocalDate(2016, Scheduler.YearMonth.MonthValue.October, 05));
                    dates.Add("Schools.Term.Autumn.HalfTerm.Start", Scheduler.DateTimeHelper.GetLocalDate(2016, Scheduler.YearMonth.MonthValue.October, 24));
                    dates.Add("Schools.Term.Autumn.HalfTerm.End", Scheduler.DateTimeHelper.GetLocalDate(2016, Scheduler.YearMonth.MonthValue.October, 31));
                    dates.Add("Schools.Term.Autumn.End", Scheduler.DateTimeHelper.GetLocalDate(2016, Scheduler.YearMonth.MonthValue.December, 21));
                }

                return dates;
            }
        }

    }
}
