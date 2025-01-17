﻿using BepInEx.Logging;
using Bunject.Internal;
using Bunject.NewYardSystem.Levels;
using Bunject.NewYardSystem.Model;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bunject.NewYardSystem.Internal
{
  internal class CustomBunburrowCache
  {
    private string cachePath;

    public List<CustomBunburrowModel> CustomBurrows;

    public CustomBunburrowCache()
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
          CustomBurrows = new JsonSerializer().Deserialize(reader, typeof(List<CustomBunburrowModel>)) as List<CustomBunburrowModel>;
        }
      }
      else
      {
        CustomBurrows = new List<CustomBunburrowModel>();
      }
    }

    public CustomBunburrowModel CacheBunburrow(BNYSModBunburrow bunburrow)
    {
      var burrow = new CustomBunburrowModel { World = bunburrow.WorldName,  Name = bunburrow.LocalName, Prefix = bunburrow.WorldPrefix, Indicator = bunburrow.LocalIndicator };
      CustomBurrows.Add(burrow);
      return burrow;
    }

    public void SaveCache()
    {
      using (var writer = new StreamWriter(cachePath))
      {
        new JsonSerializer().Serialize(writer, CustomBurrows);
      }
    }
  }
}
