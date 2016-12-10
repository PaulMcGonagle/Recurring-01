using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ArangoDB.Client;

namespace Scheduler.Users
{
    public class Profile : PersistableEntity
    {
        public string Forename { get; set; }
        public string Surname { get; set; }
        public string Email { get; set; }
        public string TimeZoneProvider { get; set; }

        public Profile Save(IArangoDatabase db)
        {
            Save<Profile>(db);

            return this;
        }
    }
}
