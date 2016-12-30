using System.Runtime.Serialization;
using ArangoDB.Client;
using Scheduler.Persistance;

namespace Scheduler
{
    public class Event : Vertex
    {
        private string _location;

        public string Location
        {
            get { return _location; }
            set
            {
                _location = value;
                SetDirty();
            }
        }

        private string _title;

        public string Title
        {
            get { return _title; }
            set
            {
                _title = value;
                SetDirty();
            }
        }

        [IgnoreDataMember]
        public Scheduler.Serials Serials { get; set; }

        public override SaveResult Save(IArangoDatabase db)
        {
            foreach (var serial in Serials)
            {
                var serialResult = serial.Save(db);

                if (serialResult != SaveResult.Success)
                    return serialResult;
            }

            var result = Save<Event>(db);

            if (result != SaveResult.Success)
                return result;

            return SaveResult.Success;
        }
    }
}
