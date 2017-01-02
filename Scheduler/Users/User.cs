﻿using Scheduler.Persistance;

namespace Scheduler.Users
{
    public class User : Vertex
    {
        public string Forename { get; set; }
        public string Surname { get; set; }

        private EdgeVertexs<Organisation> Organisations { get; set; }
    }
}