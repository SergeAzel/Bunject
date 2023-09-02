using Bunburrows;
using HarmonyLib;
using Levels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;

namespace Bunject.Patches.LevelBuilderPatches
{
  [HarmonyPatch]
  internal class BuildNewLevelPatch
  {
    static MethodInfo TargetMethod()
    {
      return typeof(LevelBuilder).GetMethod("BuildNewLevel", new Type[] { typeof(LevelObject), typeof(BunburrowStyle), typeof(bool) });
    }

    static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
    {
      // Goal of transpiler is to replace a Chars[1] with a Substring(1), so that it parses the full index of our target bunburrow
      // Not just the first character
      MethodInfo startsWith = typeof(String).GetMethod("StartsWith", new Type[] { typeof(string) });
      MethodInfo get_Chars = typeof(String).GetProperty("Chars").GetGetMethod();
      MethodInfo char_ToString = typeof(Char).GetMethod("ToString", new Type[] { } );

      int detectionStage = 0;
      foreach (var instructionIterate in instructions)
      {
        var instruction = instructionIterate;

        switch (detectionStage)
        {
          case 0:
            if (instruction.opcode == OpCodes.Ldstr && (string)instruction.operand == "N")
            {
              //Found our N
              detectionStage = 1;
            }
            break;
          case 1:
            if (instruction.Calls(startsWith))
            {
              // Found starts with - now we're committed, no resetting from here
              detectionStage = 2;
            }
            else
            {
              // reset
              detectionStage = 0;
            }
            break;
          case 2:
            if (instruction.Calls(get_Chars))
            {
              // Found getChars which we wish to replace
              detectionStage = 3;
              // Replace function call
              instruction = CodeInstruction.Call(typeof(String), "Substring", new Type[] { typeof(int) });
            }
            break;
          case 3:
            // skip until we find ToString - which we also skip, but then continue processing the rest of the IL
            if (instruction.Calls(char_ToString))
            {
              // From here on, we move on as normal
              detectionStage = 4;
            }
            continue;
        }

        yield return instruction;
      }
    }
  }
}
