using Bunburrows;
using Bunject.Computer;
using Bunject.Map;
using Characters.Bunny.Data;
using Computer;
using Computer.Opheline.Map;
using Computer.Opheline.Tabs;
using HarmonyLib;
using Levels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Bunject.Patches.OphelineComputerMapTabControllerPatches
{

  [HarmonyPatch(typeof(OphelineComputerMapTabController), nameof(OphelineComputerMapTabController.HandleOpen))]
  internal static class HandleOpenPatch 
  {
    internal static void Prefix()
    {
      var currentBurrow = GameManager.LevelStates.CurrentLevelState?.LevelIdentity.Bunburrow;
      if (currentBurrow.HasValue)
      {
        new MapContext(currentBurrow.Value);
      }
      else
      {
        MapContext.Instance?.Dispose();
      }
    }
  }

  [HarmonyPatch(typeof(OphelineComputerMapTabController), nameof(UpdateHeaderText))]
  internal static class UpdateHeaderText
  {
    internal static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
    {
      var codeMatcher = new CodeMatcher(instructions);

      codeMatcher.MatchForward(false, new CodeMatch(OpCodes.Callvirt, typeof(List<BunnyIdentity>).GetMethod(nameof(List<BunnyIdentity>.Add))))
        .SetInstruction(Transpilers.EmitDelegate<Action<List<BunnyIdentity>, BunnyIdentity>>(AddIfUnique));

      return codeMatcher.InstructionEnumeration();
    }

    private static void AddIfUnique(List<BunnyIdentity> list, BunnyIdentity identity)
    {
      if (!list.Any(i => i.Equals(identity)))
        list.Add(identity);
    }
  }

}
