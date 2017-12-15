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
using Scheduler.ScheduleInstances;
using Shouldly;
using TestHelpers;
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
            private ISerial _serial;
            private CompositeSchedule _compositeSchedule;
            private IEnumerable<IEpisode> _episodes;

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
                _generator = GenerateFromFileFactory.Get("classes");
            }

            public void AndWhenVertexsAreGenerated()
            {
                _vertexs = _generator
                    .Generate(_sourceFile, _clock)
                    .ToList();
            }

            public void ThenASerialIsCreated()
            {
                _serial = _vertexs
                    .OfType<ISerial>()
                    .SingleOrDefault();

                _serial.ShouldNotBeNull();
            }

            public void AndThenSchedulesAreGenerated()
            {
                _episodes = _serial
                    .GenerateEpisodes(_clock)
                    .GetToVertexs();
            }

            public void AndThenDatesAreExpected(IEnumerable<IDate> expectedDates)
            {
                _episodes
                    .Select(episode => episode.SourceGeneratedDate)
                    .GetToVertexs()
                    .ShouldBe(expectedDates);
            }
        }
    }
}
