using System.Collections.Generic;
using NodaTime;

namespace Scheduler.Generation
{
    public interface IGeneratedEvent
    {
        IEpisodes Episodes { get; set; }
        Instant Time { get; set; }

        void Generate(IClock clock, Event source);
    }
}