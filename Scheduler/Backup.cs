﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ArangoDB.Client;
using NodaTime;
using Scheduler.Persistance;

namespace Scheduler
{
    public class Backup : Vertex
    {
        public NodaTime.Instant Created { get; set; }

        public string Content { get; set; }

        public Type Type;
        public string TypeName => Type.Name;
        public string SourceKey { get; set; }
        public string SourceId { get; set; }
        public string SourceRev { get; set; }

        public static Backup Create(IClock clock, Vertex source)
        {
            var backup = new Backup
            {
                Created = clock.Now,

                Type = source.GetType(),

                SourceKey = source.Key,
                SourceId = source.Id,
                SourceRev = source.Rev,
            };

            return backup;
        }

        public override SaveResult Save(IArangoDatabase db, IClock clock)
        {
            return Save(new Func<SaveResult>[]
            {
                () => Save<Backup>(db),
                () => base.Save(db, clock),
            });
        }
    }
}