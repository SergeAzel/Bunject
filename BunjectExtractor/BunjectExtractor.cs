using BepInEx;
using Bunject;
using Bunject.Internal;
using Bunject.Levels;
using Levels;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Bunject.Extractor
{

  [BepInPlugin(pluginGuid, pluginName, pluginVersion)]
  public class BunjectExtractor : BaseLevelSourcePlugin
  {
    public const string pluginGuid = "sergedev.bunject.extractor";
    public const string pluginName = "Bunject Extractor";
    public const string pluginVersion = "1.0.9";

    public static string rootDirectory = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "EXTRACTED");

    public override void Awake()
    {
      Logger.LogInfo($"Bunject Extractor Plugin Awakened. v{pluginVersion}");

      if (!Directory.Exists(rootDirectory))
        Directory.CreateDirectory(rootDirectory);

      BunjectAPI.RegisterPlugin(this);
    }

    // Exports core level data on level being loaded
    public override LevelObject StartLevelTransition(LevelObject original, LevelIdentity identity)
    {
      // Serialize and output level
      if (!identity.Bunburrow.IsCustomBunburrow())
      {
        var targetFile = Path.Combine(rootDirectory, LevelIndicatorGenerator.GetShortLevelIndicator(identity) + ".level");
        if (!File.Exists(targetFile))
        {
          File.WriteAllText(targetFile, original.Content);
        }
      }

      return original;
    }
  }
}
