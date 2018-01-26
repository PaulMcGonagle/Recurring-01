using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Linq;
using NodaTime;
using Scheduler;
using Scheduler.Persistance;
using Shouldly;
using TestStack.BDDfy;
using Xunit;

namespace Generators.Test.Instances
{
    public class GenerateFromFileTimetableTests
    {
        public class Go
        {
            private XElement _xElement;
            private IClock _clock;
            private string _tempFilename;
            private IGenerateFromFile _generator;
            private IEnumerable<IVertex> _vertexs;
            private IEnumerable<ISerial> _serials;

            [Fact]
            public void Execute()
            {
                this.WithExamples(new ExampleTable(
                    "xElement"
                )
                {
                    XDocument
                        .Load("c:\\users\\mcgon\\source\\repos\\recurring-01\\scenarios\\school\\files\\timetableshape.xml")
                        .Root,
                }).BDDfy();
            }

            public void GivenAnXElement(XElement xElement)
            {
                _xElement = xElement;
            }

            public void AndGivenAClock(IClock clock)
            {
                _clock = clock;
            }

            public void WhenDocumentIsSaved()
            {
                _tempFilename = Path.GetTempFileName();

                _xElement.Save(_tempFilename);
            }

            public void AndWhenGeneratorIsRetrieved()
            {
                _generator = GenerateFromFileFactory.Get("timetables");
            }

            public void AndWhenGenerated()
            {
                _vertexs = _generator
                    .Generate(_tempFilename, _clock)
                    .ToList();
            }

            public void AndWhenDocumentIsCleanedUp()
            {
                if (File.Exists(_tempFilename))
                {
                    File.Delete(_tempFilename);
                }
            }

            public void ThenASerialsAreCreated()
            {
                _serials = _vertexs
                    .OfType<ISerial>()
                    .ToList();

                _serials
                    .ShouldNotBeEmpty();
            }
        }
    }
}
