using ArangoDB.Client;
using Generators;
using NodaTime;
using NodaTime.Testing;
using Scheduler;
using Scheduler.Persistance;
using Shouldly;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using System.Xml.XPath;
using Scheduler.Calendars;
using TestHelpers;
using TestStack.BDDfy;
using Xunit;

namespace ScheduleGeneration.Test
{
    public class LoadSourceTests
    {
        public class LoadBasicSchoolSchedule
        {
            private string _sourceFile;
            private XElement _source;
            private IClock _clock;
            private IArangoDatabase _db;
            private IGenerateFromFile _generator;
            private IEnumerable<IVertex> _vertexs;
            private IEvent _event;
            private IInstance _instance;
            private IEnumerable<IEpisode> _episodes;
            private string _timeZoneProviderPath;

            [Fact]
            public void Execute()
            {
                var fakeClock = new FakeClock(Instant.FromUtc(2016, 05, 01, 0, 0));

                var mockDb = MockVertexFactory<Vertex>.GetArangoDatabase();

                this.WithExamples(new ExampleTable(
                    "sourceFile",
                    "clock",
                    "db",
                    //"expectedTitle",
                    "timeZoneProviderPath"//,
                    //"expectedLocalDates",
                    //"expectedLocalTime",
                    //"expectedPeriod"
                )
                {
                    {
                        AppDomain.CurrentDomain.BaseDirectory + "\\TestData\\BasicSchoolSchedule.xml",
                        fakeClock,
                        mockDb.Object,
                        //"Hampden Gurney Primary School.Autumn.2016/17.Year2.Literacy",
                        "./tags/tag[@id='timeZoneProvider']"//,
                        //new List<LocalDate>
                        //{
                        //    new LocalDate(year: 2016, month: 02, day: 03),
                        //},
                        //new LocalTime(hour: 09, minute: 15),
                        //new PeriodBuilder(Period.FromMinutes(65)).Build()
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
                _generator = GenerateFromFileFactory.Get("classes");
            }

            public void AndWhenVertexsAreGenerated()
            {
                _vertexs = _generator.Generate(_sourceFile, _clock);
            }
        }

        public class LoadHg
        {
            private string _sourceFile;
            private XElement _source;
            private IClock _clock;
            private IArangoDatabase _db;
            private IGenerateFromFile _generator;
            private IEnumerable<IEvent> _events;
            private IEnumerable<IInstance> _instances;
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
                    "timeZoneProviderPath"
                )
                {
                    {
                        AppDomain.CurrentDomain.BaseDirectory + "..\\TestData\\HG2.xml",
                        fakeClock,
                        mockDb.Object,
                        "./tags/tag[@id='timeZoneProvider']"
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
                _generator = GenerateFromFileFactory.Get("classes");
            }

            public void AndWhenEventsAreGenerated()
            {
                var vertexs = _generator
                    .Generate(_sourceFile, _clock)
                    .ToList();

                _events = vertexs.OfType<Event>()
                    .ToList();
            }

            public void AndWhenEventIsSaved()
            {
                foreach (var @event in _events)
                {
                    @event.Save(_db, _clock);
                }
            }

            public void AndWhenInstanceIsGenerated()
            {
                foreach (var @event in _events)
                {
                    Instance.Generate(_clock, @event);
                }
            }

            public void AndWhenInstancesareRetrieved()
            {
                _instances = _events
                    .Select(e => e.Instance.ToVertex);
            }

            public void AndWhenEpisodesAreRetrieved()
            {
                _episodes = _instances
                    .SelectMany(i => i
                        .Episodes
                        .Select(e => e.ToVertex))
                    .ToList();
            }

            public void AndWhenTimeZoneProviderIsFound()
            {
                var sourceTimeZone = _source.XPathSelectElement(_timeZoneProviderPath)?.Attribute("value");

                sourceTimeZone.ShouldNotBeNull();

                _timeZoneProvider = sourceTimeZone?.Value;

            }

            public void AndInstancesAreExpected()
            {
                foreach (var instance in _instances)
                {
                    instance.Episodes.ShouldNotBeEmpty();
                }
            }

            public void AndTimeZoneProviderIsExpected(string timeZoneProviderPath)
            {

                _episodes.Select(e => e.Start.Zone.Id).ShouldAllBe(t => t == _timeZoneProvider);
                _episodes.Select(e => e.End.Zone.Id).ShouldAllBe(t => t == _timeZoneProvider);
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
            private IGenerateFromFile _generator;
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
                    //"expectedTitle",
                    "timeZoneProviderPath"
                    //"expectedLocalDates",
                    //"expectedLocalTime",
                    //"expectedPeriod"
                )
                {
                    {
                        "..\\..\\TestData\\Option01.xml",
                        fakeClock,
                        mockDb.Object,
                        //"Hampden Gurney Primary School.Autumn.2016/17.Year2.Literacy",
                        "./tags/tag[@id='timeZoneProvider']"
                        //new List<LocalDate>
                        //{
                        //    new LocalDate(year: 2016, month: 02, day: 03),
                        //},
                        //new LocalTime(hour: 09, minute: 15),
                        //new PeriodBuilder(Period.FromMinutes(65)).Build()
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
                _generator = GenerateFromFileFactory.Get("classes");
            }

            public void AndWhenEventsAreGenerated()
            {
                var vertexs = _generator.Generate(_sourceFile, _clock);

                _events = vertexs.OfType<Event>();
            }
        }

        public class LoadOption02
        {
            private string _sourceFile;
            private IClock _clock;
            private IArangoDatabase _db;
            private IGenerateFromFile _generator;
            private IEnumerable<ISchedule> _schedules;

            [Fact]
            public void Execute()
            {
                var fakeClock = new FakeClock(Instant.FromUtc(2016, 05, 01, 0, 0));

                var mockDb = MockVertexFactory<Vertex>.GetArangoDatabase();

                this.WithExamples(new ExampleTable(
                    "sourceFile",
                    "clock",
                    "db",
                    "generatorName"
                )
                {
                    {
                        "..\\..\\TestData\\Option02.xml",
                        fakeClock,
                        mockDb.Object,
                        "classes"
                    },
                }).BDDfy();
            }

            public void GivenASourceFile(string sourceFile)
            {
                _sourceFile = sourceFile;
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

            public void WhenGeneratorIsRetrieved(string generatorName)
            {
                _generator = GenerateFromFileFactory.Get(generatorName);
            }

            public void AndWhenEventsAreGenerated()
            {
                var vertexs = _generator
                    .Generate(_sourceFile, _clock)
                    .ToList();

                _schedules = vertexs
                    .OfType<ISchedule>()
                    .ToList();
            }

            public void AndWhenEventsAreSaved()
            {
                foreach (var compositeSchedule in _schedules)
                {
                    compositeSchedule.Save(_db, _clock);
                }
            }

            //public void ThenSchedulesShouldHaveDifferentNames()
            //{
            //    var compositeScheduleList = new List<ICompositeSchedule>(_compositeSchedules);

            //    var compositeSchedule1 = compositeScheduleList[0];
            //    var compositeSchedule2 = compositeScheduleList[1];

            //    compositeSchedule1.Title.ShouldNotBe(compositeSchedule2.Title);
            //}

            //public void ThenSchedulesShouldHaveSameYearTagReference()
            //{
            //    var serials = _compositeSchedules
            //        .SelectMany(e => e.Serials.Select(s => s.ToVertex))
            //        .ToList();

            //    var schedules = serials
            //        .Select(s => s.EdgeSchedule.Schedule)
            //        .ToList();

            //    schedules.Count.ShouldBe(2);

            //    var tagName1 = schedules[0]
            //        .Tags
            //        .SingleOrDefault(t => t.ToVertex.Ident == "name")
            //        ?.ToVertex;
            //    var tagName2 = schedules[1]
            //        .Tags
            //        .SingleOrDefault(t => t.ToVertex.Ident == "name")
            //        ?.ToVertex;

            //    tagName1.ShouldNotBeNull();
            //    tagName2.ShouldNotBeNull();

            //    tagName1?.Key.ShouldBe(tagName2?.Key);
            //}
        }

        public class LoadOption03
        {
            private string _sourceFile;
            private IClock _clock;
            private IArangoDatabase _db;
            private IGenerateFromFile _generator;
            private IEnumerable<IVertex> _vertexs;

            [Fact]
            public void Execute()
            {
                var fakeClock = new FakeClock(Instant.FromUtc(2016, 05, 01, 0, 0));

                var mockDb = MockVertexFactory<Vertex>.GetArangoDatabase();

                this.WithExamples(new ExampleTable(
                    "sourceFile",
                    "clock",
                    "db",
                    "generatorName"
                )
                {
                    {
                        "..\\..\\TestData\\Option03.xml",
                        fakeClock,
                        mockDb.Object,
                        "classes"
                    },
                }).BDDfy();
            }

            public void GivenASourceFile(string sourceFile)
            {
                _sourceFile = sourceFile;
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

            public void AndWhenGeneratorIsRetrieved(string generatorName)
            {
                _generator = GenerateFromFileFactory.Get(generatorName);
            }

            public void AndWhenVertexsAreGenerated()
            {
                _vertexs = _generator
                    .Generate(_sourceFile, _clock)
                    .ToList();
            }

            public void AndWhenVertexsAreSaved()
            {
                foreach (var vertex in _vertexs)
                {
                    vertex.Save(_db, _clock);
                }
            }

            //public void ThenSchedulesShouldHaveSameYearTagReference()
            //{
            //    var serials = _vertexs
            //        .SelectMany(e => e.Serials.Select(s => s.ToVertex))
            //        .ToList();

            //    var schedules = serials
            //        .Select(s => s.EdgeSchedule.Schedule)
            //        .ToList();

            //    schedules.Count.ShouldBe(2);

            //    var tagName1 = schedules[0]
            //        .Tags
            //        .SingleOrDefault(t => t.ToVertex.Ident == "name")
            //        ?.ToVertex;

            //    var tagName2 = schedules[1]
            //        .Tags
            //        .SingleOrDefault(t => t.ToVertex.Ident == "name")
            //        ?.ToVertex;

            //    tagName1?.Key.ShouldNotBeNull();
            //    tagName2?.Key.ShouldNotBeNull();


            //    tagName1?.Key.ShouldBe(tagName2?.Key);
            //}
        }

        public class LoadOption04
        {
            private string _sourceFile;
            private XElement _source;
            private IClock _clock;
            private IArangoDatabase _db;
            private IGenerateFromFile _generator;
            private IEnumerable<IVertex> _vertexs;

            [Fact]
            public void Execute()
            {
                var fakeClock = new FakeClock(Instant.FromUtc(2016, 05, 01, 0, 0));

                var mockDb = MockVertexFactory<Vertex>.GetArangoDatabase();

                this.WithExamples(new ExampleTable(
                    "sourceFile",
                    "clock",
                    "db",
                    "generatorName",
                    "scheduleNameTagPath"
                )
                {
                    {
                        "..\\..\\TestData\\Option04.xml",
                        fakeClock,
                        mockDb.Object,
                        "classes",
                        "./groups/group[tags/tag[@id='name'][@value='Year 2']]/classes/class[tags/tag[@id='name'][@value='Literacy']]/terms/term/tags/tag[@id='name']"
                    },
                }).BDDfy();
            }

            public void GivenASourceFile(string sourceFile)
            {
                _sourceFile = sourceFile;
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

            public void WhenSourceFileIsLoaded(string sourceFile)
            {
                _source = XElement.Load(_sourceFile);
            }

            public void AndWhenGeneratorIsRetrieved(string generatorName)
            {
                _generator = GenerateFromFileFactory.Get(generatorName);
            }

            public void AndWhenEventsAreGenerated()
            {
                _vertexs = _generator
                    .Generate(_sourceFile, _clock)
                    .ToList();
            }

            public void AndWhenVertexsAreSaved()
            {
                foreach (var vertex in _vertexs)
                {
                    vertex.Save(_db, _clock);
                }
            }

            public void ThenSchedulesShouldHaveCorrectName(string scheduleNameTagPath)
            {
                //var serials = _vertexs
                //    .SelectMany(e => e.Serials.Select(s => s.ToVertex))
                //    .ToList();

                //var schedules = serials
                //    .Select(s => s.EdgeSchedule.Schedule)
                //    .ToList();

                //schedules.Count.ShouldBe(1);

                //var scheduleName = _source
                //    .XPathSelectElement(scheduleNameTagPath)
                //    ?.Attribute("value")
                //    ?.Value;

                //schedules[0]
                //    .Tags
                //    .SingleOrDefault(t => t.ToVertex.Ident == "name")
                //    ?.ToVertex
                //    .Value
                //    .ShouldBe(scheduleName);
            }
        }
    }
}
