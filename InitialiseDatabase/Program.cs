﻿using System;
using System.Linq;
using NodaTime;
using NodaTime.Testing;
using Scheduler;
using Scheduler.Persistance;
using Scheduler.Users;
using Event = Scheduler.Event;

namespace InitialiseDatabase
{
    internal class Program
    {
        public static void Main(string[] args)
        {
            var db = SchedulerDatabase.Database.Retrieve();
            var clock = new FakeClock(new Instant(DateTime.Now.Ticks));

            var generated = new School.Generate()
                .WithTimetable();

            var location = new Location.Builder {Address = "Hampden Gurney Primary"}.Build();

            var events = generated
                .Vertexs
                .OfType<ISerial>()
                .Select(serial => new Event.Builder
                {
                    Serial = serial,
                    Location = location,
                    Title = serial.GetTagValue("name"),
                }.Build())
                .ToList();

            var activator = new SynchroniseGoogle.Activator();

            activator.AddEvents(events, clock);
        }
    }
}