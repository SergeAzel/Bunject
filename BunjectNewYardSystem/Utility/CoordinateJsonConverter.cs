using Bunject.NewYardSystem.Model;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bunject.NewYardSystem.Utility
{
  public class CoordinateJsonConverter : JsonConverter<SurfaceCoordinate>
  {
    public override SurfaceCoordinate ReadJson(JsonReader reader, Type objectType, SurfaceCoordinate existingValue, bool hasExistingValue, JsonSerializer serializer)
    {
      Console.WriteLine("Starting deserialize");
      switch (reader.TokenType)
      {
        case JsonToken.StartObject:
          Console.WriteLine("Deserializing Object");
          var token = JToken.Load(reader);
          return new SurfaceCoordinate()
          {
            Hole = token["Hole"]?.Values<int>()?.ToArray(),
            Sign = token["Sign"]?.Values<int>()?.ToArray(),
            NoSign = token.Value<bool?>("NoSign") ?? false
          };
        case JsonToken.StartArray:
          Console.WriteLine("Deserializing Array");
          var array = JArray.Load(reader);
          return new SurfaceCoordinate(array.Values<int>().ToArray());
        default:
          Console.WriteLine("Uh oh!");
          Console.WriteLine(Enum.GetName(typeof(JsonToken), reader.TokenType));
          break;
      }
      return new SurfaceCoordinate();
    }

    public override void WriteJson(JsonWriter writer, SurfaceCoordinate value, JsonSerializer serializer)
    {
      throw new NotImplementedException();
    }

    public override bool CanWrite => false;
  }
}
