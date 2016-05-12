using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Reflection;

namespace SXCore.Common.Classes
{
    public abstract class JsonCreationConverter<T> : JsonConverter
    {
        protected abstract T Create(Type objectType, JObject jsonObject);

        public override bool CanConvert(Type objectType)
        {
            return typeof(T).IsAssignableFrom(objectType);
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            if (reader.TokenType == JsonToken.Null)
                return null;

            // Load JObject from stream
            JObject jObject = JObject.Load(reader);

            // Create target object based on JObject
            T target = Create(objectType, jObject);

            //Create a new reader for this jObject, and set all properties to match the original reader.
            JsonReader jObjectReader = jObject.CreateReader();
            jObjectReader.Culture = reader.Culture;
            jObjectReader.DateParseHandling = reader.DateParseHandling;
            jObjectReader.DateTimeZoneHandling = reader.DateTimeZoneHandling;
            jObjectReader.FloatParseHandling = reader.FloatParseHandling;

            // Populate the object properties
            serializer.Populate(jObjectReader, target);

            return target;

            //var jsonObject = JObject.Load(reader);
            //var target = Create(objectType, jsonObject);
            //serializer.Populate(jsonObject.CreateReader(), target);
            //return target;
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            JObject jo = new JObject();

            Type type = value.GetType();

            foreach (PropertyInfo prop in type.GetProperties())
            {
                if (prop.CanRead)
                {
                    object propVal = prop.GetValue(value, null);

                    if (propVal != null)
                    {
                        jo.Add(prop.Name, JToken.FromObject(propVal, serializer));
                    }
                    else
                    {
                        jo.Add(prop.Name, JValue.CreateNull());
                    }
                }
            }

            jo.WriteTo(writer);
        }

        //public bool ParseEnum<T>(string value, out T result)
        //    where T : struct, IConvertible
        //{
        //    if (!typeof(T).IsEnum)
        //        throw new ArgumentException("T must be an enumerated type");

        //    result = default(T);

        //    if (String.IsNullOrWhiteSpace(value))
        //        return false;

        //    T temporary;
        //    if (Enum.TryParse(value, true, out temporary))
        //    {
        //        if (Enum.GetValues(typeof(T)).Cast<T>().Any(v => (temporary & v) != 0))
        //            return true;

        //        return result;
        //    }
        //}
    }
}
