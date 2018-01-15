using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Calendar.v3;
using Google.Apis.Calendar.v3.Data;
using Google.Apis.Requests;
using Google.Apis.Services;
using Google.Apis.Util.Store;
using NodaTime;
using Scheduler;
using Scheduler.ScheduleInstances;
using Event = Google.Apis.Calendar.v3.Data.Event;

namespace SynchroniseGoogle
{
    public class Activator
    {
        private static readonly string[] Scopes = { CalendarService.Scope.Calendar };
        private const string ApplicationName = "Google Calendar API .NET Quickstart";
        private const string CalendarId = "recurring.user.01@gmail.com";

        public /*async*/ void AddEvents(IEnumerable<IEvent> events, IClock clock)
        {
            UserCredential credential;

            using (var stream =
                new FileStream("client_secret.json", FileMode.Open, FileAccess.Read))
            {
                var credPath = System.Environment.GetFolderPath(
                    System.Environment.SpecialFolder.Personal);
                credPath = Path.Combine(credPath, ".credentials/calendar-dotnet-quickstart.json");

                credential = GoogleWebAuthorizationBroker.AuthorizeAsync(
                    GoogleClientSecrets.Load(stream).Secrets,
                    Scopes,
                    "user",
                    CancellationToken.None,
                    new FileDataStore(credPath, true)).Result;
                Console.WriteLine("Credential file saved to: " + credPath);
            }

            // Create Google Calendar API service.
            var service = new CalendarService(new BaseClientService.Initializer()
            {
                HttpClientInitializer = credential,
                ApplicationName = ApplicationName,
            });

            // Define parameters of request.
            var request = service.Events.List("primary");
            //request.TimeMin = DateTime.Now.AddMonths(-4);
            request.ShowDeleted = false;
            request.SingleEvents = true;
            request.MaxResults = 1000;
            //request.OrderBy = EventsResource.ListRequest.OrderByEnum.StartTime;

            var calendarEventsToDelete = new HashSet<string>();
            // List events.
            var calendarEvents = request.Execute();
            Console.WriteLine("Previous events:");
            if (calendarEvents.Items != null && calendarEvents.Items.Count > 0)
            {
                foreach (var calendarEvent in calendarEvents.Items)
                {
                    var when = calendarEvent.Start.DateTime.ToString();
                    if (string.IsNullOrEmpty(when))
                    {
                        when = calendarEvent.Start.Date;
                    }
                    Console.WriteLine("{0} ({1})", calendarEvent.Summary, when);

                    calendarEventsToDelete.Add(calendarEvent.RecurringEventId ?? calendarEvent.Id);
                }
            }
            else
            {
                Console.WriteLine("No upcoming events found.");
            }

            var batchRequest = new BatchRequest(service);

            foreach (var eventToDelete in calendarEventsToDelete)
            {
                var deleteRequest = service.Events.Delete(CalendarId, eventToDelete);

                deleteRequest.Execute();

                Thread.Sleep(20);

                //batchRequest.Queue<Event>(service.Events.Delete(CalendarId, eventToDelete),
                //    (content, error, i, message) =>
                //    {
                //        Console.WriteLine($"deleted {eventToDelete}");
                //    });
            }

            //await batchRequest.ExecuteAsync();

            //Console.Read();

            foreach (var @event in events)
            {
                foreach (var serial in @event.Serials.Select(s => s.ToVertex))
                {
                    var scheduleInstance = serial
                        .EdgeSchedule
                        .Schedule
                        .ScheduleInstance;

                    var recurrence2 = new List<string>{ "RRULE:FREQ=WEEKLY;BYDAY=MO,TU;UNTIL=20180701T170000Z", };//;BYDAY:MO

                    var episodes = serial
                        .GenerateEpisodes(clock)
                        .ToList();

                    episodes
                        .Sort();

                    var recurrence = episodes
                        .Skip(1)
                        .Select(episode => $"RDATE;VALUE=DATE:{ConvertToRRuleDateTime(episode.Start.ToDateTimeUtc())}")
                        .ToList();

                    var episodeFirst = episodes
                        .Min()
                    ?? throw new Exception("No episodes were generated");

                    var newEvent = new Event
                    {
                        Creator = new Event.CreatorData {Email = "recurring.user.01@gmail.com"},
                        Start = new EventDateTime
                        {
                            DateTime = episodeFirst.Start.ToDateTimeUtc(),
                            TimeZone = serial.TimeZoneProvider,
                        },
                        End = new EventDateTime
                        {
                            DateTime = episodeFirst.End.ToDateTimeUtc(),
                            TimeZone = serial.TimeZoneProvider,
                        },
                        Summary = @event.Title,
                        Location = @event.Location.ToVertex.Address,
                        Recurrence = recurrence,
                    };

                    var request2 = service.Events.Insert(newEvent, "recurring.user.01@gmail.com");

                    request2.Execute();
                }
            }
        }

        private string ConvertToRRuleByDay(IsoDayOfWeek isoDayOfWeek)
        {
            switch (isoDayOfWeek)
            {
                case IsoDayOfWeek.Monday:
                    return "MO";
                case IsoDayOfWeek.Tuesday:
                    return "TU";
                case IsoDayOfWeek.Wednesday:
                    return "WE";
                case IsoDayOfWeek.Thursday:
                    return "TH";
                case IsoDayOfWeek.Friday:
                    return "FR";
                case IsoDayOfWeek.Saturday:
                    return "SA";
                case IsoDayOfWeek.Sunday:
                    return "SU";
                default:
                    throw new ArgumentException($"Unexpected day {isoDayOfWeek}", nameof(isoDayOfWeek));
            }
        }

        private static string ConvertToRRuleDateTime(IDate date)
        {
            return date.Value.ToString("yyyyMMddT000000Z", CultureInfo.InvariantCulture.DateTimeFormat);
        }

        private static string ConvertToRRuleDateTime(DateTime dateTime)
        {
            return dateTime.ToString("yyyyMMddTHHmmssZ", CultureInfo.InvariantCulture.DateTimeFormat);
        }

        private static string ConvertToRRuleDate(IDate date)
        {
            return date.Value.ToString("yyyyMMdd", CultureInfo.InvariantCulture.DateTimeFormat);
        }

        private IEnumerable<string> ConvertToRecurrence(IClock clock, IScheduleInstance scheduleInstance)
        {
            switch (scheduleInstance)
            {
                case ByWeekdays byWeekdays:

                    var sb = new StringBuilder();

                    var days = byWeekdays
                        .Weekdays
                        .Select(ConvertToRRuleByDay);

                    yield return "RRULE:FREQ=WEEKLY;"
                                 + $"BYDAY={string.Join(",", days)};"
                                 + $"UNTIL={ConvertToRRuleDateTime(byWeekdays.EdgeRangeDate.RangeDate.End.Date)}";

                    if (byWeekdays.Exclusions != null)
                    {
                        foreach (var exclusion in byWeekdays
                            ?.Exclusions)
                        {

                        }
                    }

                    break;

                default:

                    var recurrences = scheduleInstance
                        .Generate(clock)
                        .ToList();

                    foreach (var recurrence in recurrences)
                    {
                        var rRuleDate = ConvertToRRuleDate(recurrence);
                        yield return $"RDATE;VALUE=DATE:{rRuleDate}";
                    }

                    break;
            }
        }
    }
}
