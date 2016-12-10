﻿using System.Runtime.Serialization;
using ArangoDB.Client;

namespace Scheduler
{
    public class Event : PersistableEntity
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
        public Scheduler.ISerial Serials { get; set; }

        public void Save(IArangoDatabase db)
        {
            Save<Event>(db);
        }
    }
}
