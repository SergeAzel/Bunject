using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Bunject.NewYardSystem.Utility
{
  public static class UriExtensions
  {
    public static string Load(this Uri uri)
    {
      Console.WriteLine("Loading: " + uri);
      Console.WriteLine(System.Environment.StackTrace);
      using (var client = new WebClient())
      {
        return client.DownloadString(uri);
      }
    }

    public static T Load<T>(this Uri uri) where T : class
    {
      Console.WriteLine("Loading: " + uri);
      Console.WriteLine(System.Environment.StackTrace);
      using (var client = new WebClient())
      {
        using (var reader = new StreamReader(client.OpenRead(uri)))
        {
          return new JsonSerializer().Deserialize(reader, typeof(T)) as T;
        }
      }
    }
  }
}
