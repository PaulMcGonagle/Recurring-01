using System;
using System.Linq;
using System.Net;
using ArangoDB.Client;

namespace SchedulerDatabase
{
    public static class Database
    {
        public static string DatabaseName { get; private set; } = "Scheduling";

        private static bool _isInitialised;

        public static void Initialise(string databaseName = null)
        {
            if (!string.IsNullOrEmpty(databaseName))
                DatabaseName = databaseName;

            ArangoDatabase.ChangeSetting(s =>
            {
                Console.WriteLine("ChangeSetting");
                s.Url = "http://localhost:8529";
                s.Database = DatabaseName;

                // you can set other settings if you need
                s.Credential = new NetworkCredential("root", "arango123");
                s.SystemDatabaseCredential = new NetworkCredential("root", "arango123");
            });

            using (var db = ArangoDatabase.CreateWithSetting())
            {
                var collectionNames = db.ListCollections().Select(s => s.Name).ToArray();

                foreach (var collectionName in collectionNames)
                {
                    db.DropCollection(collectionName);
                }

                db.CreateCollection("Backup");
                db.CreateCollection("Event");
                db.CreateCollection("Profile");
                db.CreateCollection("Organisation");
                db.CreateCollection("Location");
                db.CreateCollection("DateRange");
                db.CreateCollection("TimeRange");
                db.CreateCollection("Date");
                db.CreateCollection("Tag");
                db.CreateCollection("Serial");
                db.CreateCollection("CompositeSchedule");
                db.CreateCollection("ByDayOfMonth");
                db.CreateCollection("ByDayOfYear");
                db.CreateCollection("ByWeekday");
                db.CreateCollection("ByWeekdays");
                db.CreateCollection("DateList");
                db.CreateCollection("SingleDay");
                db.CreateCollection("Episode");
                db.CreateCollection("GeneratedDate");
                db.CreateCollection("GeneratedEvent");
                db.CreateCollection("Edge", type: CollectionType.Edge);
                db.CreateCollection("Relation", type: CollectionType.Edge);
            }

            _isInitialised = true;
        }

        public static IArangoDatabase Retrieve()
        {
            if (!_isInitialised)
            {
                Initialise(null);
            }

            return ArangoDatabase.CreateWithSetting();
        }
    }
}
