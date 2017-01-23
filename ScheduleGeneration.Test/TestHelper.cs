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

                result.SetupGet(r => r.Id).Returns(Guid.NewGuid().ToString());
                result.SetupGet(r => r.Key).Returns(Guid.NewGuid().ToString());
                result.SetupGet(r => r.Rev).Returns(Guid.NewGuid().ToString());

                return result;
            }
        }
    }
}
