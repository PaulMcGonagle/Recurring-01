using System.Collections.Generic;
using System.Linq;
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
            private IClock _clock;
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
                                    rangeTime: new EdgeRangeTime(new LocalTime(12, 35), new PeriodBuilder {Hours = 00, Minutes = 30}.Build()),
                                    timeZoneProvider: timeZoneProvider),
                                new Serial(
                                    schedule: new SingleDay
                                        {
                                            Date = new Date(2016, YearMonth.MonthValue.August, 01),
                                        },
                                    rangeTime: new EdgeRangeTime(new LocalTime(09, 20), new PeriodBuilder {Hours = 20, Minutes = 45}.Build()),
                                    timeZoneProvider: timeZoneProvider),
                            },
                            new Episodes
                            {
                                new Episode
                                {
                                    Start = DateTimeHelper.GetZonedDateTime(new LocalDateTime(2016, 03, 05, 12, 35), timeZoneProvider),
                                    Period = new PeriodBuilder {Hours = 00, Minutes = 30}.Build(),

                                },
                                new Episode
                                {
                                    Start = DateTimeHelper.GetZonedDateTime(new LocalDateTime(2016, 08, 01, 09, 20), timeZoneProvider),
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
                _episodes = _sut.GenerateEpisodes(_clock);
            }

            public void ThenEpisodesAreThese(Episodes expectedEpisodes)
            {
                _episodes.ShouldBe(expectedEpisodes);
            }
        }

        public class VerifyMissingPropertyThrowsArgumentException
        {
            private Serial _sut;
            private IClock _clock;
            private IEdgeVertexs<IEpisode> _episodes;
            private System.Exception _exception;

            [Fact]
            public void Execute()
            {
                const string timeZoneProvider = "Europe/London";
                var fakeClock = ScheduleTestHelper.GetFakeClock(2016, YearMonth.MonthValue.March, 15);

                this.WithExamples(new ExampleTable("sut", "clock", "parameterName")
                    {
                        {
                            new Serial(
                                schedule: new ByDateList { Items = new EdgeVertexs<IDate>(), },
                                rangeTime: null,
                                timeZoneProvider: timeZoneProvider),
                            fakeClock,
                            "RangeTime"
                        },
                        {
                            new Serial(
                                schedule: new ByDateList { Items = new EdgeVertexs<IDate>(), },
                                rangeTime: new EdgeRangeTime(new LocalTime(15, 30), null),
                                timeZoneProvider: timeZoneProvider),
                            fakeClock,
                            "Period"
                        },
                        {
                            new Serial(
                                schedule: new ByDateList { Items = new EdgeVertexs<IDate>(), },
                                rangeTime: new EdgeRangeTime(new LocalTime(15, 30), new PeriodBuilder {Hours = 00, Minutes = 30,}.Build()),
                                timeZoneProvider: null),
                            fakeClock,
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
                _exception = Record.Exception(() => { _episodes = _sut.GenerateEpisodes(_clock); });
            }

            public void ThenArgumentExceptionIsThrown(string parameterName)
            {
                _exception.ShouldNotBeNull();

                _exception.Message.ShouldBe(parameterName);
            }
        }
    }
}