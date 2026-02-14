using HarmonyLib;
using System;
using System.Collections.Generic;
using Bunburrows;
using Bunject.Internal;
using UnityEngine;

namespace Bunject.Patches
{
  [HarmonyPatch(typeof(BunniesReleaseAnimator), "ConvertBunburrowToReleaseTargetsList")]
  internal class ConvertBunburrowToReleaseTargetsListPatches
  {
    // Return a release path for custom burrows
    private static bool Prefix(ref List<Vector2Int> __result, Bunburrow bunburrow)
    { 
      if ((int)bunburrow < BunburrowManager.CustomBunburrowThreshold)
        return true;
      var mod = bunburrow.GetModBunburrow();
      __result = mod.GetReleasePath() ?? new List<Vector2Int>();
      return false;
    }
  }
}
