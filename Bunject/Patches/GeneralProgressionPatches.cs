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
  [HarmonyPatch(typeof(GeneralProgression), nameof(GeneralProgression.GetVoidlessPillarsProgress))]
  internal class GetVoidlesssPillarsProgressPatch
  {
    static MethodInfo IsVoidBunburrow = typeof(BunburrowExtension).GetMethod(nameof(BunburrowExtension.IsVoidBunburrow));

    private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
    {
      foreach (var instruction in instructions)
      {
        if (instruction.Calls(IsVoidBunburrow))
        {
          yield return CodeInstruction.Call(typeof(CustomBunburrowExtension), nameof(CustomBunburrowExtension.IsVoidOrCustomBunburrow));
        }
        else
        {
          yield return instruction;
        }
      }
    }
  }

  [HarmonyPatch(typeof(GeneralProgression), nameof(GeneralProgression.GetNonVoidBunniesCount))]
  internal class GetNonVoidBunniesCountPatches
  {
    static MethodInfo IsNonVoidBunburrow = typeof(BunburrowExtension).GetMethod(nameof(BunburrowExtension.IsNonVoidBunburrow));

    private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
    {
      foreach (var codeInstruction in instructions)
      {
        if (codeInstruction.Calls(IsNonVoidBunburrow))
        {
          yield return CodeInstruction.Call(typeof(CustomBunburrowExtension), nameof(CustomBunburrowExtension.IsCoreBunburrow));
          continue;
        }
        yield return codeInstruction;
      }
    }
  }

  [HarmonyPatch(typeof(GeneralProgression), nameof(GeneralProgression.FreeAllNonVoidBunnies))]
  internal class FreeAllNonVoidBunniesPatches
  {
    // quick patch to force UI update on bun release
    private static void Postfix()
    {
      BunnyReleaser.NotifyReleased();
    }
  }

  /*
  [HarmonyPatch(typeof(GeneralProgression), nameof(GeneralProgression.GetCapturedBunniesFromBunburrow))]
  internal class GetCapturedBunniesFromBunburrowPatch
  {
    private static List<BunnyIdentity> Postfix(List<BunnyIdentity> __result, GeneralProgression __instance, Bunburrow bunburrow)
    {
      Console.WriteLine($"BUNJECT - {nameof(GetCapturedBunniesFromBunburrowPatch)} - count: {__result.Count} - BurrowID {(int)bunburrow}");
      foreach (var id in __result)
        Console.WriteLine($"BUNJECT - brw: {(int)id.Bunburrow}, lvl: {id.LevelID}, str: {id.GetIdentityString()}");

      foreach (var capturedBunny in __instance.CapturedBunnies)
        Console.WriteLine($"BUNJECT - CAPBUN: {(int)capturedBunny.Bunburrow}, lvl: {capturedBunny.LevelID}, STR: {capturedBunny.GetIdentityString()}");

      return __result;
    }
  }*/

  /*
  [HarmonyPatch(typeof(GeneralProgression), nameof(GeneralProgression.GetBunniesCountByBunburrow))]
  internal class GetBunniesCountByBunburrowPatches
  {
    private static BunniesCount Postfix(BunniesCount __result, GeneralProgression __instance, Bunburrow bunburrow)
    {
      Console.WriteLine($"BUNJECT - {nameof(GetBunniesCountByBunburrowPatches)} - count: {__result.RegularBunniesCount} - BurrowID {(int)bunburrow}");
      return __result;
    }
  }*/

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
