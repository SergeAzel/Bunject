using Bunject.Internal;
using Bunject.Levels;
using HarmonyLib;
using Misc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tiling.Behaviour;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace Bunject.Patches.BunburrowEntryTilePatches
{
  [HarmonyPatch(typeof(BunburrowEntryTile), MethodType.Constructor, new Type[] {typeof(Vector2Int), typeof(int)})]
  internal class ConstructorPatch
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

  [HarmonyPatch(typeof(BunburrowEntryTile), nameof(BunburrowEntryTile.HandleLevelReset))]
  internal class HandleLevelResetPatch
  {
    private static void Postfix(BunburrowEntryTile __instance)
    {
      if (__instance.Bunburrow != null && __instance.Bunburrow.Value.IsCustomBunburrow())
      {
        if (__instance.Bunburrow.Value.GetModBunburrow() is IModBunburrow modBunburrow)
        {
          if (!modBunburrow.HasEntrance)
          {
            GameManager.TileMaps.ExitsTileMap.SetTile(__instance.Position.ToVector3Int(), null);
          }
        }
      }
    }
  }
}
