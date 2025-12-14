using BepInEx;
using BepInEx.Logging;
using Bunburrows;
using Bunject;
using Bunject.Monitoring;
using Characters.Bunny.Data;
using Levels;
using UnityEngine;
using Bunject.Archipelago.Utils;
using Bunject.Archipelago.UI;
using Bunject.Archipelago.Archipelago;

namespace Bunject.Archipelago
{

  [BepInPlugin(PluginGUID, PluginName, PluginVersion)]
  public class ArchipelagoPlugin : BaseUnityPlugin, IMonitor
  {
    public const string PluginGUID = "sergedev.bunject.archipelago";
    public const string PluginName = "BunjectArchipelago";
    public const string PluginVersion = "1.0.0";

    public const string ModDisplayInfo = $"{PluginName} v{PluginVersion}";
    public const string APDisplayInfo = $"Archipelago v{ArchipelagoClient.APVersion}";
    public static ManualLogSource BepinLogger;
    public static ArchipelagoClient ArchipelagoClient;

    private static GameObject menuObject;
    private static GameObject disconnectOnQuit;

    public LevelsList LoadEmergencyLevelsList(LevelsList original)
    {
      return original;
    }

    public void OnAssetsLoaded() { }

    public void OnProgressionLoaded(GeneralProgression progression) { }

    public LevelObject StartLevelTransition(LevelObject level, LevelIdentity identity)
    {
      var levelString = identity.Bunburrow.ToIndicator() + "-" + identity.Depth;
      if (!ArchipelagoClient.HasToolsForLevel(levelString))
      {
        BepinLogger.LogInfo("Level still locked " + levelString);
        return LevelLocker.Lock(level);
      }
      return level;
    }


    public void OnBunnyCapture(BunnyIdentity bunny, bool wasHomeCapture)
    {
      ArchipelagoClient.NotifyBunnyCaptured(bunny.GetIdentityString(), wasHomeCapture);
    }


    public void OnMenuLoad()
    {
      if (menuObject == null)
      {
        menuObject = new GameObject();
        menuObject.AddComponent<MenuUI>();
        DontDestroyOnLoad(menuObject);
      }

      if (disconnectOnQuit == null)
      {
        disconnectOnQuit = new GameObject();
        disconnectOnQuit.AddComponent<ArchipelagoBehavior>();
        DontDestroyOnLoad(disconnectOnQuit);
      }
    }


    protected void Awake()
    {
      // Plugin startup logic
      BepinLogger = Logger;
      ArchipelagoClient = new ArchipelagoClient();
      ArchipelagoConsole.Awake();

      ArchipelagoConsole.LogMessage($"{ModDisplayInfo} loaded!");

      BunjectAPI.RegisterPlugin(this);
    }
  }
}