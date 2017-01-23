﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Scheduler.Persistance;

namespace Scheduler
{
    public class SaveException : Exception
    {
        private readonly string _message;

        public SaveException(
            Vertex.SaveResult saveResult,
            System.Type type,
            string message)
        {
            SaveResult = saveResult;
            Type = type;
            _message = message;
        }

        public Vertex.SaveResult SaveResult { get; protected set; }
        public Type Type { get; protected set; }

        public new string Message => $"SaveException, SaveResult={SaveResult}, Type={Type}, Message={_message}";
    }
}
