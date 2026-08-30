using BepInEx;
using Bunject;
using Bunject.Internal;
using Bunject.Levels;
using Bunject.Monitoring;
using Characters.Bunny.Data;
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
  public class BunjectExtractor : BaseUnityPlugin, IMonitor
  {
    public const string pluginGuid = "sergedev.bunject.extractor";
    public const string pluginName = "Bunject Extractor";
    public const string pluginVersion = "1.0.9";

    public static string rootDirectory = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "EXTRACTED");

    public void Awake()
    {
      Logger.LogInfo($"Bunject Extractor Plugin Awakened. v{pluginVersion}");

      if (!Directory.Exists(rootDirectory))
        Directory.CreateDirectory(rootDirectory);

      BunjectAPI.RegisterPlugin(this);
    }

    public LevelsList LoadEmergencyLevelsList(LevelsList original)
    {
      return original;
    }

    public void OnAssetsLoaded() { }

    public void OnBunnyCapture(BunnyIdentity bunnyIdentity, bool wasHomeCapture) { }

    public LevelObject OnLevelLoad(LevelObject level, LevelIdentity identity)
    {
      // Serialize and output level
      if (!identity.Bunburrow.IsCustomBunburrow())
      {
        var targetFile = Path.Combine(rootDirectory, LevelIndicatorGenerator.GetShortLevelIndicator(identity) + ".level");
        if (!File.Exists(targetFile))
        {
          File.WriteAllText(targetFile, level.Content);
        }
      }

      return level;
    }

    public string OnLevelTitle(string title, LevelIdentity identity, bool useWhite)
    {
      return title;
    }

    public void OnMainMenu() { }

    public void OnProgressionLoaded(GeneralProgression progression) { }

    public void OnShowCredits() { }

    public void OnPowerTile(global::Tiling.Behaviour.PowerUnlockTile tile, LevelIdentity identity, Action dismiss) { }

    public bool TryResolvePowerTileSprite(global::Tiling.Behaviour.PowerUnlockTile tile, LevelIdentity identity, out UnityEngine.Tilemaps.TileBase sprite)
    {
      sprite = null;
      return false;
    }
  }
}
