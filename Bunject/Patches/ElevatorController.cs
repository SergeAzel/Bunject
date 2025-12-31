using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bunject.Patches.ElevatorControllerPatches
{
  [HarmonyPatch(typeof(ElevatorController), nameof(ElevatorController.StartElevating))]
  internal static class StartElevating
  {
    internal static void Prefix(bool displayCredits)
    {
      if (displayCredits)
      {
        BunjectAPI.Forward.OnShowCredits();
      }
    }
  }
}
