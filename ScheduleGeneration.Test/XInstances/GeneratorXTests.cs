using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using System.Xml.XPath;
using ArangoDB.Client;
using Generators;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NodaTime;
using NodaTime.Testing;
using Scheduler;
using Scheduler.Persistance;
using Shouldly;
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
                    .XPathSelectElements("./schedules/schedule");

                IDictionary<string, IVertex> caches= new Dictionary<string, IVertex>();
                _vertexs = new List<IVertex>();

                foreach (var schedule in schedules)
                {
                    var xTags = schedule
                        .Element("tags");

                    var type = xTags
                        ?.XPathSelectElement("./tag[@id='type']")
                        ?.Attribute("value")
                        ?.Value;
                    type.ShouldNotBeNullOrWhiteSpace();

                    var generator = GeneratorFactory.GetX(type);

                    IVertex vertex;

                    generator
                        .TryGenerate(schedule, caches, out vertex)
                        .ShouldBeTrue();

                    _vertexs.Add(vertex);
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