using HarmonyLib;
using Levels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Bunject.Patches.LevelLoaderPatches
{
  [HarmonyPatch(typeof(LevelLoader), "LoadLevel")]
  internal class LoadLevelPatch
  {
    // "^(S(?:{K})?|B(?:{[KB]*})?|E|F|D[0-9]|P[0-9]|N[0-9]+|\\!|Oph|X|C|T(?:{[URLDCTKB]+})?|W(?:{[URLD]*[0-9]?})?|R(?:{[URLD]*[0-9]?})?|EW|ER|PU|A|Y(?:{K})?)$";

    private static readonly string[] Separators = new string[4]
    {
      ",",
      "\r\n",
      "\r",
      "\n"
    };

    private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator il)
    {
      var regex_IsMatch = AccessTools.Method(typeof(Regex), nameof(Regex.IsMatch), new Type[] { typeof(string) });
      var codes = new List<CodeInstruction>(instructions);
      for (int i = 0; i < codes.Count; i++)
      {
        if (codes[i].Calls(regex_IsMatch))
        {
          if (codes[++i].Branches(out var l) && l is Label elseBlock)
          {
            Label ifBlock = il.DefineLabel();
            codes.Insert(i + 1, new CodeInstruction(OpCodes.Nop).WithLabels(ifBlock));
            codes.InsertRange(i, new List<CodeInstruction> {
              new CodeInstruction(OpCodes.Brtrue, ifBlock),
              new CodeInstruction(OpCodes.Ldarg_0),
              new CodeInstruction(OpCodes.Ldloc, 4),
              new CodeInstruction(OpCodes.Call, AccessTools.Method(typeof(LoadLevelPatch), nameof(LoadLevelPatch.CheckIfModSupportsTile))),
            });
            break;
          }
        }
      }
      return codes;
    }

    private static bool CheckIfModSupportsTile(string tile)
    {
      return BunjectAPI.Forward.SupportsTile(tile);
    }
  }
}
