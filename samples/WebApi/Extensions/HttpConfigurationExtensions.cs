using System.Collections.Generic;
using System.Web.Http;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Serialization;

namespace WebApi.Extensions
{
    public static class HttpConfigurationExtensions
    {
        public static void UseDefaultRoutes(this HttpConfiguration config)
        {
            config.Routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "{controller}/{id}",
                defaults: new {id = RouteParameter.Optional}
            );
        }

        public static void UseDefaultJsonConverter(this HttpConfiguration config)
        {
            config.Formatters.Remove(config.Formatters.XmlFormatter);
            config.Formatters.Remove(config.Formatters.FormUrlEncodedFormatter);
            config.Formatters.JsonFormatter.SerializerSettings = new JsonSerializerSettings
            {
                ContractResolver = new CamelCasePropertyNamesContractResolver(),
                Converters = new List<JsonConverter> { new StringEnumConverter(), new IsoDateTimeConverter() },
                NullValueHandling = NullValueHandling.Ignore
            };
        }
    }
}