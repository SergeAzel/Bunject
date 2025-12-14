using BepInEx;
using Bunject.Internal;
using Bunject.Levels;
using Bunject.Menu;
using Bunject.Monitoring;
using Bunject.Patches.SaveFileManipulationUtilityPatches;
using Bunject.Tiling;
using HarmonyLib;
using Menu;
using Saving;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Bunject
{
  public class BunjectAPI
  {
    static BunjectAPI()
    {
      Instance = new BunjectAPI();

      var harmony = new Harmony("bunject.API");
      harmony.PatchAll(System.Reflection.Assembly.GetAssembly(typeof(BunjectAPI)));
    }

    internal static BunjectAPI Instance { get; private set; }

    internal static ForwardingBunjector Forward { get; private set; } = new ForwardingBunjector();

    internal static IReadOnlyList<IBunjectorPlugin> Bunjectors { get => Instance.bunjectors; }

    internal static IEnumerable<ITileSource> TileSources { get => Instance.bunjectors.OfType<ITileSource>(); }

    internal static IEnumerable<IMonitor> Monitors { get => Instance.bunjectors.OfType<IMonitor>(); }

    internal static IEnumerable<IMenuSource> MenuOptions { get => Instance.bunjectors.OfType<IMenuSource>(); }

    public static void RegisterPlugin(IBunjectorPlugin bunjector)
    {
      Instance.bunjectors.Add(bunjector);
    }

    public static void RegisterBunburrow(IModBunburrow modBunburrow)
    {
      BunburrowManager.RegisterBurrow(modBunburrow);
    }

    public static void RegisterElevator(int bunburrowID, int depth)
    {
      BunburrowManager.RegisterElevator(bunburrowID, depth);
    }

    public static void ClearRegisters()
    {
      BunburrowManager.ClearRegisters();
    }

    public static Action BeginLoadingPluginSave(string pluginName, string saveName)
    {
      var menu = GameObject.FindObjectOfType<MenuController>();
      if (menu == null)
        return null;

      new Traverse(menu).Field<Camera>("mainCamera").Value.backgroundColor = new Color(0.97f, 0.94f, 0.94f);
      menu.ShowLoadingScreen();


      return () => LoadSave(pluginName, saveName);
    }

    private static bool LoadSave(string pluginName, string saveName)
    {
      SaveFileCustomData.CustomSavePath = SaveFileModUtility.GetPluginSaveFilePath(pluginName, saveName);
      SaveFileCustomData.CustomSaveBackupPath = SaveFileModUtility.GetPluginSaveBackupFilePath(pluginName, saveName);
      SaveFileCustomData.CustomDeletedSavePath = SaveFileModUtility.GetPluginSaveDeletedFilePath(pluginName, saveName);
      SaveFileCustomData.CustomOldSaveDataPath = SaveFileCustomData.CustomSavePath + ".old";

      EnsureFileExists(SaveFileCustomData.CustomSavePath);
      EnsureFileExists(SaveFileCustomData.CustomSaveBackupPath);
      EnsureFileExists(SaveFileCustomData.CustomDeletedSavePath);
      EnsureFileExists(SaveFileCustomData.CustomOldSaveDataPath);
    
      SaveFileManipulationUtility.TryLoadSave(SaveFileCustomData.CustomSaveFileIndex);

      SceneManager.LoadSceneAsync("Game");
      return true;
    }

    private static void EnsureFileExists(string path)
    {
      if (!File.Exists(path))
      {
        new FileInfo(path).Directory.Create();
        File.WriteAllBytes(path, new byte[0]);
      }
    }


    private List<IBunjectorPlugin> bunjectors;

    private BunjectAPI()
    {
      bunjectors = new List<IBunjectorPlugin>();
    }
  }
}
