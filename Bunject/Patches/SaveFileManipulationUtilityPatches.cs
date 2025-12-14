using HarmonyLib;
using Saving;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bunject.Patches.SaveFileManipulationUtilityPatches
{
  internal static class SaveFileCustomData
  {
    internal const int CustomSaveFileIndex = 999;

    internal static string CustomSavePath { get; set; }
    internal static string CustomSaveBackupPath { get; set; }
    internal static string CustomDeletedSavePath { get; set; }
    internal static string CustomOldSaveDataPath { get; set; }
  }

  [HarmonyPatch(typeof(SaveFileManipulationUtility), nameof(GetSaveDataPath))]
  internal class GetSaveDataPath
  {
    internal static bool Prefix(int saveIndex, bool platformSpecific, ref string __result)
    {
      if (saveIndex == SaveFileCustomData.CustomSaveFileIndex)
      {
        __result = SaveFileCustomData.CustomSavePath;
        return false; // skip
      }
      return true; // use original
    }
  }

  [HarmonyPatch(typeof(SaveFileManipulationUtility), nameof(GetBackupSaveDataPath))]
  internal class GetBackupSaveDataPath
  {
    internal static bool Prefix(int saveIndex, bool platformSpecific, ref string __result)
    {
      if (saveIndex == SaveFileCustomData.CustomSaveFileIndex)
      {
        __result = SaveFileCustomData.CustomSaveBackupPath;
        return false; // skip
      }
      return true; // use original
    }
  }

  [HarmonyPatch(typeof(SaveFileManipulationUtility), nameof(GetOldSaveDataPath))]
  internal class GetOldSaveDataPath
  {
    internal static bool Prefix(int saveIndex, bool platformSpecific, ref string __result)
    {
      if (saveIndex == SaveFileCustomData.CustomSaveFileIndex)
      {
        __result = SaveFileCustomData.CustomOldSaveDataPath;
        return false; // skip
      }
      return true; // use original
    }
  }

  [HarmonyPatch(typeof(SaveFileManipulationUtility), nameof(GetDeletedSaveDataPath))]
  internal class GetDeletedSaveDataPath
  {
    internal static bool Prefix(int saveIndex, bool platformSpecific, ref string __result)
    {
      if (saveIndex == SaveFileCustomData.CustomSaveFileIndex)
      {
        __result = SaveFileCustomData.CustomDeletedSavePath;
        return false; // skip
      }
      return true; // use original
    }
  }

  [HarmonyPatch(typeof(SaveFileManipulationUtility), nameof(HandleBackToMainMenu))]
  internal class HandleBackToMainMenu
  {
    internal static void Postfix()
    {
      SaveFileCustomData.CustomSavePath = null;
      SaveFileCustomData.CustomSaveBackupPath = null;
    }
  }
}
