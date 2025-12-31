using Bunject.Internal;
using Characters.Bunny;
using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bunject.Patches.MenuPatches
{
  [HarmonyPatch(typeof(global::Menu.MenuController), "Update")]
  internal class MenuPatches_Update
  {
    private static void Postfix(global::Menu.MenuController __instance)
    {
      if (__instance != null)
      {
        ((ForwardingBunjector)BunjectAPI.Forward).ShowOrHideMenu(__instance.IsReady && !__instance.IsInBunstack && !__instance.IsInOptions && !__instance.IsInSaveScreen);
      }
      else
      {
        ((ForwardingBunjector)BunjectAPI.Forward).ShowOrHideMenu(false);
      }
    }
  }
}
