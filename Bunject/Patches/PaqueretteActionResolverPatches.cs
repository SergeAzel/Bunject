using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;

namespace Bunject.Patches.PaqueretteActionResolverPatches
{
  //Total hack to ensure signs properly passthrough the burrow we need to clear out instead of relying on burrowstyle
  [HarmonyPatch(typeof(PaqueretteActionResolver), nameof(PaqueretteActionResolver.HandleTalkButtonPress))]
  internal class HandleTalkButtonPressPatches
  {
    public static Bunburrows.Bunburrow? targetBurrow;

    private static MethodInfo GetBunburrowUnlockStatus = typeof(GeneralProgression).GetProperty(nameof(GeneralProgression.BunburrowsUnlockStatus)).GetGetMethod();

    private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
    {
      int detectionState = 0;
      foreach (var instruction in instructions)
      {
        switch (detectionState)
        {
          case 0:
            if (instruction.Calls(GetBunburrowUnlockStatus))
              detectionState++;
            break;
          case 1:
            if (instruction.Branches(out Label? _))
            {
              yield return instruction;
              yield return new CodeInstruction(OpCodes.Ldloc_3);
              yield return CodeInstruction.Call(typeof(BunburrowSignController), "get_Bunburrow"); // Calling a getter.. I guess it worked
              yield return CodeInstruction.Call(typeof(HandleTalkButtonPressPatches), nameof(HandleTalkButtonPressPatches.ExtractBurrow));
              detectionState++;
              continue;
            }
            break;
        }
        yield return instruction;
      }
    }

    private static void ExtractBurrow(Bunburrows.Bunburrow bunburrow)
    {
      targetBurrow = bunburrow;
    }
  }
}
