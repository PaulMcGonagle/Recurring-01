using Google.Apis.Auth.OAuth2;
using Google.Apis.Calendar.v3;
using Google.Apis.Calendar.v3.Data;
using Google.Apis.Services;
using Google.Apis.Util.Store;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using NodaTime;
using NodaTime.Testing;
using Scheduler;
using Scheduler.Persistance;
using Scheduler.Users;
using School;
using Event = Scheduler.Event;

namespace CalendarQuickstart
{
    class Program
    {
        public static void Main(string[] args)
        {
            var db = SchedulerDatabase.Database.Retrieve();
            var clock = new FakeClock(new Instant(DateTime.Now.Ticks));

            var generator = new Generate(db, clock);

            generator.Go();

            var location = new Location.Builder {Address = "Hampden Gurney Primary"}.Build();

            var events = generator
                .Vertexs
                .OfType<ISerial>()
                .Select(serial => new Event.Builder
                {
                    Serial = serial,
                    Location = location,
                    Title = serial.GetTagValue("name"),
                }.Build());

            var activator = new SynchroniseGoogle.Activator();

            activator.AddEvents(events, clock);
        }
    }
}