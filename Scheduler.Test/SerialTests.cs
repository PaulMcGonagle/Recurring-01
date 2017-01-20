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
    public class SerialTests
    {
        public class VerifyDateOutOfBoundsExceptionIsThrown
        {
            private Serial _sut;
            private IEpisodes _episodes;

            [Fact]
            public void Execute()
            {
                const string timeZoneProvider = "Europe/London";

                this.WithExamples(new ExampleTable("sut", "expectedEpisodes")
                    {
                        {
                            new Serial(
                                schedule: new DateList
                                    {
                                        Items = new List<Date>()
                                        {
                                            new Date(2016, YearMonth.MonthValue.January, 05),
                                            new Date(2016, YearMonth.MonthValue.January, 06),
                                            new Date(2016, YearMonth.MonthValue.January, 07),
                                        }
                                    },
                                timeRange: new TimeRange(new LocalTime(15, 30), new PeriodBuilder {Hours = 00, Minutes = 30,}.Build()),
                                timeZoneProvider: timeZoneProvider),
                            new Episodes
                            {
                                new Episode
                                {
                                    From = DateTimeHelper.GetZonedDateTime(new Date(2016, YearMonth.MonthValue.January, 05), new LocalTime(15, 30), timeZoneProvider),
                                    Period = new PeriodBuilder {Hours = 00, Minutes = 30}.Build(),

                                },
                                new Episode
                                {
                                    From = DateTimeHelper.GetZonedDateTime(new Date(2016, YearMonth.MonthValue.January, 06), new LocalTime(15, 30), timeZoneProvider),
                                    Period = new PeriodBuilder {Hours = 00, Minutes = 30}.Build()
                                },
                                new Episode
                                {
                                    From = DateTimeHelper.GetZonedDateTime(new Date(2016, YearMonth.MonthValue.January, 07), new LocalTime(15, 30), timeZoneProvider),
                                    Period = new PeriodBuilder {Hours = 00, Minutes = 30}.Build()
                                },
                            }
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
                _episodes = _sut.Episodes;
            }

            public void ThenEpisodesAreExpected(IEpisodes expectedEpisodes)
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
                                timeRange: new TimeRange(new LocalTime(15, 30), new PeriodBuilder {Minutes = 30,}.Build()),
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