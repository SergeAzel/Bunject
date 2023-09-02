using Bunburrows;
using Bunject.Internal;
using HarmonyLib;
using Levels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;

namespace Bunject.Patches.LevelIndicatorGeneratorPatches
{
  [HarmonyPatch(typeof(LevelIndicatorGenerator), nameof(LevelIndicatorGenerator.GetLongLevelIndicator))]
  internal class GetLongLevelIndicatorPatches
  {
    private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
    {
      foreach (var instruction in instructions)
      {
        if (instruction.opcode == OpCodes.Ldstr && (string)instruction.operand == "")
        {
          yield return CodeInstruction.Call(typeof(GameManager), "get_CurrentLevel");
          yield return CodeInstruction.LoadField(typeof(Level), nameof(Level.BaseData));
          yield return CodeInstruction.Call(typeof(LevelObject), "get_CustomNameKey");
        }
        else
        {
          yield return instruction;
        }
      }
    }
  }
  [HarmonyPatch(typeof(LevelIndicatorGenerator), nameof(LevelIndicatorGenerator.GetLevelBunburrowStyle))]
  internal class GetLevelBunburrowStylePatch
  {
    public static BunburrowStyle Postfix(BunburrowStyle __result, LevelIdentity levelIdentity)
    {
      if (levelIdentity.Bunburrow.IsCustomBunburrow())
      {
        return AssetsManager.LevelsLists[levelIdentity.Bunburrow.ToBunburrowName()][levelIdentity.Depth].BunburrowStyle;
      }
      return __result;
    }
  }
}
