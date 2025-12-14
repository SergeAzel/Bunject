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
using Bunject.Menu;
using System;
using System.Collections;

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
    
    
    private ArchipelagoClient ArchipelagoClient = null;
    private ArchipelagoMenu Menu = null;


    public LevelsList LoadEmergencyLevelsList(LevelsList original)
    {
      return original;
    }

    public void OnAssetsLoaded() { }

    public void OnProgressionLoaded(GeneralProgression progression) { }

    public LevelObject OnLevelLoad(LevelObject level, LevelIdentity identity)
    {
      if (ArchipelagoClient != null)
      {
        var levelString = identity.Bunburrow.ToIndicator() + "-" + identity.Depth;
        if (!ArchipelagoClient.HasToolsForLevel(levelString))
        {
          BepinLogger.LogInfo("Level still locked " + levelString);
          return LevelLocker.Lock(level);
        }
      }
      return level;
    }

    public void OnBunnyCapture(BunnyIdentity bunny, bool wasHomeCapture)
    {
      if (ArchipelagoClient != null)
      {
        ArchipelagoClient.NotifyBunnyCaptured(bunny.GetIdentityString(), wasHomeCapture);
      }
    }

    protected void Awake()
    {
      // Plugin startup logic
      BepinLogger = Logger;

      ArchipelagoConsole.Awake();
      ArchipelagoConsole.LogMessage($"{ModDisplayInfo} loaded!");

      Menu = new ArchipelagoMenu(() => new GameObject().AddComponent<DummyBehavior>().StartCoroutine(TryConnect()));

      BunjectAPI.RegisterPlugin(this);
      BunjectAPI.RegisterPlugin(Menu);
    }

    public void OnMainMenu()
    {
      // We're back from the game - disconnect and reset any major changes
      if (ArchipelagoClient != null)
      {
        ArchipelagoClient.Dispose();
        ArchipelagoClient = null;
      }
    }

    private IEnumerator TryConnect()
    {
      BunjectAPI.BeginLoadingScreen();

      yield return null;

      try
      {
        // Create Client and Start Connection
        ArchipelagoClient = ArchipelagoClient.Connect(Menu.Uri, Menu.SlotName, Menu.Password);

        if (ArchipelagoClient != null)
        {
          // Get world id from client
          string saveName = GetSaveName(ArchipelagoClient); //TODO

          BunjectAPI.LoadSave("Archipelago", saveName);
        }
        else
        {
          BunjectAPI.CancelLoadingScreen();
        }
      }
      catch (Exception e)
      {
        Debug.LogException(e);

        BunjectAPI.CancelLoadingScreen();
      }
      yield return null;
    }

    private string GetSaveName(ArchipelagoClient client)
    {
      return $"{client.Seed}_{client.Slot}";
    }
  }
}