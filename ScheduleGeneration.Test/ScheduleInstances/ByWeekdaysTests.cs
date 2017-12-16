using System.Collections.Generic;
using System.Linq;
using ArangoDB.Client;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NodaTime;
using Scheduler;
using Scheduler.Calendars;
using Scheduler.Persistance;
using Scheduler.Ranges;
using Scheduler.ScheduleEdges;
using Scheduler.ScheduleInstances;
using Scheduler.Test;
using Scheduler.Users;
using Shouldly;
using TestHelpers;
using TestStack.BDDfy;
using Xunit;

namespace ScheduleGeneration.Test.ScheduleInstances
{
    [TestClass]
    public class ByWeekdaysTests
    {
        public class GeneratesSingleEpisode
        {
            private IEvent _event;
            private IClock _clock;
            private IArangoDatabase _db;

            [Fact]
            public void Execute()
            {
                var mockDb = MockVertexFactory<Vertex>.GetArangoDatabase();

                this.WithExamples(new ExampleTable(
                    "SUT",
                    "db",
                    "clock",
                    "Expected Episodes"
                )
                {
                    {
                        new Event.Builder
                            {
                                Serial = new Serial.Builder
                                    {
                                        Schedule = new Schedule(
                                            new ByWeekdays.Builder
                                                {
                                                    Weekdays = new List<IsoDayOfWeek> { IsoDayOfWeek.Wednesday },
                                                    RangeDate = new RangeDate.Builder
                                                    { 
                                                        Start = new Date(2016, YearMonth.MonthValue.February, 20),
                                                        End = new Date(2016, YearMonth.MonthValue.May, 15)
                                                    }.Build(),
                                                }.Build()),
                                        RangeTime = new RangeTime.Builder
                                            {
                                                Start = new LocalTime(16, 30),
                                                Period = new PeriodBuilder {Minutes = 45}.Build()
                                            }.Build(),
                                        TimeZoneProvider = "Europe/London",
                                   }.Build(),
                                Instance = new Instance(),
                                Location = new Location(),
                                Title = "new title"
                            }.Build(),
                        mockDb.Object,
                        ScheduleTestHelper.GetFakeClock(2016, 12, 03, 12, 15),

                        new List<LocalDateTime>
                        {
                            new LocalDateTime(2016, 02, 24, 16, 30),
                            new LocalDateTime(2016, 03, 02, 16, 30),
                            new LocalDateTime(2016, 03, 09, 16, 30),
                            new LocalDateTime(2016, 03, 16, 16, 30),
                            new LocalDateTime(2016, 03, 23, 16, 30),
                            new LocalDateTime(2016, 03, 30, 16, 30),
                            new LocalDateTime(2016, 04, 06, 16, 30),
                            new LocalDateTime(2016, 04, 13, 16, 30),
                            new LocalDateTime(2016, 04, 20, 16, 30),
                            new LocalDateTime(2016, 04, 27, 16, 30),
                            new LocalDateTime(2016, 05, 04, 16, 30),
                            new LocalDateTime(2016, 05, 11, 16, 30),
                        }
                    },
                }).BDDfy();
            }

            public void GivenEvent(Event sut)
            {
                _event = sut;
            }

            public void AndGivenDatabase(IArangoDatabase db)
            {
                _db = db;
            }

            public void AndGivenClock(IClock clock)
            {
                _clock = clock;
            }

            public void WhenEventIsSaved()
            {
                _event.Save(_db, _clock);
            }

            public void AndWhenGenerated()
            {
                Instance.Generate(_clock, _event);
            }

            public void ThenInstanceExists()
            {
                _event.Instance.ShouldNotBeNull();
            }

            public void AndThenGeneratedDatesAreAsExpected(IEnumerable<LocalDateTime> expectedEpisodes)
            {
                _event.Instance.ToVertex.Episodes.Select(e => e.ToVertex.Start.LocalDateTime).ShouldBe(expectedEpisodes.Select(ee => ee));
            }
        }
    }

    public class GeneratesMultiipleEpisodes
    {
        private IEvent _event;
        private IClock _clock;
        private IArangoDatabase _db;

        [Fact]
        public void Execute()
        {
            var mockDb = MockVertexFactory<Vertex>.GetArangoDatabase();

            this.WithExamples(new ExampleTable(
                "SUT",
                "db",
                "clock",
                "Expected Episodes"
            )
                {
                    {
                        new Event.Builder
                            {
                                Serial = new Serial.Builder
                                    {
                                        Schedule = new Schedule(
                                            new ByWeekdays.Builder
                                                {
                                                    Weekdays = new List<IsoDayOfWeek>() { IsoDayOfWeek.Wednesday, },
                                                    EdgeRangeDate =
                                                        new EdgeRangeDate(
                                                            start: new Date(2016, YearMonth.MonthValue.February, 20),
                                                            end: new Date(2016, YearMonth.MonthValue.May, 15)),
                                                }.Build()
                                            ),
                                        RangeTime = new RangeTime.Builder
                                            {
                                                Start = new LocalTime(16, 30),
                                                Period = new PeriodBuilder
                                                {
                                                    Minutes = 45
                                                }.Build()
                                            }.Build(),
                                        TimeZoneProvider = "Europe/London",
                                    }.Build(),
                                Location = TestData.DataRetrieval.Organisations["Lords Cricket Academy"].Location.ToVertex,
                                Instance = new Instance(),
                                Title = "new title"
                            }.Build(),
                        mockDb.Object,
                        ScheduleTestHelper.GetFakeClock(2016, 12, 03, 12, 15),

                        new List<LocalDateTime>
                        {
                            new LocalDateTime(2016, 02, 24, 16, 30),
                            new LocalDateTime(2016, 03, 02, 16, 30),
                            new LocalDateTime(2016, 03, 09, 16, 30),
                            new LocalDateTime(2016, 03, 16, 16, 30),
                            new LocalDateTime(2016, 03, 23, 16, 30),
                            new LocalDateTime(2016, 03, 30, 16, 30),
                            new LocalDateTime(2016, 04, 06, 16, 30),
                            new LocalDateTime(2016, 04, 13, 16, 30),
                            new LocalDateTime(2016, 04, 20, 16, 30),
                            new LocalDateTime(2016, 04, 27, 16, 30),
                            new LocalDateTime(2016, 05, 04, 16, 30),
                            new LocalDateTime(2016, 05, 11, 16, 30),
                        }
                    },
                }).BDDfy();
        }

        public void GivenEvent(Event sut)
        {
            _event = sut;
        }

        public void AndGivenDatabase(IArangoDatabase db)
        {
            _db = db;
        }

        public void AndGivenClock(IClock clock)
        {
            _clock = clock;
        }

        public void WhenEventIsSaved()
        {
            _event.Save(_db, _clock);
        }

        public void AndWhenGenerated()
        {
            Instance.Generate(_clock, _event);
        }

        public void ThenInstanceExists()
        {
            _event.Instance.ShouldNotBeNull();
        }

        public void AndThenGeneratedDatesAreAsExpected(IEnumerable<LocalDateTime> expectedEpisodes)
        {
            _event.Instance.ToVertex.Episodes.Select(e => e.ToVertex.Start.LocalDateTime).ShouldBe(expectedEpisodes.Select(ee => ee));
        }
    }
}