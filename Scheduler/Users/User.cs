﻿using Scheduler.Persistance;

namespace Scheduler.Users
{
    public class User : Vertex, IUser
    {
        public string Forename { get; set; }
        public string Surname { get; set; }
    }
}
