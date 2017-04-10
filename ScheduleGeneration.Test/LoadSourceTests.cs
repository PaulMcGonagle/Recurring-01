using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using System.Xml.XPath;
using ArangoDB.Client;
using Generators;
using NodaTime;
using NodaTime.Testing;
using Scheduler;
using Scheduler.Generation;
using Scheduler.Persistance;
using Scheduler.ScheduleEdges;
using Shouldly;
using TestStack.BDDfy;
using Xunit;

namespace ScheduleGeneration.Test
{
    public class LoadSourceTests
    {
        public class LoadHg
        {
            private string _sourceFile;
            private XElement _source;
            private IClock _clock;
            private IArangoDatabase _db;
            private IGenerator _generator;
            private IEnumerable<IEvent> _events;
            private IEvent _event;
            private IInstance _instance;
            private IEnumerable<IEpisode> _episodes;
            private string _timeZoneProviderPath;
            private string _timeZoneProvider;

            [Fact]
            public void Execute()
            {
                var fakeClock = new FakeClock(Instant.FromUtc(2016, 05, 01, 0, 0));

                var mockDb = MockVertexFactory<Vertex>.GetArangoDatabase();

                this.WithExamples(new ExampleTable(
                    "sourceFile",
                    "clock",
                    "db",
                    "expectedTitle",
                    "timeZoneProviderPath",
                    "expectedLocalDates",
                    "expectedLocalTime",
                    "expectedPeriod"
                )
                {
                    {
                        AppDomain.CurrentDomain.BaseDirectory + "\\TestData\\BasicSchoolSchedule.xml",
                        fakeClock,
                        mockDb.Object,
                        "Hampden Gurney Primary School.Autumn.2016/17.Year2.Literacy",
                        "./generator/tags/tag[@id='timeZoneProvider']",
                        new List<LocalDate>
                        {
                            new LocalDate(year: 2016, month: 02, day: 03),
                        },
                        new LocalTime(hour: 09, minute: 15),
                        new PeriodBuilder(Period.FromMinutes(65)).Build()
                    },
                }).BDDfy();
            }

            public void GivenASourceFile(string sourceFile)
            {
                _sourceFile = sourceFile;

                _source = XElement.Load(sourceFile);
            }

            public void AndGivenAClock(IClock clock)
            {
                _clock = clock;
            }

            public void AndGivenADatabase(IArangoDatabase db)
            {
                _db = db;
            }

            public void AndGivenExpectedTimings(
                IList<LocalDate> expectedLocalDates,
                Period expectedPeriod)
            {
            }

            public void AndGivenATimeZoneProviderPath(string timeZoneProviderPath)
            {
                _timeZoneProviderPath = timeZoneProviderPath;
            }

            public void WhenGeneratorIsRetrieved()
            {
                _generator = GeneratorFactory.Get("classes");
            }

            public void AndWhenEventsAreGenerated()
            {
                var vertexs = _generator.Generate(_sourceFile, _clock);

                _events = vertexs.OfType<Event>();
            }

            public void AndWhenASingleEventIsRetrieved()
            {
                _events.ShouldHaveSingleItem();

                _event = _events.Single();
            }

            public void AndWhenEventIsSaved()
            {
                _event.Save(_db, _clock);
            }

            public void AndWhenInstanceIsGenerated()
            {
                Instance.Generate(_clock, _event);
            }

            public void AndWhenInstanceIsRetrieved()
            {
                _instance = _event.Instance.ToVertex;
            }

            public void AndWhenEpisodesAreRetrieved()
            {
                _episodes = _instance
                    .Episodes
                    .Select(e => e.ToVertex)
                    .ToList();
            }

            public void AndWhenTimeZoneProviderIsFound()
            {
                var sourceTimeZone = _source.XPathSelectElement(_timeZoneProviderPath)?.Attribute("value");

                sourceTimeZone.ShouldNotBeNull();

                _timeZoneProvider = sourceTimeZone?.Value;

            }

            public void ThenTitleIsExpected(string expectedTitle)
            {
                _event.Title.ShouldBe(expectedTitle);
            }

            public void AndInstancesAreExpected()
            {
                _instance.Episodes.ShouldNotBeEmpty();
            }

            public void AndTimeZoneProviderIsExpected(string timeZoneProviderPath)
            {

                _episodes.Select(e => e.From.Zone.Id).ShouldAllBe(t => t == _timeZoneProvider);
                _episodes.Select(e => e.To.Zone.Id).ShouldAllBe(t => t == _timeZoneProvider);
            }

            public void AndTimesAreExpected(LocalTime expectedLocalTime, Period expectedPeriod)
            {
                _episodes.Select(e => e.From.LocalDateTime.TimeOfDay).ShouldAllBe(d => d == expectedLocalTime);
                _episodes.Select(e => e.To.LocalDateTime.TimeOfDay).ShouldAllBe(d => d == expectedLocalTime.Plus(expectedPeriod));
            }
        }
    }

    public class LoadOptions
    {
        public class LoadOption01
        {
            private string _sourceFile;
            private XElement _source;
            private IClock _clock;
            private IArangoDatabase _db;
            private IGenerator _generator;
            private IEnumerable<IEvent> _events;
            private IEvent _event;
            private IInstance _instance;
            private IEnumerable<IEpisode> _episodes;
            private string _timeZoneProviderPath;
            private string _timeZoneProvider;

            [Fact]
            public void Execute()
            {
                var fakeClock = new FakeClock(Instant.FromUtc(2016, 05, 01, 0, 0));

                var mockDb = MockVertexFactory<Vertex>.GetArangoDatabase();

                this.WithExamples(new ExampleTable(
                    "sourceFile",
                    "clock",
                    "db",
                    "expectedTitle",
                    "timeZoneProviderPath",
                    "expectedLocalDates",
                    "expectedLocalTime",
                    "expectedPeriod"
                )
                {
                    {
                        "..\\..\\TestData\\Option01.xml",
                        fakeClock,
                        mockDb.Object,
                        "Hampden Gurney Primary School.Autumn.2016/17.Year2.Literacy",
                        "./generator/tags/tag[@id='timeZoneProvider']",
                        new List<LocalDate>
                        {
                            new LocalDate(year: 2016, month: 02, day: 03),
                        },
                        new LocalTime(hour: 09, minute: 15),
                        new PeriodBuilder(Period.FromMinutes(65)).Build()
                    },
                }).BDDfy();
            }

            public void GivenASourceFile(string sourceFile)
            {
                _sourceFile = sourceFile;

                _source = XElement.Load(sourceFile);
            }

            public void AndGivenAClock(IClock clock)
            {
                _clock = clock;
            }

            public void AndGivenADatabase(IArangoDatabase db)
            {
                _db = db;
            }

            public void AndGivenExpectedTimings(
                IList<LocalDate> expectedLocalDates,
                Period expectedPeriod)
            {
            }

            public void AndGivenATimeZoneProviderPath(string timeZoneProviderPath)
            {
                _timeZoneProviderPath = timeZoneProviderPath;
            }

            public void WhenGeneratorIsRetrieved()
            {
                _generator = GeneratorFactory.Get("classes");
            }

            public void AndWhenEventsAreGenerated()
            {
                var vertexs = _generator.Generate(_sourceFile, _clock);

                _events = vertexs.OfType<Event>();
            }

            public void AndWhenASingleEventIsRetrieved()
            {
                _events.ShouldHaveSingleItem();

                _event = _events.Single();
            }

            public void AndWhenEventIsSaved()
            {
                _event.Save(_db, _clock);
            }

            public void AndWhenInstanceIsGenerated()
            {
                Instance.Generate(_clock, _event);
            }

            public void AndWhenInstanceIsRetrieved()
            {
                _instance = _event.Instance.ToVertex;
            }

            public void AndWhenEpisodesAreRetrieved()
            {
                _episodes = _instance
                    .Episodes
                    .Select(e => e.ToVertex)
                    .ToList();
            }

            public void AndWhenTimeZoneProviderIsFound()
            {
                var sourceTimeZone = _source.XPathSelectElement(_timeZoneProviderPath)?.Attribute("value");

                sourceTimeZone.ShouldNotBeNull();

                _timeZoneProvider = sourceTimeZone?.Value;

            }

            public void ThenTitleIsExpected(string expectedTitle)
            {
                _event.Title.ShouldBe(expectedTitle);
            }

            public void AndInstancesAreExpected()
            {
                _instance.Episodes.ShouldNotBeEmpty();
            }

            public void AndTimeZoneProviderIsExpected(string timeZoneProviderPath)
            {

                _episodes.Select(e => e.From.Zone.Id).ShouldAllBe(t => t == _timeZoneProvider);
                _episodes.Select(e => e.To.Zone.Id).ShouldAllBe(t => t == _timeZoneProvider);
            }

            public void AndTimesAreExpected(LocalTime expectedLocalTime, Period expectedPeriod)
            {
                _episodes.Select(e => e.From.LocalDateTime.TimeOfDay).ShouldAllBe(d => d == expectedLocalTime);
                _episodes.Select(e => e.To.LocalDateTime.TimeOfDay).ShouldAllBe(d => d == expectedLocalTime.Plus(expectedPeriod));
            }
        }

        public class LoadOption02
        {
            private string _sourceFile;
            private XElement _source;
            private IClock _clock;
            private IArangoDatabase _db;
            private IGenerator _generator;
            private IEnumerable<IEvent> _events;
            private IEvent _event;
            private IInstance _instance;
            private IEnumerable<IEpisode> _episodes;
            private string _timeZoneProviderPath;
            private string _timeZoneProvider;

            [Fact]
            public void Execute()
            {
                var fakeClock = new FakeClock(Instant.FromUtc(2016, 05, 01, 0, 0));

                var mockDb = MockVertexFactory<Vertex>.GetArangoDatabase();

                this.WithExamples(new ExampleTable(
                    "sourceFile",
                    "clock",
                    "db",
                    "timeZoneProviderPath",
                    "generatorName"
                )
                {
                    {
                        "..\\..\\TestData\\Option02.xml",
                        fakeClock,
                        mockDb.Object,
                        "./generator/tags/tag[@id='timeZoneProvider']",
                        "classes"
                    },
                }).BDDfy();
            }

            public void AndGivenExpectedTimings(
                IList<LocalDate> expectedLocalDates,
                Period expectedPeriod)
            {
            }

            public void AndGivenATimeZoneProviderPath(string timeZoneProviderPath)
            {
                _timeZoneProviderPath = timeZoneProviderPath;
            }

            public void WhenSourceFileIsLoaded(string sourceFile)
            {
                _source = XElement.Load(_sourceFile);
            }

            public void AndWhenGeneratorIsRetrieved(string generatorName)
            {
                _generator = GeneratorFactory.Get(generatorName);
            }

            public void AndWhenEventsAreGenerated()
            {
                var vertexs = _generator
                    .Generate(_sourceFile, _clock)
                    .ToList();

                _events = vertexs.OfType<Event>();
            }

            public void ThenSchedulesShouldHaveSameYearTagReference()
            {
                var serials = _events
                    .SelectMany(e => e.Serials.Select(s => s.ToVertex))
                    .ToList();

                var schedules = serials
                    .Select(s => s.EdgeSchedule.Schedule)
                    .ToList();

                schedules.Count().ShouldBe(2);

                var yearTag0 = schedules[0]
                    .RelatedTags
                    .SingleOrDefault(t => t.ToVertex.Ident == "year")
                    ?.ToVertex;
                var yearTag1 = schedules[1]
                    .RelatedTags
                    .SingleOrDefault(t => t.ToVertex.Ident == "year")
                    ?.ToVertex;

                yearTag0.ShouldNotBeNull();
                yearTag1.ShouldNotBeNull();

                object.ReferenceEquals(yearTag0, yearTag1).ShouldBeTrue();
            }
        }
    }
}
