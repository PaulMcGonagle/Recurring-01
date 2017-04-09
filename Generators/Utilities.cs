using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using System.Xml.XPath;
using NodaTime;
using NodaTime.Text;
using Scheduler;
using Scheduler.Persistance;
using Scheduler.Ranges;
using Scheduler.ScheduleEdges;
using Scheduler.ScheduleInstances;

namespace Generators
{
    public static class Utilities
    {
        public static IEnumerable<XElement> RetrieveXReferences(XDocument xInput, IEnumerable<XElement> xReferences, string type)
        {
            var paths = xReferences
                .Where(tr => tr.Attribute("type")?.Value == type)
                .Select(tr => tr.Attribute("path")?.Value)
                .Where(p => p != null);

            foreach (var path in paths)
            {
                var xElements = xInput
                    .XPathSelectElements(path)
                    .ToList();

                if (!xElements.Any())
                    throw new Exception($"Unable to load reference {path}");

                foreach (var xElement in xElements)
                {
                    yield return xElement;
                }
            }
        }

        public static XElement RetrieveXReference(XDocument xInput, XElement xReference, string type)
        {
            var path = xReference
                .Attribute("path")?.Value;

            if (string.IsNullOrEmpty(path))
            {
                throw new ArgumentException("Path could not be found");
            }

            var xElement = xInput
                .XPathSelectElement(path);

            return xElement;
        }

        public static DateRange RetrieveRangeDate(this XElement xInput, string elementName = "rangeDate")
        {
            var rangeDate = xInput
                .Element(elementName);

            if (rangeDate == null)
                throw new ArgumentException("Could not find Element {elementName}");

            var start = rangeDate
                .RetrieveAttributeAsLocalDate("start");

            var end = rangeDate
                .RetrieveAttributeAsLocalDate("end");

            return new DateRange(
                from: new EdgeDate(start),
                to: new EdgeDate(end));
        }

        public static IEnumerable<IDateRange> RetrieveDateRanges(XElement xInput, IDictionary<string, IVertex> commons, string elementsName = "rangeDates", string elementName = "rangeDate")
        {
            var xElements = xInput
                .Elements(elementsName)
                .Elements(elementName)
                .ToList();

            foreach (var xElement in xElements)
            {
                yield return RetrieveRangeDate(xElement, elementName);
            }

            var xLinks = xInput
                .Elements(elementsName)
                .Elements("link")
                .ToList();

            foreach (var xLink in xLinks)
            {
                var commonName = xLink.RetrieveValue("common");

                IVertex commonVertex;

                if (!commons.TryGetValue(commonName, out commonVertex))
                    throw new ArgumentException($"No common found with attributeName {commonName}");

                IDateRange dateRange = commonVertex as IDateRange;

                if (dateRange == null)
                    throw new Exception("Common link does not return a date range");

                yield return dateRange;
            }
        }

        public static ITimeRange RetrieveRangeTime(this XElement input, string elementName = "rangeTime")
        {
            var rangeTime = input.Element(elementName);

            if (rangeTime == null)
                throw new ArgumentException("Could not find Element {elementName}");

            var start = ParseAttributeAsLocalTime(rangeTime, "start");
            var end = ParseAttributeAsLocalTime(rangeTime, "end");

            var period = Period.Between(start, end, PeriodUnits.AllTimeUnits);

            return new TimeRange(
                from: start,
                period: period);
        }

        public static IList<XElement> RetrieveXTags(XElement input)
        {
            return input
                .Elements("tags")
                .Elements("tag")
                .ToList();
        }

        public static ITag RetrieveTag(this XElement xInput)
        {
            var inputIdent = ParseAttribute(xInput, "id");
            var inputValue = ParseAttribute(xInput, "value");
            var inputPayload = xInput.Elements("payload").FirstOrDefault();

            var tag = new Tag(inputIdent.Value, inputValue.Value, inputPayload?.Value);

            tag.RelatedTags.AddRange(RetrieveXTags(xInput)
                .Select(relatedTag => new EdgeTag(RetrieveTag(relatedTag))));

            return tag;
        }

        public static IEnumerable<ITag> RetrieveTags(this XElement xInput)
        {
            var xTags = RetrieveXTags(xInput);

            foreach (var xTag in xTags)
            {
                yield return RetrieveTag(xTag);
            }
        }

        public static IEnumerable<IDate> RetrieveDates(XElement xInput)
        {
            var dates = new List<IDate>();

            var xDates = xInput
                .Elements("dates")
                .Elements("date")
                .ToList();

            foreach (var xDate in xDates)
            {
                var date = new Date(xDate.RetrieveAttributeAsLocalDate("value"));

                date.Connect(xDate.RetrieveTags());

                dates.Add(date);
            }

            var xScheduleInstances = xInput
                .Elements("dates")
                .Elements("scheduleInstance");

            foreach (var xScheduleInstance in xScheduleInstances)
            {
                dates.AddRange(RetrieveSchedule(xScheduleInstance).Generate());
            }

            return dates;
        }

        public static IEnumerable<IsoDayOfWeek> RetrieveWeekdays(this XElement xInput)
        {
            var dates = new List<IDate>();

            var xWeekdays = xInput
                .Elements("weekdays")
                .Elements("weekday")
                .ToList();

            foreach (var xWeekday in xWeekdays)
            {
                var weekday = xWeekday.RetrieveValue("value");

                IsoDayOfWeek isoDayOfWeek;

                if (!Enum.TryParse(weekday, out isoDayOfWeek))
                    throw new Exception($"Unable to parse weekday {weekday}");

                yield return isoDayOfWeek;
            }
        }

        public static ISchedule RetrieveSchedule(XElement xInput)
        {
            var tags = xInput.RetrieveTags();

            var scheduleTag = tags
                .SingleOrDefault(st => st.Ident == "type");

            if (scheduleTag == null)
                throw new Exception($"Unable to retrieve type");

            switch (scheduleTag.Value)
            {
                case "ByOffset":
                    {
                        var initialDateTag = scheduleTag
                            .RelatedTags
                            .SingleOrDefault(rt => rt.ToVertex.Ident == "InitialDate");

                        if (initialDateTag == null)
                        {
                            throw new Exception($"Unable to retrieve InitialDate tag");
                        }

                        var intervalTag = scheduleTag
                            .RelatedTags
                            .SingleOrDefault(rt => rt.ToVertex.Ident == "Interval");

                        if (intervalTag == null)
                            throw new Exception($"Unable to retrieve Interval tag");

                        var byOffset = new ByOffset(
                            initialDate: RetrieveLocalDate(initialDateTag.ToVertex.Value),
                            interval: intervalTag.ToVertex.Value);

                        var countTag = scheduleTag
                            .RelatedTags
                            .SingleOrDefault(rt => rt.ToVertex.Ident == "Count");

                        if (countTag != null)
                        {
                            int count;

                            if (!int.TryParse(countTag.ToVertex.Value, out count))
                            {
                                throw new Exception($"Unable to retrieve count '{countTag.ToVertex.Value}'");
                            }

                            byOffset.CountTo = count;
                        }
                        return byOffset;
                    }
                default:
                    throw new Exception($"Unable to retrieve schedule");
            }
        }

        public static LocalDate RetrieveLocalDate(string input)
        {
            var datePattern = LocalDatePattern.CreateWithInvariantCulture("yyyy-MM-dd");

            return datePattern.Parse(input).Value;
        }

        public static void ExpandReferences(XDocument xInput)
        {
            var xElements = xInput
                .Root
                ?.DescendantsAndSelf()
                .ToList();

            if (xElements == null)
            {
                return;
            }

            foreach (var xElement in xElements)
            {
                var xElementReferences = xElement
                    .Elements("reference")
                    .ToList();

                foreach (var xElementReference in xElementReferences)
                {
                    var type = xElementReference.Attribute("type")?.Value;

                    var xReferencedTags = RetrieveXReferences(xInput, xElementReferences, type)
                        .ToList();

                    xElement.Add(xReferencedTags);
                }

                xElementReferences.Remove();
            }
        }

        public static string RetrieveValue(this XElement xInput, string name)
        {
            if (string.IsNullOrEmpty(name))
                throw new ArgumentException("Unable to search for empty attribute attributeName");

            var value = xInput.Attribute(name)?.Value;

            if (value == null)
                throw new ArgumentException($"No attribute with attributeName '{name}' could be found");

            return value;
        }

        public static string RetrieveValue(this IEnumerable<ITag> tags, string name)
        {
            if (string.IsNullOrEmpty(name))
                throw new ArgumentException("Unable to search for tag with empty name");

            var tag = tags
                .SingleOrDefault(t => t.Ident == name);

            if (tag == null)
                throw new ArgumentException($"No tag with attributeName '{name}' could be found");

            return tag.Value;
        }

        public static IDictionary<string, IVertex> ExpandLinks(XDocument xInput)
        {
            var xElements = xInput
                .Root
                ?.DescendantsAndSelf()
                .ToList();

            var links = new Dictionary<string, IVertex>();

            if (xElements == null)
            {
                return links;
            }

            foreach (var xElement in xElements)
            {
                var xElementReferences = xElement
                    .Elements("common")
                    .ToList();

                foreach (var xElementReference in xElementReferences)
                {
                    var type = xElementReference.RetrieveValue("type");
                    var name = xElementReference.RetrieveValue("attributeName");

                    var generatorX = GeneratorFactory.GetX(type);

                    var xReferencedTags = RetrieveXReference(xInput, xElementReference, type);

                    var link = generatorX.Generate(xReferencedTags);

                    links.Add(name, link);
                }
            }

            return links;
        }

        public static bool TryParseAttributeAsWeekday(XElement input, string name, out IsoDayOfWeek value)
        {
            var attribute = input.Attribute(name);

            if (attribute != null
            && Enum.TryParse(attribute.Value, out value))
            {
                return true;
            }

            value = IsoDayOfWeek.None;

            return false;
        }

        public static LocalDate RetrieveAttributeAsLocalDate(this XElement xInput, string attributeName)
        {
            var attribute = xInput.Attribute(attributeName);

            var localDatePattern = LocalDatePattern.CreateWithInvariantCulture("yyyy-MM-dd");
            var parseResult = localDatePattern.Parse(attribute?.Value);

            return parseResult.Value;
        }

        public static LocalTime ParseAttributeAsLocalTime(XElement input, string name)
        {
            var attribute = ParseAttribute(input, name);

            var localTimePattern = LocalTimePattern.CreateWithInvariantCulture("HH:mm");
            var parseResult = localTimePattern.Parse(attribute.Value);

            return parseResult.Value;
        }

        public static XAttribute ParseAttribute(XElement input, string name)
        {
            if (input == null)
            {
                throw new ArgumentNullException(nameof(input));
            }

            var attribute = input.Attribute(name);

            if (attribute == null)
            {
                throw new ArgumentNullException(name);
            }

            return attribute;
        }
    }
}
