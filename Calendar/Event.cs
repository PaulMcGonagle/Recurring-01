using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyCalendar
{
    public class Event
    {
        public string Title { get; set; }
        public string Location { get; set; }

        public Scheduler.ISerial Serials { get; set; }
    }
}
