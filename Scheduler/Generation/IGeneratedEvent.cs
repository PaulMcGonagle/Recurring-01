using System.Collections.Generic;
using NodaTime;

namespace Scheduler.Generation
{
    public interface IGeneratedEvent
    {
        IEpisodes Episodes { get; set; }
        Instant Time { get; set; }
    }
}