using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tiling.Behaviour;
using UnityEngine;

namespace Bunject.Patches.BunburrowEntryTilePatches
{
  [HarmonyPatch(typeof(BunburrowEntryTile), MethodType.Constructor, new Type[] {typeof(Vector2Int), typeof(int)})]
  internal class HandleLevelResetPatch
  {
    private static void Postfix(BunburrowEntryTile __instance)
    {
      var instance = Traverse.Create(__instance);
      var specialIndex = instance.Field<int>("specialIndex");
      if (specialIndex.Value > 10) // past the normal bounds and limits of level builder 
      {
        instance.Field<Bunburrows.Bunburrow?>("Bunburrow").Value = (Bunburrows.Bunburrow)specialIndex.Value;
        instance.Field("isUnlocked").SetValue(true);
        specialIndex.Value = -1;
      }
    }
  }
}
