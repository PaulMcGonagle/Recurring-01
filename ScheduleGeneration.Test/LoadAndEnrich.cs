using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using ArangoDB.Client;
using Generators;
using Generators.Enrichers;
using NodaTime;
using NodaTime.Testing;
using Scheduler;
using Scheduler.Persistance;
using Shouldly;
using TestStack.BDDfy;
using Xunit;

namespace ScheduleGeneration.Test
{
    public class LoadAndEnrich
    {
        public class EnrichEventNumbering
        {
            private string _sourceFile;
            private IClock _clock;
            private IGenerator _generator;
            private IEnumerable<IEvent> _events;
            private IEnumerable<IEpisode> _episodes;

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

                XElement.Load(sourceFile);
            }

            public void AndGivenAClock(IClock clock)
            {
                _clock = clock;
            }

            public void AndGivenADatabase(IArangoDatabase db)
            {
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

            public void AndWhenSerialIsRetrieved()
            {
                var @event = _events
                    .Single();

                var serial = @event
                    .Serials
                    .Single()
                    .ToVertex;

                _episodes = serial
                    .Episodes
                    .Select(e => e.ToVertex);
            }

            public void AndWhenEpisodesAreEnriched()
            {
                var enricher = new EnricherNumbering();

                enricher.Go(
                    vertexs: _episodes,
                    ident: "EventNumber"
                    );
            }

            public void ThenEventsMustHaveNumbers()
            {
                foreach (var episode in _episodes)
                {
                    var tag = episode
                        .Tags
                        .SingleOrDefault(e => e.ToVertex.Ident == "EventNumber");

                    tag.ShouldNotBeNull();

                    var value = tag
                        ?.ToVertex
                        .Value;

                    // ReSharper disable once UnusedVariable
                    int.TryParse(value, out int unusedResult).ShouldBeTrue();
                }
            }
        }
    }
}
