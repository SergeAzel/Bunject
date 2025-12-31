using Computer;
using HarmonyLib;
using Items;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;

namespace Bunject.Archipelago.Patches.ComputerMapInfoControllerPatches
{
  [HarmonyPatch(typeof(ComputerMapInfoController), nameof(DisplayLevelInfo))]
  internal static class DisplayLevelInfo
  {
    internal static void Postfix(ComputerMapInfoController __instance)
    {
      if (ArchipelagoPlugin.Instance != null)
      {
        var levelItemTexts = Traverse.Create(__instance).Field<ItemListOf<TextMeshProUGUI>>("levelItemsCounters").Value;
        var levelItemObjects = Traverse.Create(__instance).Field<ItemListOf<GameObject>>("levelItemsGameObjects").Value;

        ArchipelagoPlugin.Instance.LockComputerVisuals(levelItemTexts, levelItemObjects);
      }
    }
  }
}
