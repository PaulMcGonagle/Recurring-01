﻿using System.Collections.Generic;
using System.Linq;
using ArangoDB.Client;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NodaTime;
using NodaTime.Testing;
using Scheduler;
using Scheduler.Calendars;
using Scheduler.Persistance;
using Scheduler.Ranges;
using Scheduler.ScheduleInstances;
using Scheduler.Users;
using Shouldly;
using TestHelpers;
using TestStack.BDDfy;
using Xunit;

namespace ScheduleGeneration.Test.ScheduleInstances
{
    [TestClass]
    public class SingleDayTests
    {
        public class VerifyCanBeSaved
        {
            private ISchedule _singleDay;
            private IClock _clock;
            private IArangoDatabase _db;

            [Fact]
            public void Execute()
            {
                var mockDb = MockVertexFactory<Schedule>.GetArangoDatabase();

                this.WithExamples(new ExampleTable(
                    "SUT",
                    "db",
                    "clock"
                )
                {
                    {
                        new Schedule(
                            new SingleDay.Builder
                            {
                                Date = new Date(2016, YearMonth.MonthValue.January, 01)
                            }.Build()),
                        mockDb.Object,
                        new FakeClock(Instant.FromUtc(2016, 12, 03, 12, 15))
                    }
                }).BDDfy();
            }

            public void GivenSingleDay(ISchedule sut)
            {
                _singleDay = sut;
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
                _singleDay.Save(_db, _clock);
            }
            
            public void ThenEventIsPersisted()
            {
                _singleDay.IsPersisted.ShouldBeTrue();
            }
        }

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
                                            new SingleDay.Builder
                                                {
                                                    Date = new Date(2016, YearMonth.MonthValue.January, 01)
                                                }.Build()),
                                        RangeTime = new RangeTime.Builder
                                            {
                                                Start = new LocalTime(16, 30),
                                                Period = new PeriodBuilder {Minutes = 45}.Build()
                                            }.Build(),
                                        TimeZoneProvider = "Europe/London"
                                    }.Build(),
                                Instance = new Instance(),
                                Location = new Location(),
                                Title = "new title"
                            }.Build(),
                        mockDb.Object,
                        new FakeClock(Instant.FromUtc(2016, 12, 03, 12, 15)),
                        new List<LocalDateTime> { new LocalDateTime(2016, 01, 01, 16, 30) }
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

            public void WhenGenerated()
            {
                Instance.Generate(_clock, _event);
            }

            public void ThenDatesAreAsExpected(IEnumerable<LocalDateTime> expectedEpisodes)
            {
                _event.Instance.ToVertex.Episodes.Select(e => e.ToVertex.Start.LocalDateTime).ShouldBe(expectedEpisodes.Select(ee => ee));
            }
        }
    }
}
