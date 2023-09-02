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
}
