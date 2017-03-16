using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;
using System.Xml.XPath;
using ArangoDB.Client;
using Generators;
using NodaTime;
using NodaTime.Testing;
using NodaTime.Testing.TimeZones;
using Scheduler;
using Scheduler.Generation;
using Scheduler.Persistance;
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
            private IEnumerable<IEvent> _events;
            private IEvent _event;
            private IInstance _instance;
            private IList<LocalDate> _expectedLocalDates;
            private LocalTime _expectedLocalTime;
            private Period _expectedPeriod;
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
                        "C:\\Users\\Paul\\Documents\\Sandbox\\Recurring\\Recurring 01\\ScheduleGeneration.Test\\TestData\\BasicSchoolSchedule.xml",
                        fakeClock,
                        mockDb.Object,
                        "Hampden Gurney Primary School.Autumn.2016/17.Literacy",
                        "./generator/tags/tag[@id='TimeZoneProvider']",
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
                _expectedLocalDates = expectedLocalDates;
                _expectedPeriod = expectedPeriod;
            }

            public void AndGivenATimeZoneProviderPath(string timeZoneProviderPath)
            {
                _timeZoneProviderPath = timeZoneProviderPath;
            }

            public void WhenEventsAreGenerated()
            {
                _events = Generator.GenerateEvents(_sourceFile);
            }

            private void AndWhenASingleEventIsRetrieved()
            {
                _events.ShouldHaveSingleItem();

                _event = _events.Single();
            }

            private void AndWhenEventIsSaved()
            {
                _event.Save(_db, _clock);
            }

            private void AndWhenInstanceIsGenerated()
            {
                Instance.Generate(_clock, _event);
            }

            private void AndWhenInstanceIsRetrieved()
            {
                _instance = _event.Instance.ToVertex;
            }

            private void AndWhenEpisodesAreRetrieved()
            {
                _episodes = _instance
                    .Episodes
                    .Select(e => e.ToVertex)
                    .ToList();
            }

            private void AndWhenTimeZoneProviderIsFound()
            {
                var sourceTimeZone = _source.XPathSelectElement(_timeZoneProviderPath)?.Attribute("value");

                sourceTimeZone.ShouldNotBeNull();

                _timeZoneProvider = sourceTimeZone.Value;

            }

            private void ThenTitleIsExpected(string expectedTitle)
            {
                _event.Title.ShouldBe(expectedTitle);
            }

            private void AndInstancesAreExpected()
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

    public class LoadOption01
    {
        public class LoadHg
        {
            private string _sourceFile;
            private XElement _source;
            private IClock _clock;
            private IArangoDatabase _db;
            private IEnumerable<IEvent> _events;
            private IEvent _event;
            private IInstance _instance;
            private IList<LocalDate> _expectedLocalDates;
            private LocalTime _expectedLocalTime;
            private Period _expectedPeriod;
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
                        "C:\\Users\\Paul\\Documents\\Sandbox\\Recurring\\Recurring 01\\ScheduleGeneration.Test\\TestData\\Option01.xml",
                        fakeClock,
                        mockDb.Object,
                        "Hampden Gurney Primary School.Autumn.2016/17.Literacy",
                        "./generator/tags/tag[@id='TimeZoneProvider']",
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
                _expectedLocalDates = expectedLocalDates;
                _expectedPeriod = expectedPeriod;
            }

            public void AndGivenATimeZoneProviderPath(string timeZoneProviderPath)
            {
                _timeZoneProviderPath = timeZoneProviderPath;
            }

            public void WhenEventsAreGenerated()
            {
                _events = Generator.GenerateEvents(_sourceFile);
            }

            private void AndWhenASingleEventIsRetrieved()
            {
                _events.ShouldHaveSingleItem();

                _event = _events.Single();
            }

            private void AndWhenEventIsSaved()
            {
                _event.Save(_db, _clock);
            }

            private void AndWhenInstanceIsGenerated()
            {
                Instance.Generate(_clock, _event);
            }

            private void AndWhenInstanceIsRetrieved()
            {
                _instance = _event.Instance.ToVertex;
            }

            private void AndWhenEpisodesAreRetrieved()
            {
                _episodes = _instance
                    .Episodes
                    .Select(e => e.ToVertex)
                    .ToList();
            }

            private void AndWhenTimeZoneProviderIsFound()
            {
                var sourceTimeZone = _source.XPathSelectElement(_timeZoneProviderPath)?.Attribute("value");

                sourceTimeZone.ShouldNotBeNull();

                _timeZoneProvider = sourceTimeZone.Value;

            }

            private void ThenTitleIsExpected(string expectedTitle)
            {
                _event.Title.ShouldBe(expectedTitle);
            }

            private void AndInstancesAreExpected()
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
}
