﻿using System.Collections.Generic;
using System.Linq;
using ArangoDB.Client;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using NodaTime;
using NodaTime.Testing;
using Scheduler;
using Scheduler.Generation;
using Scheduler.Persistance;
using Scheduler.ScheduleEdges;
using Scheduler.ScheduleInstances;
using Scheduler.Test;
using Scheduler.Users;
using Shouldly;
using TestStack.BDDfy;
using Xunit;

namespace ScheduleGeneration.Test.ScheduleInstances
{
    [TestClass]
    public class ByWeekdayTests
    {
        public class GeneratesSingleEpisode
        {
            private Event _event;
            private IClock _clock;
            private IArangoDatabase _db;
            private GeneratedEvent _generatedEvent;

            [Fact]
            public void Execute()
            {
                var fakeClock = ScheduleTestHelper.GetFakeClock(2016, 05, 01);

                var e = new Event(new Serials
                {
                    new Serial(new CompositeSchedule()
                    {
                        InclusionsEdges = new EdgeVertexs<Schedule>
                        {
                            new EdgeVertex<Schedule>(new ByWeekday(fakeClock, IsoDayOfWeek.Wednesday)
                            {
                                EdgeRange = new EdgeRange(new Scheduler.Range(2016, YearMonth.MonthValue.February, 20, 2016, YearMonth.MonthValue.May, 15)),
                            })
                            ,
                        },
                    })
                    {
                        From = new LocalTime(16, 30),
                        Period = new PeriodBuilder {Minutes = 45}.Build(),
                        TimeZoneProvider = "Europe/London",
                    }
                })
                {
                    Location =
                        new EdgeVertex<Location>(
                            TestData.DataRetrieval.Organisations["Lords Cricket Academy"].Location.ToVertex),

                };

                var mockDb = new Mock<IArangoDatabase>();

                mockDb.Setup(x => x.Insert<Vertex>(It.IsAny<Vertex>(), null, null))
                    .Returns(TestHelper.MockInsertSuccess.Object);

                this.WithExamples(new ExampleTable(
                    "SUT",
                    "db",
                    "clock",
                    "Expected Episodes"
                )
                {
                    {
                        e,
                        mockDb.Object,
                        new FakeClock(Instant.FromUtc(2016, 12, 03, 12, 15)),

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
                _generatedEvent = new GeneratedEvent();

                _generatedEvent.Generate(_clock, _event);
            }

            public void ThenDatesAreAsExpected(IEnumerable<LocalDateTime> expectedEpisodes)
            {
                _generatedEvent.Episodes.Select(e => e.From.LocalDateTime).ShouldBe(expectedEpisodes.Select(ee => ee));
            }
        }
    }
}