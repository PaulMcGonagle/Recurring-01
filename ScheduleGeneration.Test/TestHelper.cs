using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ArangoDB.Client;
using Moq;

namespace ScheduleGeneration.Test
{
    public static class TestHelper
    {
        public static Mock<IDocumentIdentifierResult> MockInsertSuccess
        {
            get
            {
                var result = new Mock<IDocumentIdentifierResult>();

                result.Object.Id = Guid.NewGuid().ToString();
                result.Object.Key = Guid.NewGuid().ToString();
                result.Object.Rev = Guid.NewGuid().ToString();

                return result;
            }
        }
    }
}
