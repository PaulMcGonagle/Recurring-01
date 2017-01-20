using System.Collections.Generic;
using NodaTime;
using Scheduler.Persistance;
using Scheduler.Ranges;
using Scheduler.ScheduleEdges;
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
            private ISerials _sut;
            private IEpisodes _episodes;

            [Fact]
            public void Execute()
            {
                const string timeZoneProvider = "Europe/London";

                this.WithExamples(new ExampleTable("sut", "expectedEpisodes")
                    {
                        {
                            new Serials
                            {
                                new Serial(
                                    schedule: new SingleDay
                                            {
                                                Date = new Date(2016, YearMonth.MonthValue.March, 05),
                                            },
                                    timeRange: new TimeRange(new LocalTime(12, 35), new PeriodBuilder {Hours = 00, Minutes = 30}.Build()),
                                    timeZoneProvider: timeZoneProvider),
                                new Serial(
                                    schedule: new SingleDay
                                        {
                                            Date = new Date(2016, YearMonth.MonthValue.August, 01),
                                        },
                                    timeRange: new TimeRange(new LocalTime(09, 20), new PeriodBuilder {Hours = 20, Minutes = 45}.Build()),
                                    timeZoneProvider: timeZoneProvider),
                            },
                            new Episodes
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
                _episodes = _sut.Episodes;
            }

            public void ThenEpisodesAreThese(Episodes expectedEpisodes)
            {
                _episodes.ShouldBe(expectedEpisodes);
            }
        }

        public class VerifyMissingPropertyThrowsArgumentException
        {
            private Serial _sut;
            private IEpisodes _episodes;
            private System.Exception _exception;

            [Fact]
            public void Execute()
            {
                const string timeZoneProvider = "Europe/London";

                this.WithExamples(new ExampleTable("sut", "parameterName")
                    {
                        {
                            new Serial(
                                schedule: new DateList { Items = new List<Date>(), },
                                timeRange: null,
                                timeZoneProvider: timeZoneProvider),
                            "TimeRange"
                        },
                        {
                            new Serial(
                                schedule: new DateList { Items = new List<Date>(), },
                                timeRange: new TimeRange(new LocalTime(15, 30), null),
                                timeZoneProvider: timeZoneProvider),
                            "Period"
                        },
                        {
                            new Serial(
                                schedule: new DateList { Items = new List<Date>(), },
                                timeRange: new TimeRange(new LocalTime(15, 30), new PeriodBuilder {Hours = 00, Minutes = 30,}.Build()),
                                timeZoneProvider: null),
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
                _exception = Record.Exception(() => { _episodes = _sut.Episodes; });
            }

            public void ThenArgumentExceptionIsThrown(string parameterName)
            {
                _exception.ShouldNotBeNull();

                _exception.Message.ShouldBe(parameterName);
            }
        }
    }
}