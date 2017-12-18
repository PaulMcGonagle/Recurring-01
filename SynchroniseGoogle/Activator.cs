using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Calendar.v3;
using Google.Apis.Calendar.v3.Data;
using Google.Apis.Services;
using Google.Apis.Util.Store;
using NodaTime;
using Scheduler;
using Event = Google.Apis.Calendar.v3.Data.Event;

namespace SynchroniseGoogle
{
    public class Activator
    {
        private static readonly string[] Scopes = { CalendarService.Scope.Calendar };
        private const string ApplicationName = "Google Calendar API .NET Quickstart";
        private const string CalendarId = "recurring.user.01@gmail.com";

        public void AddEvents(IEnumerable<IEvent> events, IClock clock)
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
            request.TimeMin = DateTime.Now;
            request.ShowDeleted = false;
            request.SingleEvents = true;
            request.MaxResults = 10;
            request.OrderBy = EventsResource.ListRequest.OrderByEnum.StartTime;

            var calendarEventsToDelete = new List<string>();
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

                    calendarEventsToDelete.Add(calendarEvent.Id);
                }
            }
            else
            {
                Console.WriteLine("No upcoming events found.");
            }

            foreach (var eventToDelete in calendarEventsToDelete)
            {
                var requestDelete = service.Events.Delete(CalendarId, eventToDelete);

                requestDelete.Execute();
            }
            Console.Read();

            foreach (var @event in events)
            {
                foreach (var episode in @event.Serials.SelectMany(s => s.ToVertex.GenerateEpisodes(clock)))
                {
                    var newEvent = new Event
                    {
                        Creator = new Event.CreatorData {Email = "recurring.user.01@gmail.com"},
                        Start = new EventDateTime {DateTime = episode.Start.ToDateTimeUtc()},
                        End = new EventDateTime {DateTime = episode.End.ToDateTimeUtc()},
                        Summary = @event.Title,
                        Location = @event.Location.ToVertex.Address,
                    };

                    var request2 = service.Events.Insert(newEvent, "recurring.user.01@gmail.com");

                    request2.Execute();
                }
            }
        }
    }
}
