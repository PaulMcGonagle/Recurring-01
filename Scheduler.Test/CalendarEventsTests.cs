using System.Collections.Generic;
using NodaTime;
using Scheduler.Calendars;
using Shouldly;
using TestStack.BDDfy;
using Scheduler.ScheduleInstances;
using Xunit;

namespace Scheduler.Test
{
    public class CalendarEventsTests
    {
        public class VerifyDateOutOfBoundsExceptionIsThrown
        {
            private CalendarEvents _sut;
            private IEnumerable<Episode> _dates;

            [Fact]
            public void Execute()
            {
                const string timeZoneProvider = "Europe/London";

                this.WithExamples(new ExampleTable("sut", "expectedTimes")
                    {
                        {
                            new CalendarEvents()
                            {
                                new CalendarEvent
                                {
                                    Schedule = new SingleDay
                                    {
                                        Date = new LocalDate(2016, 03, 05),
                                    },
                                    TimeStart = new LocalTime(12, 35),
                                    TimeZoneProvider = timeZoneProvider,
                                    Period = new PeriodBuilder {Hours = 00, Minutes = 30}.Build(),
                                },
                                new CalendarEvent
                                {
                                    Schedule = new SingleDay
                                    {
                                        Date = new LocalDate(2016, 08, 01),
                                    },
                                    TimeStart = new LocalTime(09, 20),
                                    TimeZoneProvider = timeZoneProvider,
                                    Period = new PeriodBuilder {Hours = 20, Minutes = 45}.Build(),
                                },
                            },
                            new List<Episode>()
                            {
                                new Episode
                                {
                                    From = DateTimeHelper.GetZonedDateTime(new LocalDateTime(2016, 03, 05, 12, 35), timeZoneProvider),
                                    Period = new PeriodBuilder {Hours = 00, Minutes = 30}.Build(),

                                },
                                new Episode
                                {
                                    From = DateTimeHelper.GetZonedDateTime(new LocalDateTime(2016, 08, 01, 09, 20), timeZoneProvider),
                                    Period = new PeriodBuilder {Hours = 20, Minutes = 45}.Build(),
                                },
                            }
                        },
                    })
                    .BDDfy();
            }

            public void GivenACalendarEvent(CalendarEvents sut)
            {
                _sut = sut;
            }

            public void WhenOccurrencesAreRetrieved()
            {
                _dates = _sut.Occurrences();
            }

            public void ThenOccurrencesAreThese(IEnumerable<Episode> expectedTimes)
            {
                _dates.ShouldBe(expectedTimes);
            }
        }

        public class VerifyMissingPropertyThrowsArgumentException
        {
            private CalendarEvent _sut;
            private IEnumerable<Episode> _dates;
            private System.Exception _exception;

            [Fact]
            public void Execute()
            {
                const string timeZoneProvider = "Europe/London";

                this.WithExamples(new ExampleTable("sut", "parameterName")
                    {
                        {
                            new CalendarEvent()
                            {
                                TimeStart = new LocalTime(15, 30),
                                Period = new PeriodBuilder {Hours = 00, Minutes = 30,}.Build(),
                                TimeZoneProvider = timeZoneProvider,
                            },
                            "Schedule"
                        },
                        {
                            new CalendarEvent()
                            {
                                Schedule = new DateList() { Items = new List<LocalDate>(), },
                                Period = new PeriodBuilder {Hours = 00, Minutes = 30,}.Build(),
                                TimeZoneProvider = timeZoneProvider,
                            },
                            "TimeStart"
                        },
                        {
                            new CalendarEvent()
                            {
                                Schedule = new DateList() { Items = new List<LocalDate>(), },
                                TimeStart = new LocalTime(15, 30),
                                TimeZoneProvider = timeZoneProvider,
                            },
                            "Period"
                        },
                        {
                            new CalendarEvent()
                            {
                                Schedule = new DateList() { Items = new List<LocalDate>(), },
                                TimeStart = new LocalTime(15, 30),
                                Period = new PeriodBuilder {Hours = 00, Minutes = 30,}.Build(),
                            },
                            "TimeZoneProvider"
                        },
                    })
                    .BDDfy();
            }

            public void GivenACalendarEvent(CalendarEvent sut)
            {
                _sut = sut;
            }

            public void WhenOccurrencesAreRetrieved()
            {
                _exception = Record.Exception(() => { _dates = _sut.Episodes(); });
            }

            public void ThenArgumentExceptionIsThrown(string parameterName)
            {
                _exception.ShouldNotBeNull();

                _exception.Message.ShouldBe(parameterName);
            }
        }
    }
}