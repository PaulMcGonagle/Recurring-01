using ArangoDB.Client;

namespace Scheduler
{
    public class Event : PersitableEntity
    {
        public string Title { get; set; }
        public string Location { get; set; }

        public Scheduler.ISerial Serials { get; set; }

        public void Save(IArangoDatabase db)
        {
            Save<Event>(db);
        }
    }
}
