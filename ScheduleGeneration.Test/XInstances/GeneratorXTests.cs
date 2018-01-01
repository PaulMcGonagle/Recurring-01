using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using ArangoDB.Client;
using Generators;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NodaTime;
using NodaTime.Testing;
using Scheduler.Persistance;
using Shouldly;
using TestHelpers;
using TestStack.BDDfy;
using Xunit;

namespace ScheduleGeneration.Test.XInstances
{
    [TestClass]
    public class GeneratorXTests
    {
        public class GenerateXByOffsetTest
        {
            private string _sourceFile;
            private XElement _source;
            private IClock _clock;
            private IArangoDatabase _db;
            private IList<IVertex> _vertexs;

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
                        AppDomain.CurrentDomain.BaseDirectory + "..\\TestData\\GeneratorXByOffset.xml",
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

            public void AndWhenScheduleExists()
            {
            }

            public void AndWhenGenerated()
            {
                var schedules = _source
                    .Elements("schedules")
                    .Single();

                IDictionary<string, IVertex> caches= new Dictionary<string, IVertex>();
                _vertexs = new List<IVertex>();

                foreach (var xSchedule in schedules.Elements())
                {
                    var xTags = xSchedule
                        .Element("tags");

                    var type = xSchedule.Name.LocalName;

                    type.ShouldNotBeNullOrWhiteSpace();

                    var generator = GenerateFromFileFactory.GetX(type);

                    var schedule = generator.Generate(
                            xSchedule,
                        caches);

                    _vertexs.Add(schedule);
                }
            }

            public void ThenVertexesAreExpected()
            {
                _vertexs
                    .ShouldNotBeNull();
            }
        }
    }
}