using HarmonyLib;
using PlatformSpecific;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bunject.Patches.PlatformSpecificManagerPatches
{
  [HarmonyPatch(typeof(PlatformSpecificManager), nameof(PlatformSpecificManager.UnlockAchievement))]
  internal class UnlockAchievementPatches
  {
    static bool Prefix()
    {
      // FORCEFULLY prevent any achievement unlocking - mods should not ever unlock platform achievements.
      return false;
    }
  }

  [HarmonyPatch(typeof(PlatformSpecificManager), nameof(PlatformSpecificManager.CheckAchievementsOnStart))]
  internal class CheckAchievementsOnStartPatches
  {
    static bool Prefix()
    {
      // FORCEFULLY prevent any achievement unlocking - mods should not ever unlock platform achievements.
      return false;
    }
  }
}
