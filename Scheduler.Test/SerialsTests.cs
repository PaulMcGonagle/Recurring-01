using System.Collections.Generic;
using NodaTime;
using Shouldly;
using TestStack.BDDfy;
using Scheduler.ScheduleInstances;
using Xunit;

namespace Scheduler.Test
{
    public class SerialsTests
    {
        public class VerifyDateOutOfBoundsExceptionIsThrown
        {
            private Serials _sut;
            private IEnumerable<Episode> _episodes;

            [Fact]
            public void Execute()
            {
                const string timeZoneProvider = "Europe/London";

                this.WithExamples(new ExampleTable("sut", "expectedEpisodes")
                    {
                        {
                            new Serials
                            {
                                new Serial
                                {
                                    Schedule = new SingleDay
                                    {
                                        Date = DateTimeHelper.GetLocalDate(2016, YearMonth.MonthValue.March, 05),
                                    },
                                    TimeStart = new LocalTime(12, 35),
                                    TimeZoneProvider = timeZoneProvider,
                                    Period = new PeriodBuilder {Hours = 00, Minutes = 30}.Build(),
                                },
                                new Serial
                                {
                                    Schedule = new SingleDay
                                    {
                                        Date = DateTimeHelper.GetLocalDate(2016, YearMonth.MonthValue.August, 01),
                                    },
                                    TimeStart = new LocalTime(09, 20),
                                    TimeZoneProvider = timeZoneProvider,
                                    Period = new PeriodBuilder {Hours = 20, Minutes = 45}.Build(),
                                },
                            },
                            new List<Episode>
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

            public void GivenACalendarEvent(Serials sut)
            {
                _sut = sut;
            }

            public void WhenEpisodesAreRetrieved()
            {
                _episodes = _sut.Episodes();
            }

            public void ThenEpisodesAreThese(IEnumerable<Episode> expectedEpisodes)
            {
                _episodes.ShouldBe(expectedEpisodes);
            }
        }

        public class VerifyMissingPropertyThrowsArgumentException
        {
            private Serial _sut;
            private IEnumerable<Episode> _episodes;
            private System.Exception _exception;

            [Fact]
            public void Execute()
            {
                const string timeZoneProvider = "Europe/London";

                this.WithExamples(new ExampleTable("sut", "parameterName")
                    {
                        {
                            new Serial
                            {
                                TimeStart = new LocalTime(15, 30),
                                Period = new PeriodBuilder {Hours = 00, Minutes = 30,}.Build(),
                                TimeZoneProvider = timeZoneProvider,
                            },
                            "Schedule"
                        },
                        {
                            new Serial
                            {
                                Schedule = new DateList { Items = new List<LocalDate>(), },
                                Period = new PeriodBuilder {Hours = 00, Minutes = 30,}.Build(),
                                TimeZoneProvider = timeZoneProvider,
                            },
                            "TimeStart"
                        },
                        {
                            new Serial
                            {
                                Schedule = new DateList { Items = new List<LocalDate>(), },
                                TimeStart = new LocalTime(15, 30),
                                TimeZoneProvider = timeZoneProvider,
                            },
                            "Period"
                        },
                        {
                            new Serial
                            {
                                Schedule = new DateList { Items = new List<LocalDate>(), },
                                TimeStart = new LocalTime(15, 30),
                                Period = new PeriodBuilder {Hours = 00, Minutes = 30,}.Build(),
                            },
                            "TimeZoneProvider"
                        },
                    })
                    .BDDfy();
            }

            public void GivenACalendarEvent(Serial sut)
            {
                _sut = sut;
            }

            public void WhenEpisodesAreRetrieved()
            {
                _exception = Record.Exception(() => { _episodes = _sut.Episodes(); });
            }

            public void ThenArgumentExceptionIsThrown(string parameterName)
            {
                _exception.ShouldNotBeNull();

                _exception.Message.ShouldBe(parameterName);
            }
        }
    }
}