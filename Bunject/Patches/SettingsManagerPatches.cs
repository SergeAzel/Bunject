using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bunject.Patches.SettingsManagerPatches
{
  [HarmonyPatch(typeof(SettingsManager), nameof(SettingsManager.HandleBoot))]
  internal class HandleBootPatches
  {
    private static void Postfix()
    {
      BunjectAPI.Forward.OnAssetsLoaded();
    }
  }
}
