using Bunject.Internal;
using Characters.Bunny.Data;
using HarmonyLib;
using State;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bunject.Patches.LevelStatePatches
{
  [HarmonyPatch(typeof(LevelState), nameof(LevelState.HandleBunnyCapture))]
  internal class LevelStatePatch_HandleBunnyCapture
  {
    private static void Postfix(BunnyCaptureData bunnyCaptureData)
    {
      BunjectAPI.Forward.OnBunnyCapture(bunnyCaptureData.BunnyIdentity, !bunnyCaptureData.WasInfluencedByStacking);
    }
  }
}
