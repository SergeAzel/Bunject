using BepInEx.Logging;
using Bunburrows;
using Bunject.Internal;
using Bunject.Patches.BunburrowExtensionPatches;
using Characters.Bunny.Data;
using HarmonyLib;
using Levels;
using Misc;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Bunject.Patches.GeneralProgressionPatches
{
  [HarmonyPatch(typeof(GeneralProgression), nameof(GeneralProgression.FreeAllNonVoidBunnies))]
  internal class FreeAllNonVoidBunniesPatches
  {
    // quick patch to force UI update on bun release
    private static void Postfix()
    {
      BunnyReleaser.NotifyReleased();
    }
  }

  [HarmonyPatch(typeof(GeneralProgression), nameof(GeneralProgression.FreeBunniesFromBunburrow))]
  internal class FreeBunniesFromBunburrowPatches
  {
    private static void Postfix()
    {
      BunnyReleaser.NotifyReleased();
    }
  }

  [HarmonyPatch(typeof(GeneralProgression), nameof(GeneralProgression.HandleElevatorUnlock))]
  internal class HandleElevatorUnlockPatch
  {
    public static void Postfix(GeneralProgression __instance)
    {
      var identity = GameManager.LevelStates.CurrentLevelState.LevelIdentity;
      if (ElevatorManager.ElevatorUnlock(identity, out var elevatorData))
			{
        if (!__instance.UnlockedElevators.ContainsEquatable(elevatorData))
        {
          Traverse.Create(__instance).Field<List<string>>("unlockedElevators").Value.Add(elevatorData);
        }
      }
    }
  }

  [HarmonyPatch(typeof(GeneralProgression), nameof(GeneralProgression.GetNonVoidBunniesSpreadSort))]
  internal class GetNonVoidBunniesSpreadSortPatch
  {
    // Injecting custom bunnies into GetNonVoidBunniesSpreadSort
    public static List<BunnyIdentity> Postfix(List<BunnyIdentity> values, GeneralProgression __instance)
    {
      foreach (var customBurrow in BunburrowManager.Bunburrows)
      {
        if (customBurrow.IsCustom)
        {
          var burrow = (Bunburrow) customBurrow.ID;
          values.AddRange(__instance.GetCapturedBunniesFromBunburrow(burrow));
        }
      }
      values.Sort((l,r) => l.InitialDepth < r.InitialDepth ? -1 : 1);

      return values;
    }
  }

  [HarmonyPatch(typeof(GeneralProgression), nameof(GeneralProgression.GetNonVoidBunniesCount))]
  internal class GetNonVoidBunniesCountPatch
  {
    // Removing Custom bunnies from GetNonVoidBunniesCount
    // This is only called from the vanilla "StartAllRelease" code, and it gets stuck with custom buns.
    public static bool Prefix(ref int __result, GeneralProgression __instance)
    {
      int num = 0;
      foreach (var bunnyIdentity in __instance.CapturedBunnies)
      {
        if (!bunnyIdentity.Bunburrow.IsCustomBunburrow() && bunnyIdentity.Bunburrow.IsNonVoidBunburrow()) // Check if open?
        {
          num++;
        }
      }
      __result = num;
      return false;
    }
  }
}
