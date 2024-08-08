using BepInEx.Logging;
using Bunject.Internal;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bunject.Saving
{
  internal class BNYSCustomBunburrowCache
  {
    private string cachePath;

    public List<BNYSCustomBunburrowModel> CustomBurrows;

    public BNYSCustomBunburrowCache()
    {
      var saveDirectory = SaveFileModUtility.GetRootSaveDataPath();
      cachePath = Path.Combine(saveDirectory, "mod-burrow-cache.json");

      if (!Directory.Exists(saveDirectory))
      {
        Directory.CreateDirectory(saveDirectory);
      }

      if (File.Exists(cachePath))
      {
        using (var reader = new StreamReader(cachePath))
        {
          CustomBurrows = new JsonSerializer().Deserialize(reader, typeof(List<BNYSCustomBunburrowModel>)) as List<BNYSCustomBunburrowModel>;
        }
      }
      else
      {
        CustomBurrows = new List<BNYSCustomBunburrowModel>();
      }
    }
  }
}
