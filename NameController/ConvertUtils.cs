using System.IO;
using System.Runtime.Serialization.Json;

namespace WebTask1.Controllers
{
    public static class ConvertUtils
    {
        public static byte[] ConvertObjectToJsonByteArray(object obj)
        {
            if (obj == null)
            {
                return null;
            }

            DataContractJsonSerializer serializer = new DataContractJsonSerializer(obj.GetType());
            using (MemoryStream memoryStream = new MemoryStream())
            {
                serializer.WriteObject(memoryStream, obj);
                return memoryStream.ToArray();
            }
        }
    }
}
