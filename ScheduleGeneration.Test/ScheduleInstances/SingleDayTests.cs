using System;
using System.Collections.Generic;
using System.Linq;
using ArangoDB.Client;
using ArangoDB.Client.Data;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using NodaTime;
using NodaTime.Testing;
using Scheduler;
using Scheduler.Generation;
using Scheduler.Persistance;
using Scheduler.ScheduleEdges;
using Scheduler.ScheduleInstances;
using Scheduler.Users;
using Shouldly;
using TestStack.BDDfy;
using Xunit;

namespace ScheduleGeneration.Test.ScheduleInstances
{
    [TestClass]
    public class SingleDayTests
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
                var mockDb = new Mock<IArangoDatabase>();

                mockDb.Setup(x => x.Insert<Vertex>(It.IsAny<Vertex>(), null, null)).Returns(TestHelper.MockInsertSuccess.Object);

                this.WithExamples(new ExampleTable(
                    "SUT",
                    "db",
                    "clock",
                    "Expected Episodes"
                )
                {
                    {
                        Event.Create(
                            schedule: new SingleDay { Date = new Date(2016, YearMonth.MonthValue.January, 01) },
                            from: new LocalTime(16, 30),
                            period: new PeriodBuilder {Minutes = 45}.Build(),
                            timeZoneProvider: "Europe/London"),
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
                _generatedEvent = new GeneratedEvent();

                _generatedEvent.Generate(_clock, _event);
            }

            public void ThenDatesAreAsExpected(IEnumerable<LocalDateTime> expectedEpisodes)
            {
                _generatedEvent.Episodes.Select(e => e.From.LocalDateTime).ShouldBe(expectedEpisodes.Select(ee => ee));
            }
        }

        public class MissingFieldsRaiseExpectedException
        {
            private Event _event;
            private IClock _clock;
            private IArangoDatabase _db;
            private GeneratedEvent _generatedEvent;
            private Exception _exception;

            [Fact]
            public void Execute()
            {
                var mockDb = new Mock<IArangoDatabase>();

                mockDb.Setup(x => x.Insert<Vertex>(It.IsAny<Vertex>(), null, null)).Returns(TestHelper.MockInsertSuccess.Object);

                this.WithExamples(new ExampleTable(
                    "SUT",
                    "db",
                    "clock",
                    "Expected Message"
                )
                {
                    {
                        new Event(new Serials
                        {
                            new Serial(new CompositeSchedule()
                                {
                                    InclusionsEdges = new EdgeVertexs<Schedule>
                                    {
                                        new EdgeVertex<Schedule>(new SingleDay
                                        {
                                            Date = new Date(2016, YearMonth.MonthValue.January, 01),
                                        })
                                        ,
                                    },
                                })
                            {
                                //From = new LocalTime(16, 30),
                                Period = new PeriodBuilder {Minutes = 45}.Build(),
                                TimeZoneProvider = "Europe/London",
                            }
                        }),
                        mockDb.Object,
                        new FakeClock(Instant.FromUtc(2016, 12, 03, 12, 15)),
                        "From"
                    },
                    {
                        Event.Create(
                            schedule: new SingleDay
                                        {
                                            Date = new Date(2016, YearMonth.MonthValue.January, 01),
                                        },
                            from: new LocalTime(16, 30),
                            period: null,
                            timeZoneProvider: "Europe/London"),
                        mockDb.Object,
                        new FakeClock(Instant.FromUtc(2016, 12, 03, 12, 15)),
                        "Period"
                    },
                    {
                        Event.Create(
                            schedule: new SingleDay
                                        {
                                            Date = new Date(2016, YearMonth.MonthValue.January, 01),
                                        },
                            from: new LocalTime(16, 30),
                            period: new PeriodBuilder {Minutes = 45}.Build(),
                            timeZoneProvider: null),
                        mockDb.Object,
                        new FakeClock(Instant.FromUtc(2016, 12, 03, 12, 15)),
                        "TimeZoneProvider"
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

                _exception = Record.Exception(() => _generatedEvent.Generate(_clock, _event));
            }

            public void ThenArgumentExceptionIsThrown(string expectedMessage)
            {
                _exception.ShouldBeOfType(typeof(ArgumentException));
                _exception.Message.ShouldBe(expectedMessage);
            }
        }
    }
}
