using Bunburrows;
using Bunject.Internal;
using Bunject.Utility;
using HarmonyLib;
using Newtonsoft.Json;
using Saving;
using Saving.Architecture;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Bunject.Saving
{
  public static class SaveHandler
  {
    private static Func<string> GetRootSaveDataPath = Traverse.Create(typeof(SaveFileManipulationUtility))
      .Method(nameof(GetRootSaveDataPath), new Type[] { typeof(bool) })
      .Curry<Traverse, string>((t) => (string)t.GetValue(true));

    public static void RestructureLoadedProgress(SaveData gameSaveData, int saveIndex)
    {
      var modSaveState = LoadModState(saveIndex);

      if (modSaveState != null)
      {
        var remappingDictionary = new Dictionary<Bunburrow, Bunburrow>();
        foreach (var saveBunburrow in modSaveState.CustomBunburrows)
        {
          var registeredBunburrow = BunburrowManager.Bunburrows.FirstOrDefault(bb => bb.ModBunburrow.Name == saveBunburrow.Name);



          if (registeredBunburrow.ID != saveBunburrow.ID)
            remappingDictionary.Add(saveBunburrow.ID.ToBunburrow(), registeredBunburrow.ID.ToBunburrow());
        }

        if (remappingDictionary.Count > 0)
        {
          var progress = gameSaveData.ProgressSaveData;

          //Collect ALL the BunnyIdentitySaveData instances... if we missed any, add them
          var bunnyHashSet = new HashSet<BunnyIdentitySaveData>();
          bunnyHashSet.UnionWith(progress.CapturedBunnies);
          bunnyHashSet.UnionWith(progress.ExistingCouples.Select(ec => ec.Left));
          bunnyHashSet.UnionWith(progress.ExistingCouples.Select(ec => ec.Right));
          bunnyHashSet.UnionWith(progress.ExistingGoldenCouples.Select(ec => ec.Left));
          bunnyHashSet.UnionWith(progress.ExistingGoldenCouples.Select(ec => ec.Right));
          bunnyHashSet.UnionWith(progress.SeenBunnies);
          bunnyHashSet.UnionWith(progress.HistoryCapturedBunnies);
          bunnyHashSet.UnionWith(progress.HomeCapturedBunnies);
          bunnyHashSet.UnionWith(gameSaveData.LevelStartStateSaveData.InitialBunniesData.Select(bd => bd.BunnyInfo.BunnyIdentity));
          bunnyHashSet.UnionWith(gameSaveData.LevelStartStateSaveData.InitialBunniesData.SelectMany(bd => bd.BunnyInfo.PileBunniesIdentities));

          //Collect ALL level instances... if we missed any, add them
          var levelHashSet = new HashSet<LevelIdentitySaveData>();
          levelHashSet.Add(gameSaveData.CurrentLevel);
          levelHashSet.UnionWith(progress.LevelsCompletedOnce);
          levelHashSet.UnionWith(progress.LevelsVisited);

          foreach (var bunny in bunnyHashSet)
          {
            if (remappingDictionary.ContainsKey(bunny.Bunburrow))
            {
              Traverse.Create(bunny).Field("Bunburrow").SetValue(remappingDictionary[bunny.Bunburrow]);
            }
          }

          foreach (var level in levelHashSet)
          {
            if (remappingDictionary.ContainsKey(level.Bunburrow))
            {
              Traverse.Create(level).Property("Bunburrow").SetValue(remappingDictionary[level.Bunburrow]);
            }
          }
        }
      }
    }

    private static string GetModSaveDataPath(int saveIndex)
    {
      return GetRootSaveDataPath() + "/bunject-" + SaveFileManipulationUtility.IndexToSaveLetter(saveIndex) + ".json";
    }

    public static BunjectSaveState LoadModState(int saveIndex)
    {
      var path = GetModSaveDataPath(saveIndex);
      if (File.Exists(path))
      {
        using (var reader = new StreamReader(path))
        {
          return new JsonSerializer().Deserialize(reader, typeof(BunjectSaveState)) as BunjectSaveState;
        }
      }
      /*
      else if old BNYS save cache exists
      {
        synthesize a BunjectSaveState from that
      }*/
      return null;
    }

    public static void SaveModState(BunjectSaveState state, int saveIndex)
    {
      var path = GetModSaveDataPath(saveIndex);
      using (var writer = new StreamWriter(path))
      {
        new JsonSerializer().Serialize(writer, state);
      }
    }

    /*
    [Obsolete]
    public static Something LoadOldBNYSCache()
    {

    }
    */
  }
}
