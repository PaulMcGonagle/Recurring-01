﻿using Scheduler.Persistance;
using System.Collections.Generic;
using System.Xml.Linq;
using NodaTime;

namespace Generators
{
    public interface IGeneratorX
    {
        IVertex Generate(XElement xInput, IDictionary<string, IVertex> caches, string elementsName = null, IClock clock = null);
    }
}
