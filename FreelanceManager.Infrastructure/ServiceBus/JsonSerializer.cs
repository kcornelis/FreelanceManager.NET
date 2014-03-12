using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace FreelanceManager.Infrastructure.ServiceBus
{
    public class JsonSerializer
    {
        private static readonly JsonSerializerSettings Settings = new JsonSerializerSettings
        {
            TypeNameHandling = TypeNameHandling.Objects,
            ContractResolver = new CustomJsonConverter()
        };

        public static string Serialize(object o)
        {
            return JsonConvert.SerializeObject(o, Formatting.None, Settings);
        }

        public static object Deserialize(string s)
        {
            return JsonConvert.DeserializeObject(s, Settings);
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
