using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using ArangoDB.Client;
using Generators;
using NodaTime;
using NodaTime.Testing;
using Scheduler;
using Scheduler.Persistance;
using Shouldly;
using TestStack.BDDfy;
using Xunit;

namespace ScheduleGeneration.Test
{
    public class LoadFiles
    {
        private string _type;
        private string _sourceFile;
        private IList<Tuple<string, XElement>> _sources;
        private XElement _source;
        private IClock _clock;
        private IArangoDatabase _db;
        private string _timeZoneProviderPath;
        private string _timeZoneProvider;
        private IEnumerable<IVertex> _vertexs;

        [Fact]
        public void Execute()
        {
            var fakeClock = new FakeClock(Instant.FromUtc(2016, 05, 01, 0, 0));

            var mockDb = MockVertexFactory<Vertex>.GetArangoDatabase();

            this.WithExamples(new ExampleTable(
                "type",
                "sourceFile",
                "clock",
                "db",
                "timeZoneProviderPath"
            )
            {
                {
                    "byOffset",
                    "C:\\Users\\mcgon\\Source\\Repos\\Recurring-01\\Generators\\Sources\\Caterlink3.xml",
                    fakeClock,
                    mockDb.Object,
                    "./tags/tag[@id='timeZoneProvider']"
                },
                //{
                //    "schedule",
                //    "C:\\Users\\mcgon\\Source\\Repos\\Recurring-01\\Generators\\Sources\\Caterlink2.xml",
                //    fakeClock,
                //    mockDb.Object,
                //    "./tags/tag[@id='timeZoneProvider']"
                //},
                //{
                //    "holidays", 
                //    "C:\\Users\\mcgon\\Source\\Repos\\Recurring-01\\Generators\\Sources\\Holidays.xml",
                //    fakeClock,
                //    mockDb.Object,
                //    "./tags/tag[@id='timeZoneProvider']"
                //},
            }).BDDfy();
        }

        public void GivenSourceFile(string sourceFile)
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

        public void AndGivenATimeZoneProviderPath(string timeZoneProviderPath)
        {
            _timeZoneProviderPath = timeZoneProviderPath;
        }

        public void WhenSourcesAreLoaded()
        {
            _sources = new List<Tuple<string, XElement>>();

            var generatorName = _type;
            var generator = GeneratorFactory.Get(generatorName);
            _vertexs = generator
                .Generate(_sourceFile, _clock)
                .ToList();
        }

        public void ThenVertexsHaveBeenGenerated()
        {
            _vertexs.ShouldNotBeEmpty();
        }

        public void AndThenAtLeastOneScheduleIsReturned()
        {
            var @events = _vertexs
                .OfType<ISchedule>();

            @events
                .ShouldNotBeEmpty();
        }
    }
}
