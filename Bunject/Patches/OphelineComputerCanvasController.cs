using Bunject.Computer;
using Bunject.Utility;
using Characters.Bunny.Data;
using Computer.Opheline;
using Computer.Opheline.Tabs;
using HarmonyLib;
using Levels;
using Misc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using TMPro;
using UnityEngine;

namespace Bunject.Patches.OphelineComputerCanvasControllerPatches
{
  [HarmonyPatch(typeof(OphelineComputerCanvasController), nameof(Awake))]
  internal static class Awake
  {
    internal static void Postfix(OphelineComputerCanvasController __instance)
    {
      if (__instance.gameObject.GetComponent<CustomComputerPagesListBehavior>() == null)
      {
        var tabs = Traverse.Create(__instance).Field<OphelineComputerTabsList>("ophelineComputerTabsList").Value;
        var customPages = __instance.gameObject.AddComponent<CustomComputerPagesListBehavior>();
        customPages.Initialize(tabs);
      }
    }
  }

  [HarmonyPatch(typeof(OphelineComputerCanvasController), nameof(SwitchTab))]
  internal static class SwitchTab
  {
    private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
    {
      var codeMatcher = new CodeMatcher(instructions);

      codeMatcher.MatchForward(false, new CodeMatch(OpCodes.Callvirt, typeof(OphelineComputerTabsList).GetProperties().Single(p => p.GetIndexParameters().Length > 0).GetMethod))
        .InsertAndAdvance(new CodeInstruction(OpCodes.Ldarg_0), Transpilers.EmitDelegate<Func<OphelineComputerTabsList, OphelineComputerTab, OphelineComputerCanvasController, OphelineComputerTabController>>(IndexExtendedPages))
        .RemoveInstruction();

      var instructionlist = codeMatcher.InstructionEnumeration().ToList();

      return instructionlist;
    }


    [HarmonyReversePatch(type: HarmonyReversePatchType.Snapshot)]
    public static void Invoke(OphelineComputerCanvasController instance, OphelineComputerTab newTab, bool isFromOpen = false)
    {
      //Emtpy - populated with private method contents
    }

    private static OphelineComputerTabController IndexExtendedPages(OphelineComputerTabsList list, OphelineComputerTab tab, OphelineComputerCanvasController canvasController)
    {
      var pageList = canvasController.gameObject.GetComponent<CustomComputerPagesListBehavior>();
      return pageList.GetByIndex(tab);
    }
  }

  [HarmonyPatch(typeof(OphelineComputerCanvasController), nameof(Open))]
  internal static class Open
  {
    private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
    {
      var codeMatcher = new CodeMatcher(instructions);

      codeMatcher.MatchForward(false, new CodeMatch(OpCodes.Callvirt, typeof(OphelineComputerTabsList).GetMethod(nameof(OphelineComputerTabsList.GetEnumerator))))
        .InsertAndAdvance(new CodeInstruction(OpCodes.Ldarg_0), Transpilers.EmitDelegate<Func<OphelineComputerTabsList, OphelineComputerCanvasController, IEnumerator<OphelineComputerTabController>>>(GetExtendedPages))
        .RemoveInstruction();

      var instructionlist = codeMatcher.InstructionEnumeration().ToList();

      return instructionlist;
    }

    private static IEnumerator<OphelineComputerTabController> GetExtendedPages(OphelineComputerTabsList list, OphelineComputerCanvasController canvasController)
    {
      var pageList = canvasController.gameObject.GetComponent<CustomComputerPagesListBehavior>();
      return pageList.GetPageControllers().GetEnumerator();
    }
  }
}
