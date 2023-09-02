using BepInEx;
using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SaveFileCleanup
{
  [BepInPlugin(pluginGuid, pluginName, pluginVersion)]
  public class SaveFileCleanupPlugin : BaseUnityPlugin
  {
    public const string pluginGuid = "sergedev.savefilecleanup";
    public const string pluginName = "Save File Cleanup"; 
    public const string pluginVersion = "1.0.9";

    public void Awake()
    {
      Console.WriteLine($"Save File Cleanup Awakened. v{pluginVersion}");
      Logger.LogInfo($"Save File Cleanup Awakened. v{pluginVersion}");

      var harmony = new Harmony("Save File Cleanup");
      harmony.PatchAll();
    }
  }
}
