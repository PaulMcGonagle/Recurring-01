using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Scheduler.Persistance;

namespace Scheduler.Users
{
    public class User : Vertex
    {
        public string Forename { get; set; }
        public string Surname { get; set; }

        private EdgeVertexs<Organisation> Organisations { get; set; }
    }
}
