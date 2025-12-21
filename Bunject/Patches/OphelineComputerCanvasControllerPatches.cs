using Bunburrows;
using Bunject.Computer;
using Computer;
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

namespace Bunject.Patches.OphelineComputerCanvasControllerPatches
{

  [HarmonyPatch(typeof(OphelineComputerCanvasController), nameof(Awake))]
  internal static class Awake
  {
    internal static void Postfix(OphelineComputerCanvasController __instance)
    {
      ComputerTabManager.Instantiate(__instance);
    }
  }

  [HarmonyPatch(typeof(OphelineComputerCanvasController), nameof(Open))]
  internal static class Open
  {
    internal static void Postfix(OphelineComputerCanvasController __instance)
    {
      ComputerTabManager.instance?.OnComputerOpen();
    }
  }

  [HarmonyPatch(typeof(OphelineComputerCanvasController), nameof(SwitchTab))]
  internal class SwitchTab
  {
    static MethodInfo StartLevelTransition = typeof(GameManager).GetMethod("StartLevelTransition", BindingFlags.NonPublic | BindingFlags.Static);

    internal static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
    {
      int state = 0;
      foreach (var instruction in instructions)
      {
        switch (state)
        {
          case 0:
            if (instruction.opcode == System.Reflection.Emit.OpCodes.Switch)
            {
              Console.WriteLine("Alter switch value");
              // Adjust the switch value for the moment
              yield return CodeInstruction.Call(typeof(SwitchTab), nameof(SwitchTab.ChangeSwitchValue));

              Console.WriteLine("SWITCH:");
              Console.WriteLine(instruction);
              Console.WriteLine("END SWITCH");

              // Adjust the switch itself to call a new label
              state++;
            }
            break;
          case 1:
            // Create the default case statement oh how fun

            Console.WriteLine("Load 'this'");
            yield return new CodeInstruction(OpCodes.Ldarg_0); // load "this"

            Console.WriteLine("Next thing?");
            yield return CodeInstruction.LoadField(typeof(OphelineComputerCanvasController), "currentTabIndex");

            yield return new CodeInstruction(OpCodes.Ldarg_0); // load "this"
            yield return CodeInstruction.LoadField(typeof(OphelineComputerCanvasController), "availableTabs");
            yield return CodeInstruction.Call(typeof(SwitchTab), nameof(SwitchToCustomTab));
            state++;
            break;
        }
        yield return instruction;
      }
      Console.WriteLine("All the way through");
    }

    private static int ChangeSwitchValue(int value)
    {
      // Need to move the value outside of the switch range (0 to 2)
      // Map Tab May not be unlocked, so 2 could be a custom tab instead
      if (value == 2 && !GameManager.GeneralProgression.IsMapUnlocked)
      {
        // 2 is a custom tab, escape the switch
        return 3;
      }

      return value;
    }

    private static void SwitchToCustomTab(int currentTabIndex, List<ComputerTabController> availableTabs)
    {
      var core = availableTabs[currentTabIndex];
      ComputerTabManager.instance.SelectTab(core.ToCustom());
    }
  }
}
