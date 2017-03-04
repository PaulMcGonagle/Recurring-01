﻿using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using ArangoDB.Client;
using Generators;
using NodaTime;
using NodaTime.Testing;
using Scheduler;
using Scheduler.Generation;
using Scheduler.Persistance;
using Scheduler.Ranges;
using Scheduler.ScheduleEdges;
using Scheduler.ScheduleInstances;
using Shouldly;
using TestStack.BDDfy;
using Xunit;

namespace ScheduleGeneration.Test
{
    public class CompositeScheduleTests
    {
        public class GeneratesSingleEpisode
        {
            private IEvent _event;
            private IClock _clock;
            private IArangoDatabase _db;

            [Fact]
            public void Execute()
            {
                var fakeClock = new FakeClock(Instant.FromUtc(2016, 05, 01, 0, 0));

                var mockDb = MockVertexFactory<Vertex>.GetArangoDatabase();

                this.WithExamples(new ExampleTable(
                    "SUT",
                    "db",
                    "clock",
                    "Expected Episodes",
                    "Duration"
                )
                {
                    {
                        Event.Create(
                            schedule: new ByWeekday(fakeClock, IsoDayOfWeek.Wednesday)
                            {
                                EdgeRange = new EdgeRangeDate(new DateRange(2016, YearMonth.MonthValue.February, 20, 2016, YearMonth.MonthValue.May, 15)),
                            },
                            rangeTime: new TimeRange(new LocalTime(16, 30), new PeriodBuilder {Minutes = 45}.Build()),
                            timeZoneProvider: "Europe/London",
                            location: TestData.DataRetrieval.Organisations["Lords Cricket Academy"].Location.ToVertex),
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
                        },
                        new PeriodBuilder {Minutes = 45}.Build()
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
                GeneratedEvent.Generate(_clock, _event);
            }

            public void ThenGeneratedEventExists()
            {
                _event.GeneratedEvent.ShouldNotBeNull();
            }

            public void AndThenDatesAreAsExpected(IEnumerable<LocalDateTime> expectedEpisodes, Period duration)
            {
                _event.GeneratedEvent.ToVertex.Episodes.Select(e => e.ToVertex.From.LocalDateTime).ShouldBe(expectedEpisodes.Select(ee => ee));
                _event.GeneratedEvent.ToVertex.Episodes.Select(e => e.ToVertex.To.LocalDateTime).ShouldBe(expectedEpisodes.Select(ee => ee.Plus(duration)));
            }
        }
        public class LoadBasicSchoolScheduleAndVerifyEpisodes
        {
            private string _sourceFile;
            private XElement _source;

            private IClock _clock;
            private IArangoDatabase _db;
            private IEnumerable<IEvent> _events;

            [Fact]
            public void Execute()
            {
                var fakeClock = new FakeClock(Instant.FromUtc(2016, 05, 01, 0, 0));

                var mockDb = MockVertexFactory<Vertex>.GetArangoDatabase();

                this.WithExamples(new ExampleTable(
                    "sourceFile",
                    "db",
                    "clock",
                    "Expected Episodes"
                )
                {
                    {
                        "C:\\Users\\Paul\\Documents\\Sandbox\\Recurring\\Recurring 01\\Generators\\Sources\\HG.xml",
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

            public void GivenASourceFile(string sourceFile)
            {
                _sourceFile = sourceFile;

                _source = XElement.Load(sourceFile);
            }

            public void AndGivenDatabase(IArangoDatabase db)
            {
                _db = db;
            }

            public void AndGivenClock(IClock clock)
            {
                _clock = clock;
            }

            public void WhenEventIsGenerated()
            {
                _events = Generator.GenerateEvents(_sourceFile);
            }

            public void AndWhenEventsAreSaved()
            {
                foreach (var @event in _events)
                {
                    @event.Save(_db, _clock);
                }
            }

            public void AndWhenGenerated()
            {
                foreach (var @event in _events)
                {
                    GeneratedEvent.Generate(_clock, @event);
                }
            }

            public void ThenGeneratedEventExists()
            {
                foreach (var @event in _events)
                {
                    @event.GeneratedEvent.ShouldNotBeNull();
                }
            }

            public void AndThenDatesAreAsExpected(IEnumerable<LocalDateTime> expectedEpisodes)
            {
                //todo Verify expected episodes
                //_event.GeneratedEvent.ToVertex.Episodes.Select(e => e.ToVertex.From.LocalDateTime).ShouldBe(expectedEpisodes.Select(ee => ee));
            }
        }
        public class GeneratesMultipleEpisodes
        {
            private IEvent _event;
            private IClock _clock;
            private IArangoDatabase _db;

            [Fact]
            public void Execute()
            {
                var fakeClock = new FakeClock(Instant.FromUtc(2016, 05, 01, 0, 0));

                var mockDb = MockVertexFactory<Vertex>.GetArangoDatabase();

                this.WithExamples(new ExampleTable(
                    "SUT",
                    "db",
                    "clock",
                    "Expected Episodes"
                )
                {
                    {
                        Event.Create(
                            schedule: new ByWeekday(fakeClock, IsoDayOfWeek.Wednesday)
                            {
                                EdgeRange = new EdgeRangeDate(new DateRange(2016, YearMonth.MonthValue.February, 20, 2016, YearMonth.MonthValue.May, 15)),
                            },
                            rangeTime: new TimeRange(new LocalTime(16, 30), new PeriodBuilder {Minutes = 45}.Build()),
                            timeZoneProvider: "Europe/London",
                            location: TestData.DataRetrieval.Organisations["Lords Cricket Academy"].Location.ToVertex),
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
                GeneratedEvent.Generate(_clock, _event);
            }

            public void ThenGeneratedEventExists()
            {
                _event.GeneratedEvent.ShouldNotBeNull();
            }

            public void AndThenDatesAreAsExpected(IEnumerable<LocalDateTime> expectedEpisodes)
            {
                _event.GeneratedEvent.ToVertex.Episodes.Select(e => e.ToVertex.From.LocalDateTime).ShouldBe(expectedEpisodes.Select(ee => ee));
            }
        }
    }
}
