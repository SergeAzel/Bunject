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
}
