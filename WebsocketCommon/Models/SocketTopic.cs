using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using WebsocketCommon.Interface;

namespace WebsocketCommon.Models
{
    public class SocketTopic : ISocketTopic
    {
        const string PathDelimeter = "/";

        public string Topic { get; set; }

        public IList<string> PathSegments
        {
            get => Topic.Split(PathDelimeter);
            set => Topic = string.Join(PathDelimeter, value.Select(e => 
                {
                    while (e.Length > 2 && e.Substring(e.Length - 1, 1) == PathDelimeter)
                    {
                        e = e.Remove(e.Length - 1, 1);
                    }
                    while (e.Substring(0, 1) == PathDelimeter)
                    {
                        e = e.Remove(0, 1);
                    }
                    return e; 
                }
            ));
        }

        public SocketTopic() { }

        public SocketTopic(string mainTopic) 
        {
            PathSegments = new[] { mainTopic };
        }

        public SocketTopic(string mainTopic, string secondaryTopic)
        {
            PathSegments = new[] { mainTopic, secondaryTopic };
        }

        public SocketTopic(string mainTopic, string param2, string param3)
        {
            PathSegments = new[] { mainTopic, param2, param3 };
        }

        public SocketTopic(IList<string> pathSegments)
        {
            PathSegments = pathSegments;
        }

        public SocketTopic(SocketTopic socketTopic, string addSegment)
        {
            var newArray = new string[socketTopic.PathSegments.Count + 1];
            socketTopic.PathSegments.CopyTo(newArray, 0);
            newArray[socketTopic.PathSegments.Count] = addSegment;
            PathSegments = newArray;
        }
    }


    public class SocketTopicConverter : JsonConverter
    {
        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(SocketTopic);
        }


        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            object rootObject = Activator.CreateInstance(objectType);
            JToken objJSON = JToken.ReadFrom(reader);

            foreach (var token in objJSON)
            {
                PropertyInfo propInfo = rootObject.GetType().GetProperty
                    (token.Path, BindingFlags.IgnoreCase | BindingFlags.Public |
                     BindingFlags.Instance);

                var propname = propInfo.Name;

                if (propInfo.CanWrite)
                {
                    var tk = token as JProperty;
                    if (tk.Value is JObject)
                    {
                        JValue val = tk.Value.SelectToken("value") as JValue;
                        propInfo.SetValue(rootObject, Convert.ChangeType
                         (val.Value, propInfo.PropertyType.UnderlyingSystemType), null);
                    }
                    else
                    {
                        propInfo.SetValue(rootObject, Convert.ChangeType
                          (tk.Value, propInfo.PropertyType.UnderlyingSystemType), null);
                    }
                }
            }

            return rootObject;
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            var item = (SocketTopic)value;
            writer.WriteValue(item.Topic);
            writer.Flush();
        }
    }
}
