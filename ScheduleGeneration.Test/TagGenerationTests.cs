using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using System.Xml.XPath;
using Castle.Core.Internal;
using Generators;
using Scheduler;
using Scheduler.Persistance;
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
            private IGenerateFromFile _generator;

            private IEnumerable<IVertex> _vertexs;
            private ISerial _serial;

            private IEnumerable<XElement> _providedGeneratorTags;
            private IEnumerable<XElement> _providedScheduleTags;
            private IEnumerable<XElement> _providedTermTags;
            private IEnumerable<XElement> _providedClassTags;
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
                            AppDomain.CurrentDomain.BaseDirectory + "\\TestData\\BasicSchoolSchedule.xml",
                            new Organisation
                            {
                                Title = "a test organisation",
                            },
                            "./tags/tag",
                            "./groups/group/classes/class/schedules/byWeekdays/tags/tag",
                            "./groups/group/classes/class/tags/tag",
                            "./terms/term/tags/tag",
                            "./groups/group/tags/tag"
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
                _generator = GenerateFromFileFactory.Get("classes");
            }

            public void AndWhenVertexsAreGenerated()
            {
                 _vertexs = _generator
                    .Generate(_sourceFile, null)
                    .ToArray();
            }

            public void AndWhenOneSerialIsRetrieved()
            {
                _serial = _vertexs.OfType<ISerial>().Single();
            }

            public void AndThenSerialTagsAreValid()
            {
                CompareTagsToSource(
                    _serial.Tags.Select(e => e.ToVertex), 
                    _providedClassTags);
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

            enumeratedTags.Length.ShouldBe(sourceTags.Count());

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

            enumeratedTags.Length.ShouldBe(sourceTags.Count());

            foreach (var tag in enumeratedTags)
            {
                var sourceTag = sourceTags
                    .Where(st => st.Attribute("id")?.Value == tag.Ident)
                    .Where(st => st.Attribute("value")?.Value == tag.Value);

                sourceTag.ShouldHaveSingleItem();

                //CompareTagsToSource(tag.Tags.Select(rl => rl.ToVertex), sourceTag.Single());
            }
        }
    }
}
