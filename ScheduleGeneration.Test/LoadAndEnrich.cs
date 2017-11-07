using System;
using System.Collections.Generic;
using System.Linq;
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
    public class LoadAndEnrich
    {
        public class EnrichEventNumbering
        {
            private string _sourceFile;
            private IClock _clock;
            private IGenerateFromFile _generator;
            private IEnumerable<IVertex> _vertexs;
            private ICompositeSchedule _compositeSchedule;
            private IEnumerable<IDate> _dates;

            [Fact]
            public void Execute()
            {
                var fakeClock = new FakeClock(Instant.FromUtc(2016, 05, 01, 0, 0));

                var mockDb = MockVertexFactory<Vertex>.GetArangoDatabase();

                this.WithExamples(new ExampleTable(
                    "sourceFile",
                    "clock",
                    "db",
                    "expectedDates"
                )
                {
                    {
                        AppDomain.CurrentDomain.BaseDirectory + "\\TestData\\BasicSchoolSchedule.xml",
                        fakeClock,
                        mockDb.Object,
                        new List<IDate>
                        {
                            new Date(2017, YearMonth.MonthValue.January, 02),
                            new Date(2017, YearMonth.MonthValue.January, 03),
                            new Date(2017, YearMonth.MonthValue.January, 04),
                            new Date(2017, YearMonth.MonthValue.January, 06),
                            new Date(2017, YearMonth.MonthValue.January, 09),
                            new Date(2017, YearMonth.MonthValue.January, 10)
                        }
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

            public void AndWhenVertexsAreGenerated()
            {
                _vertexs = _generator
                    .Generate(_sourceFile, _clock)
                    .ToList();
            }

            public void AndWhenASingleCompositeScheduleIsRetrieved()
            {
                _compositeSchedule = _vertexs
                    .OfType<ICompositeSchedule>()
                    .Single();
            }

            public void AndWhenSchedulesAreGenerated()
            {
                _dates = _compositeSchedule
                    .Generate(_clock);
            }

            public void ThenDatesAreExpected(IEnumerable<IDate> expectedDates)
            {
                _dates
                    .ShouldBe(expectedDates);
            }

            //public void AndWhenSerialIsRetrieved()
            //{
            //    var @event = _events
            //        .Single();

            //    var serial = @event
            //        .Serials
            //        .Single()
            //        .ToVertex;

            //    _episodes = serial
            //        .GenerateEpisodes(_clock)
            //        .Select(e => e.ToVertex);
            //}

            //public void AndWhenEpisodesAreEnriched()
            //{
            //    var enricher = new EnricherNumbering();

            //    enricher.Go(
            //        vertexs: _episodes,
            //        ident: "EventNumber"
            //        );
            //}

            //public void ThenEventsMustHaveNumbers()
            //{
            //    foreach (var episode in _episodes)
            //    {
            //        var tag = episode
            //            .Tags
            //            .SingleOrDefault(e => e.ToVertex.Ident == "EventNumber");

            //        tag.ShouldNotBeNull();

            //        var value = tag
            //            ?.ToVertex
            //            .Value;

            //        // ReSharper disable once UnusedVariable
            //        int.TryParse(value, out int unusedResult).ShouldBeTrue();
            //    }
            //}
        }
    }
}
