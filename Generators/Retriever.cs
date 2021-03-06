﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using System.Xml.XPath;
using CoreLibrary;
using NodaTime;
using NodaTime.Text;
using Scheduler;
using Scheduler.Persistance;
using Scheduler.Ranges;
using Scheduler.ScheduleEdges;
using Scheduler.ScheduleInstances;

namespace Generators
{
    public static class Retriever
    {
        public static IEnumerable<XElement> RetrieveXReferences(this XElement xInput, IEnumerable<XElement> xReferences, string type)
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

        public static IEnumerable<XElement> RetrieveXReferences(this XElement xInput, XElement xReference, string type)
        {
            var path = xReference
                .Attribute("path")?.Value;

            if (string.IsNullOrEmpty(path))
            {
                throw new ArgumentException("Path could not be found");
            }

            var xElements = xInput
                .XPathSelectElements(path);

            foreach (var xElement in xElements)
            {
                yield return xElement;
            }
        }

        public static RangeDate RetrieveRangeDate(this XElement xInput, IDictionary<string, IVertex> caches, string elementName = "rangeDate")
        {
            var x = GenerateFromFileFactory.GetX("RangeDate");

            var rangeDate = (RangeDate)x.Generate(xInput, caches, elementName: elementName);

            rangeDate.Connect(xInput.RetrieveTags(caches));

            return rangeDate;
        }

        public static IEnumerable<IRangeDate> RetrieveRangeDates(this XElement xInput, IDictionary<string, IVertex> caches, string elementsName = "rangeDates", string elementName = "rangeDate")
        {
            var xRangeDates = xInput
                .Element(elementsName);

            if (xRangeDates == null)
                yield break;

            foreach (var xElement in xRangeDates.Elements(elementName))
            {
                var rangeDate = RetrieveRangeDate(xElement, caches, elementsName);

                yield return rangeDate;
            }

            foreach (var link in UtilitiesLinks<RangeDate>.RetrieveAll(xRangeDates, caches))
            {
                yield return link;
            }
        }

        public static IEnumerable<IRangeTime> RetrieveRangeTimes(this XElement xInput, IDictionary<string, IVertex> caches, string elementsName = "rangeTimes", string elementName = "rangeTime")
        {
            Guard.AgainstNull(xInput, nameof(xInput));
            Guard.AgainstNull(caches, nameof(caches));

            var xRangeTimes = xInput
                .Element(elementsName);

            if (xRangeTimes is null)
                throw new Exception("No element {elementsName} could be found");

            foreach (var xElement in xRangeTimes.Elements(elementName))
            {
                var rangeTime = RetrieveRangeTime(xElement, caches, elementsName);

                yield return rangeTime;
            }

            foreach (var link in UtilitiesLinks<RangeTime>.RetrieveAll(xRangeTimes, caches))
            {
                yield return link;
            }
        }
        
        public static RangeTime RetrieveRangeTime(this XElement xInput, IDictionary<string, IVertex> caches, string elementName = "rangeTime")
        {
            Guard.AgainstNull(xInput, nameof(xInput));
            Guard.AgainstNull(caches, nameof(caches));

            var start = xInput
                .RetrieveAttributeAsLocalTime("start");

            var end = xInput
                .RetrieveAttributeAsLocalTime("end");

            var period = Period.Between(start, end, PeriodUnits.AllTimeUnits);

            var rangeTime = new RangeTime.Builder
            {
                Start = start,
                Period = period
            }.Build();

            rangeTime.Connect(xInput.RetrieveTags(caches));

            return rangeTime;
        }

        public static IList<XElement> RetrieveXTags(XElement xInput)
        {
            Guard.AgainstNull(xInput, nameof(xInput));

            return xInput
                .Elements("tags")
                .Elements("tag")
                .ToList();
        }

        public static ITag RetrieveTag(this XElement xInput)
        {
            Guard.AgainstNull(xInput, nameof(xInput));

            var inputIdent = RetrieveAttribute(xInput, "id");
            var inputValue = RetrieveAttribute(xInput, "value");
            var inputPayload = xInput.Elements("payload").FirstOrDefault();

            var tag = new Tag(inputIdent.Value, inputValue.Value, inputPayload?.Value);

            var xTags = RetrieveXTags(xInput);
            var relatedTags = xTags
                .Select(RetrieveTag);

            tag.Tags.AddRange(relatedTags
                .Select(relatedTag => new EdgeTag(relatedTag)));

            return tag;
        }

        public static IEnumerable<ITag> RetrieveTags(this XElement xInput, IDictionary<string, IVertex> caches, string elementsName = null)
        {
            Guard.AgainstNull(xInput, nameof(xInput));

            var xTags = RetrieveXTags(xInput);

            foreach (var xTag in xTags)
            {
                yield return RetrieveTag(xTag);
            }

            if (caches == null)
                yield break;

            var xLinkTags = xInput
                .Elements("tags");

            foreach (var xLink in xLinkTags)
            {
                var links = UtilitiesLinks<Tag>.RetrieveAll(xLink, caches)
                    .ToList();

                foreach (var link in links)
                {
                    yield return link;
                }
            }
        }

        public static IEnumerable<IDate> RetrieveDates(this XElement xInput, IClock clock, IDictionary<string, IVertex> caches, string elementsName = "date")
        {
            Guard.AgainstNull(xInput, nameof(xInput));
            Guard.AgainstNull(clock, nameof(clock));
            Guard.AgainstNull(caches, nameof(caches));

            var dates = new List<IDate>();

            var xDates = xInput
                .Elements(elementsName)
                .ToList();

            foreach (var xDate in xDates)
            {
                var date = new Date(xDate.RetrieveAttributeAsLocalDate("value"));

                date.Connect(xDate.RetrieveTags(caches));

                dates.Add(date);
            }

            var xScheduleInstances = xInput
                .Elements("dates")
                .Elements("scheduleInstance");

            foreach (var xScheduleInstance in xScheduleInstances)
            {
                var schedule = RetrieveSchedule(xScheduleInstance, caches);

                dates.AddRange(schedule.Generate(clock));
            }

            return dates;
        }

        public static IEnumerable<IsoDayOfWeek> RetrieveWeekdays(this XElement xInput)
        {
            var xWeekdays = xInput
                .Elements("weekdays")
                .Elements("weekday")
                .ToList();

            foreach (var xWeekday in xWeekdays)
            {
                var weekday = xWeekday.RetrieveValue("value");

                if (!Enum.TryParse(weekday, out IsoDayOfWeek isoDayOfWeek))
                    throw new Exception($"Unable to parse weekday {weekday}");

                yield return isoDayOfWeek;
            }
        }

        public static ISchedule RetrieveSchedule(XElement xInput, IDictionary<string, IVertex> caches)
        {
            var tags = xInput.RetrieveTags(caches);

            var scheduleTag = tags
                .SingleOrDefault(st => st.Ident == "type");

            if (scheduleTag == null)
                throw new Exception($"Unable to retrieve type");

            ISchedule schedule = null;

            switch (scheduleTag.Value)
            {
                case "ByOffset":
                {
                    var initialDateTag = scheduleTag
                        .Tags
                        .SingleOrDefault(rt => rt.ToVertex.Ident == "InitialDate");

                    if (initialDateTag == null)
                    {
                        throw new Exception($"Unable to retrieve InitialDate tag");
                    }

                    var intervalTag = scheduleTag
                        .Tags
                        .SingleOrDefault(rt => rt.ToVertex.Ident == "Interval");

                    if (intervalTag == null)
                        throw new Exception($"Unable to retrieve Interval tag");

                    var scheduleInstance = new ByOffset.Builder
                    {
                        InitialDate = RetrieveLocalDate(initialDateTag.ToVertex.Value),
                        Interval = intervalTag.ToVertex.Value,
                    }.Build();

                    var countTag = scheduleTag
                        .Tags
                        .SingleOrDefault(rt => rt.ToVertex.Ident == "Count");

                    if (countTag == null) break;

                    if (!int.TryParse(countTag.ToVertex.Value, out int count))
                    {
                        throw new Exception($"Unable to retrieve count '{countTag.ToVertex.Value}'");
                    }

                    scheduleInstance.CountTo = count;

                    schedule = new Schedule(scheduleInstance);

                    break;
                }

                default:
                    throw new Exception($"Unable to retrieve schedule");
            }

            return schedule;
        }

        public static LocalDate RetrieveLocalDate(string input)
        {
            var datePattern = LocalDatePattern.CreateWithInvariantCulture("yyyy-MM-dd");

            return datePattern.Parse(input).Value;
        }

        public static string RetrieveValue(this XElement xInput, string name)
        {
            if (string.IsNullOrEmpty(name))
                throw new ArgumentException("Unable to search for empty attribute name");

            var value = xInput.Attribute(name)?.Value;

            if (value == null)
                throw new ArgumentException($"No attribute with name '{name}' could be found");

            return value;
        }

        public static string RetrieveValue(this IEnumerable<ITag> tags, string name)
        {
            if (String.IsNullOrEmpty(name))
                throw new ArgumentException("Unable to search for tag with empty name");

            var tag = tags
                .SingleOrDefault(t => t.Ident == name);

            if (tag == null)
                throw new ArgumentException($"No tag with name '{name}' could be found");

            return tag.Value;
        }

        #region Retrieve Attributes

        public static bool TryRetrieveAttributeAsWeekday(this XElement xInput, string name, out IsoDayOfWeek value)
        {
            if (xInput.TryRetrieveAttribute(name, out XAttribute attribute)
            && Enum.TryParse(attribute.Value, out value))
            {
                return true;
            }

            value = IsoDayOfWeek.None;

            return false;
        }

        public static bool TryRetrieveAttribute(this XElement xInput, string name, out XAttribute attribute)
        {
            Guard.AgainstNull(xInput, nameof(xInput));
            Guard.AgainstNull(name, nameof(name));

            attribute = xInput
                .Attributes(name)
                .SingleOrDefault();

            return attribute != null;
        }

        public static int RetrieveAttributeAsInt(this XElement xInput, string name)
        {
            var attribute = RetrieveAttribute(xInput, name);

            return int.Parse(attribute.Value);
        }

        public static LocalTime RetrieveAttributeAsLocalTime(this XElement xInput, string name)
        {
            var attribute = RetrieveAttribute(xInput, name);

            var localTimePattern = LocalTimePattern.CreateWithInvariantCulture("HH:mm");
            var parseResult = localTimePattern.Parse(attribute.Value);

            return parseResult.Value;
        }

        public static TimeSpan RetrieveAttributeAsTimeSpan(this XElement xInput, string name)
        {
            var attribute = RetrieveAttribute(xInput, name);

            return System.Xml.XmlConvert.ToTimeSpan(attribute.Value);
        }

        public static Period RetrieveAttributeAsPeriod(this XElement xInput, string name)
        {
            var attribute = RetrieveAttribute(xInput, name);

            var localTimePattern = PeriodPattern.NormalizingIsoPattern;
            var parseResult = localTimePattern.Parse(attribute.Value);

            return parseResult.Value;
        }

        public static LocalDate RetrieveAttributeAsLocalDate(this XElement xInput, string name)
        {
            var attribute = RetrieveAttribute(xInput, name);

            var localTimePattern = LocalDatePattern.CreateWithInvariantCulture("yyyy-MM-dd");
            var parseResult = localTimePattern.Parse(attribute.Value);

            return parseResult.Value;
        }

        public static IsoDayOfWeek RetrieveAttributeAsIsoDayOfWeek(this XElement xInput, string name)
        {
            var attribute = RetrieveAttribute(xInput, name);

            if (!Enum.TryParse(attribute.Value, out IsoDayOfWeek result))
            {
                throw new ArgumentException($"Invalid day: {attribute}");
            }
            

            return result;
        }

        public static string RetrieveAttributeValue(this XElement xInput, string name)
        {
            var attribute = RetrieveAttribute(xInput, name);

            return attribute.Value;
        }

        public static XAttribute RetrieveAttribute(this XElement xInput, string name)
        {
            Guard.AgainstNull(xInput, nameof(xInput));
            Guard.AgainstNull(name, nameof(name));

            var attribute = xInput.Attribute(name);

            if (attribute == null)
            {
                throw new KeyNotFoundException($"No Attribute found matching name '{name}'");
            }

            return attribute;
        }

        public static bool HasAttribute(this XElement input, string name)
        {
            return input?.Attribute(name) != null;
        }

        #endregion
    }
}
