using Bunject.Internal;
using HarmonyLib;
using Misc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tiling.Behaviour;
using UnityEngine.Tilemaps;

namespace Bunject.Patches.PowerUnlockTilePatches
{
  [HarmonyPatch(typeof(PowerUnlockTile), "HandlePaquerettePassAsSpecificFloor")]
  internal static class HandlePaquerettePassAsSpecificFloorPatch
  {
    private static bool Prefix(PowerUnlockTile __instance)
    {
      var identity = GameManager.LevelStates.CurrentLevelState?.LevelIdentity;
      if (identity == null)
        return true;

      var here = identity.Value;

      void Dismiss()
      {
        if (GameManager.LevelStates.CurrentLevelState?.LevelIdentity.Equals(here) == true)
          Traverse.Create(__instance).Method("UpdateTileVisuals").GetValue();
      }

      BunjectAPI.Forward.OnPowerTile(__instance, here, Dismiss);

      return !here.Bunburrow.IsCustomBunburrow();
    }
  }

  [HarmonyPatch(typeof(PowerUnlockTile), "UpdateTileVisuals")]
  internal static class UpdateTileVisualsPatch
  {
    private static void Postfix(PowerUnlockTile __instance)
    {
      var identity = GameManager.LevelStates.CurrentLevelState?.LevelIdentity;
      if (identity == null || !identity.Value.Bunburrow.IsCustomBunburrow())
        return;

      if (BunjectAPI.Forward.TryResolvePowerTileSprite(__instance, identity.Value, out var sprite))
        GameManager.TileMaps.RocksTileMap.SetTile(__instance.Position.ToVector3Int(), sprite);
    }
  }
}
