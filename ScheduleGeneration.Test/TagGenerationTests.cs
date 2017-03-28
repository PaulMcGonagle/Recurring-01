using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using System.Xml.XPath;
using Castle.Core.Internal;
using Generators;
using Scheduler;
using Scheduler.Users;
using Shouldly;
using TestStack.BDDfy;
using Xunit;

namespace ScheduleGeneration.Test
{
    public class TagGenerationTests
    {
        public class VerifyTags
        {
            private string _sourceFile;
            private XElement _source;
            private IGenerator _generator;

            private IEnumerable<IEvent> _events;
            private IOrganisation _organisation;
            private IEvent _event;
            private ISerial _serial;

            private IEnumerable<XElement> _providedGeneratorTags;
            private IEnumerable<XElement> _providedScheduleTags;
            private IEnumerable<XElement> _providedClassTags;
            private IEnumerable<XElement> _providedTermTags;
            private IEnumerable<XElement> _providedGroupTags;

            [Fact]
            public void Execute()
            {
                this.WithExamples(new ExampleTable(
                    "sourceFile",
                    "expectedOrganisation",
                    "generatorTagPath",
                    "scheduleTagPath",
                    "classTagPath",
                    "termTagPath",
                    "groupTagPath")
                    {
                        {
                            "C:\\Users\\Paul\\Documents\\Sandbox\\Recurring\\Recurring 01\\ScheduleGeneration.Test\\TestData\\BasicSchoolSchedule.xml",
                            new Organisation
                            {
                                Title = "a test organisation",
                            },
                            "./generator/tags/tag",
                            "./generator/groups/group/classes/class/schedules/schedule/tags/tag",
                            "./generator/groups/group/classes/class/tags/tag",
                            "./generator/terms/term/tags/tag",
                            "./generator/groups/group/tags/tag"
                        },
                    })
                    .BDDfy();
            }

            public void GivenASourceFile(string sourceFile)
            {
                _sourceFile = sourceFile;

                _source = XElement.Load(sourceFile);
            }

            public void AndGivenAnOrganisation(IOrganisation expectedOrganisation)
            {
                _organisation = expectedOrganisation;
            }

            public void AndGivenGeneratorTagPath(string generatorTagPath)
            {
                _providedGeneratorTags = _source.XPathSelectElements(generatorTagPath);
            }

            public void AndGivenClassTagPath(string classTagPath)
            {
                _providedClassTags = _source.XPathSelectElements(classTagPath);
            }

            public void AndGivenTermTagPath(string termTagPath)
            {
                _providedTermTags = _source.XPathSelectElements(termTagPath);
            }

            public void AndGivenScheduleTagPath(string scheduleTagPath)
            {
                _providedScheduleTags = _source.XPathSelectElements(scheduleTagPath);
            }

            public void AndGivenYearTagPath(string groupTagPath)
            {
                _providedGroupTags = _source.XPathSelectElements(groupTagPath);
            }

            public void WhenGeneratorIsRetrieved()
            {
                _generator = GeneratorFactory.Get("classes");
            }

            public void AndWhenEventsAreGenerated()
            {
                var vertexs = _generator.Generate(_sourceFile);

                _events = vertexs.OfType<Event>();
            }

            public void ThenOneEventIsGenerated()
            {
                _events.Count().ShouldBe(1);

                _event = _events.Single();
            }

            public void AndThenEventTagsAreValid()
            {
                var expectedTags = _providedGroupTags
                    .ToList();

                CompareTagsToSource(_event.Tags.Select(e => e.ToVertex), expectedTags);
            }

            public void AndThenEventHasOneSerial()
            {
                _event.Serials.Count().ShouldBe(1);

                _serial = _event.Serials.Single().ToVertex;
            }

            public void AndThenSerialTagsAreValid()
            {
                var expectedTags = _providedScheduleTags
                    .Union(_providedGeneratorTags)
                    .Union(_providedGroupTags)
                    .Union(_providedTermTags)
                    .ToList();

                CompareTagsToSource(_serial.Tags
                    .Select(s => s.ToVertex)
                    .Where(s => s.Ident != "term"), expectedTags);

                _serial.Tags
                    .Select(s => s.ToVertex)
                    .Single(s => s.Ident == "term")
                    ?.Value.ShouldBe("Autumn.2016/17");

            }
        }

        public static void CompareTagsToSource(IEnumerable<ITag> tags, IEnumerable<XElement> sourceTags)
        {
            sourceTags = sourceTags.ToList();

            if (sourceTags.IsNullOrEmpty())
            {
                tags.ShouldBeEmpty();

                return;
            }

            var enumeratedTags = tags as ITag[] ?? tags.ToArray();

            enumeratedTags.Count().ShouldBe(sourceTags.Count());

            foreach (var tag in enumeratedTags)
            {
                var sourceTag = sourceTags
                    .Where(st => st.Attribute("id")?.Value == tag.Ident)
                    .Where(st => st.Attribute("value")?.Value == tag.Value);

                sourceTag.ShouldHaveSingleItem();
            }
        }

        public static void ShouldBeSameAs(IEnumerable<ITag> tags, IEnumerable<XElement> sourceTags)
        {
            sourceTags = sourceTags.ToList();

            if (sourceTags.IsNullOrEmpty())
            {
                tags.ShouldBeEmpty();

                return;
            }

            var enumeratedTags = tags as ITag[] ?? tags.ToArray();

            enumeratedTags.Count().ShouldBe(sourceTags.Count());

            foreach (var tag in enumeratedTags)
            {
                var sourceTag = sourceTags
                    .Where(st => st.Attribute("id")?.Value == tag.Ident)
                    .Where(st => st.Attribute("value")?.Value == tag.Value);

                sourceTag.ShouldHaveSingleItem();

                //CompareTagsToSource(tag.RelatedTags.Select(rl => rl.ToVertex), sourceTag.Single());
            }
        }
    }
}
