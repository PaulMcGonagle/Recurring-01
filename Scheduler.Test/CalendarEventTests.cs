using System.Collections.Generic;
using NodaTime;
using Shouldly;
using TestStack.BDDfy;
using Scheduler.ScheduleInstances;
using Xunit;

namespace Scheduler.Test
{
    public class CalendarEventTests
    {
        public class VerifyDateOutOfBoundsExceptionIsThrown
        {
            private CalendarEvent _sut;
            private IEnumerable<Appointment> _occurrences;

            [Fact]
            public void Execute()
            {
                const string timeZoneProvider = "Europe/London";

                this.WithExamples(new ExampleTable("sut", "expectedTimes")
                    {
                        {
                            new CalendarEvent()
                            {
                                Schedule =
                                    new DateList()
                                    {
                                        Dates = new List<LocalDate>()
                                        {
                                            new LocalDate(2016, 01, 05),
                                            new LocalDate(2016, 01, 06),
                                            new LocalDate(2016, 01, 07),
                                        },
                                    },
                                TimeStart = new LocalTime(15, 30),
                                Period = new PeriodBuilder {Hours = 00, Minutes = 30,}.Build(),
                                TimeZoneProvider = timeZoneProvider,
                            },
                            new List<Appointment>()
                            {
                                new Appointment
                                {
                                    From = DateTimeHelper.GetZonedDateTime(new LocalDate(2016, 01, 05), new LocalTime(15, 30), timeZoneProvider),
                                    Period = new PeriodBuilder {Hours = 00, Minutes = 30}.Build(),

                                },
                                new Appointment
                                {
                                    From = DateTimeHelper.GetZonedDateTime(new LocalDate(2016, 01, 06), new LocalTime(15, 30), timeZoneProvider),
                                    Period = new PeriodBuilder {Hours = 00, Minutes = 30}.Build()
                                },
                                new Appointment
                                {
                                    From = DateTimeHelper.GetZonedDateTime(new LocalDate(2016, 01, 07), new LocalTime(15, 30), timeZoneProvider),
                                    Period = new PeriodBuilder {Hours = 00, Minutes = 30}.Build()
                                },
                            }
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
                _occurrences = _sut.Occurrences();
            }

            public void ThenOccurrencesAreThese(IEnumerable<Appointment> expectedTimes)
            {
                _occurrences.ShouldBe(expectedTimes);
            }
        }

        public class VerifyMissingPropertyThrowsArgumentException
        {
            private CalendarEvent _sut;
            private IEnumerable<Appointment> _occurrences;
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
                                Schedule = new DateList() { Dates = new List<LocalDate>(), },
                                Period = new PeriodBuilder {Hours = 00, Minutes = 30,}.Build(),
                                TimeZoneProvider = timeZoneProvider,
                            },
                            "TimeStart"
                        },
                        {
                            new CalendarEvent()
                            {
                                Schedule = new DateList() { Dates = new List<LocalDate>(), },
                                TimeStart = new LocalTime(15, 30),
                                TimeZoneProvider = timeZoneProvider,
                            },
                            "Period"
                        },
                        {
                            new CalendarEvent()
                            {
                                Schedule = new DateList() { Dates = new List<LocalDate>(), },
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
                _exception = Record.Exception(() => { _occurrences = _sut.Occurrences(); });
            }

            public void ThenArgumentExceptionIsThrown(string parameterName)
            {
                _exception.ShouldNotBeNull();

                _exception.Message.ShouldBe(parameterName);
            }
        }
    }
}