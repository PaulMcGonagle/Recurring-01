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

namespace ScheduleGeneration.Test
{
    [TestClass]
    public class GeneratedEventTests
    {
        public class GenerateBasicEvent
        {
            private Event _event;
            private IClock _clock;
            private IArangoDatabase _db;
            private GeneratedEvent _generatedEvent;

            [Fact]
            public void Execute()
            {
                var e = new Event
                {
                    Location =
                        new EdgeVertex<Location>(
                            TestData.DataRetrieval.Organisations["Lords Cricket Academy"].Location.ToVertex),

                    Serials = new Serials
                    {
                        new Serial
                        {
                            From = new LocalTime(16, 30),
                            Period = new PeriodBuilder {Minutes = 45}.Build(),
                            TimeZoneProvider = "Europe/London",

                            EdgeSchedule = new EdgeSchedule(new CompositeSchedule()
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
                        }
                    }
                };

                var r = new Mock<IDocumentIdentifierResult>();
                r.Object.Id = Guid.NewGuid().ToString();
                r.Object.Key = Guid.NewGuid().ToString();
                r.Object.Rev = Guid.NewGuid().ToString();
                var v = new Mock<Vertex>();

                var m = new Mock<IArangoDatabase>();
                m.Setup(x => x.Insert<Event>(It.IsAny<Event>(), null, null)).Returns(r.Object);
                m.Setup(x => x.Insert<Vertex>(It.IsAny<Vertex>(), null, null)).Returns(r.Object);

                this.WithExamples(new ExampleTable(
                    "SUT",
                    "db",
                    "clock",
                    "Expected Episodes"
                )
                {
                    {
                        e,
                        m.Object,
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
    }
}
