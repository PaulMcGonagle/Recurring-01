using System.Collections.Generic;
using Scheduler.Persistance;

namespace Scheduler.Users
{
    public interface IOrganisation : IVertex
    {
        List<Event> Events { get; set; }
        EdgeVertex<Location> Location { get; set; }
        string Title { get; set; }
    }
}