using System.Collections.Generic;
using Scheduler.Persistance;

namespace Scheduler.Users
{
    public interface IOrganisation : IVertex
    {
        List<IEvent> Events { get; set; }
        EdgeVertex<ILocation> Location { get; set; }
        string Title { get; set; }
    }
}