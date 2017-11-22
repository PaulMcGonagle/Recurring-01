using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Threading.Tasks;

namespace CoreLibrary
{
    public static class Transfer
    {
        public static string Serialize(object toSerialize)
        {
            var stream1 = new MemoryStream();
            var ser = new DataContractJsonSerializer(toSerialize.GetType());
            ser.WriteObject(stream1, toSerialize);

            stream1.Position = 0;
            var sr = new StreamReader(stream1);
            return sr.ReadToEnd();
        }

        public static T Deserialize<T>(string toDeserialize)
        {
            var ms = new MemoryStream(Encoding.UTF8.GetBytes(toDeserialize));
            var serializer = new DataContractJsonSerializer(typeof(T));
            var deserialized = (T)serializer.ReadObject(ms);
            ms.Close();

            return deserialized;
        }
    }
}
