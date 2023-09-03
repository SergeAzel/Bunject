using Bunject.Internal;
using Characters.Bunny;
using Characters.Bunny.Data;
using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bunject.Patches.ReleasedBunnyControllerPatches
{
  // this is a temporary stopover to make releasing work.
  // TODO properly animate released buns instead of bypassing.  Oh well.
  [HarmonyPatch(typeof(ReleasedBunnyController), nameof(ReleasedBunnyController.Initialize))]
  internal class InitializePatch
  {
    private static bool Prefix(ReleasedBunnyController __instance, BunnyIdentity bunnyIdentity, BunniesReleaseAnimator bunniesReleaseAnimator)
    {
      if ((int)bunnyIdentity.Bunburrow < BunburrowManager.CustomBunburrowThreshold)
        return true;

      //Console.WriteLine("BUNJECT - Releasing custom bun");

      // Consider it done
      bunniesReleaseAnimator.NotifyBunnyInHole(__instance);

      UnityEngine.Object.Destroy((UnityEngine.Object) __instance.gameObject);

      return false;
    }
  }
}
