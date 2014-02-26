using System;
using System.IO;
using System.Messaging;
using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace FreelanceManager.Infrastructure.ServiceBus
{
    public class JsonMessageFormatter<T> : IMessageFormatter
    {
        private static readonly JsonSerializerSettings Settings = new JsonSerializerSettings
        {
            TypeNameHandling = TypeNameHandling.Objects,
            ContractResolver = new CustomJsonConverter()
        };

        public bool CanRead(Message message)
        {
            if (message == null)
                throw new ArgumentNullException("message");

            var stream = message.BodyStream;

            return stream != null
                && stream.CanRead
                && stream.Length > 0;
        }

        public object Clone()
        {
            return new JsonMessageFormatter<T>();
        }

        public object Read(Message message)
        {
            if (message == null)
                throw new ArgumentNullException("message");

            if (CanRead(message) == false)
                return null;

            using (var reader = new StreamReader(message.BodyStream, Encoding.UTF8))
            {
                var json = reader.ReadToEnd();

                return JsonConvert.DeserializeObject<T>(json, Settings);
            }
        }

        public void Write(Message message, object obj)
        {
            if (message == null)
                throw new ArgumentNullException("message");

            if (obj == null)
                throw new ArgumentNullException("obj");

            string json = JsonConvert.SerializeObject(obj, Formatting.None, Settings);

            message.BodyStream = new MemoryStream(Encoding.UTF8.GetBytes(json));

            //Need to reset the body type, in case the same message
            //is reused by some other formatter.
            message.BodyType = 0;
        }

        public class CustomJsonConverter : DefaultContractResolver
        {
            protected override JsonProperty CreateProperty(System.Reflection.MemberInfo member, MemberSerialization memberSerialization)
            {
                var prop = base.CreateProperty(member, memberSerialization);

                // properties with only a getter should be ignored
                // properties with a private setter should be writable
                if (member.MemberType == System.Reflection.MemberTypes.Property)
                {
                    var setter = ((System.Reflection.PropertyInfo)member).GetSetMethod(true);
                    if (setter != null)
                    {
                        prop.Writable = true;
                    }
                    else
                    {
                        prop.Ignored = true;
                    }
                }

                return prop;
            }
        }
    }
}