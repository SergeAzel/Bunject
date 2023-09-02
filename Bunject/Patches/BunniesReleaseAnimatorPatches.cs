using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bunject.Patches
{
  [HarmonyPatch(typeof(BunniesReleaseAnimator), nameof(BunniesReleaseAnimator.StartRelease))]
  internal class BunniesReleaseAnimatorPatches
  {
    private static void Prefix()
    {
      Console.WriteLine("BUNJECT - StartRelease Begins");
    }

    private static void Postfix()
    {
      Console.WriteLine("BUNJECT - StartRelease Ends");
    }
  }
}
