using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using ArangoDB.Client;
using Generators;
using Generators.Enrichers;
using NodaTime;
using NodaTime.Testing;
using Scheduler;
using Scheduler.Persistance;
using TestStack.BDDfy;
using Xunit;

namespace ScheduleGeneration.Test
{
    public class LoadAndEnrich
    {
        public class EnrichEventNumbering
        {
            private string _sourceFile;
            private XElement _source;
            private IClock _clock;
            private IArangoDatabase _db;
            private IGenerator _generator;
            private IEnumerable<IEvent> _events;

            [Fact]
            public void Execute()
            {
                var fakeClock = new FakeClock(Instant.FromUtc(2016, 05, 01, 0, 0));

                var mockDb = MockVertexFactory<Vertex>.GetArangoDatabase();

                this.WithExamples(new ExampleTable(
                    "sourceFile",
                    "clock",
                    "db"
                )
                {
                    {
                        AppDomain.CurrentDomain.BaseDirectory + "\\TestData\\BasicSchoolSchedule.xml",
                        fakeClock,
                        mockDb.Object
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

            public void WhenGeneratorIsRetrieved()
            {
                _generator = GeneratorFactory.Get("classes");
            }

            public void AndWhenEventsAreGenerated()
            {
                var vertexs = _generator.Generate(_sourceFile, _clock);

                _events = vertexs.OfType<Event>();
            }

            public void AndWhenEventsAreEnriched()
            {
                var enricher = new EnricherNumbering();

                enricher.Go(
                    vertexs: _events,
                    ident: "EventNumber"
                    );
            }

            public void ThenEventsMustHaveNumbers()
            {
                foreach (var @event in _events)
                {
                    var tag = @event
                        .Tags
                        .Where(e => e.ToVertex.Ident == "EventNumber");
                }
            }
        }
    }
}
