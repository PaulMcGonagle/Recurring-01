﻿using System.Collections.Generic;
using System.Linq;
using ArangoDB.Client;
using NodaTime;

namespace Scheduler
{
    public class Serials : List<Serial>, ISerials
    {
        public IEpisodes Episodes
        {
            get
            {
                var episodes = new Episodes();

                episodes.AddRange(this
                    .SelectMany(ce => ce.Episodes)
                    .Select(e => e.ToVertex));

                return episodes;
            }
        }

        public void Save(IArangoDatabase db, IClock clock)
        {
            foreach (var serial in this)
            {
                serial.Save(db, clock);
            }
        }
    }
}
