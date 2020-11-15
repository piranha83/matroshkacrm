using System;
using System.Security.Claims;
using System.Threading.Tasks;
using Infrastructure.Enums;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using  Infrastructure.Interfaces;

namespace Web.Extensions
{
    public static class ControllerExtentsions
    {
        public static OkObjectResult Ok(this ControllerBase c, object value)
        {
            var res = c.Ok(value);
            /*res.Formatters.Add(new Microsoft.AspNetCore.Mvc.Formatters.J(
                new Newtonsoft.Json.JsonSerializerSettings() { Formatting = Newtonsoft.Json.Formatting.Indented },
                System.Buffers.ArrayPool<char>.Create()));*/
            return res;
        }

        public static T GetClaim<T>(this ControllerBase c, string claim)
            where T : struct
        {
            var value = c.User?.FindFirst(claim)?.Value;
            return string.IsNullOrWhiteSpace(value) == false
                ? (T)Convert.ChangeType(value, typeof(T))
                : default(T);
        }

        public static int GetUserId(this ControllerBase c)
            => GetClaim<int>(c, ClaimTypes.NameIdentifier);

        public static string GetUserName(this ControllerBase c)
            => c.User?.Identity?.Name;

        public static void SerCredential<TModel>(this ControllerBase c, TModel entity)
            where TModel: class
        {
            if (entity is ICreator creator)
            {
                creator.CreatedBy = c.GetUserName();
                creator.CreatorId = c.GetUserId();
                creator.CreatedDate = DateTime.UtcNow;
            }
            if (entity is IEditor editor)
            {
                editor.EditorName = c.GetUserName();
                editor.EditorId = c.GetUserId();
                editor.UpdateDate = DateTime.UtcNow;
            }
        }
    }

    public class WeirdNameSerializer : JsonConverter
    {
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            var xml = JsonConvert.SerializeObject(value);
            writer.WriteValue(xml);
            //serializer.Serialize(writer, value);
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            var json = serializer.Deserialize(reader, objectType);
            return json;
        }

        public override bool CanConvert(Type objectType) => true;
    }
}