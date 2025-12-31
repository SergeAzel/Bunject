using HarmonyLib;
using Pause;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bunject.Patches.PauseControllerPatches
{
  [HarmonyPatch(typeof(PauseController), nameof(HandleBackToMenuConfirm))]
  internal static class HandleBackToMenuConfirm
  {
    internal static void Postfix()
    {
      BunjectAPI.Forward.OnMainMenu();
    }
  }
}
