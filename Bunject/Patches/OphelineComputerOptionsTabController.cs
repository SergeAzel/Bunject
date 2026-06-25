using Bunject.Computer;
using Computer.Opheline.Tabs;
using HarmonyLib;
using Levels;
using Misc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TMPro;
using UnityEngine;

namespace Bunject.Patches.OphelineComputerOptionsTabControllerPatches
{
  [HarmonyPatch(typeof(OphelineComputerOptionsTabController), nameof(OphelineComputerOptionsTabController.HandleOpen))]
  internal class HandleOpen
  {
    internal static void Postfix(OphelineComputerOptionsTabController __instance)
    {
      Debug.Log("We are in the postfix! instance? " + __instance != null);

      var extraOption = __instance.GetComponent<ComputerExtraButtonBehavior>();
      if (extraOption == null)
      {
        extraOption = __instance.gameObject.AddComponent<ComputerExtraButtonBehavior>();
      }

      Debug.Log("A");

      var traverse = Traverse.Create(__instance);

      Debug.Log("B");

      var supportTextComponent = traverse.Field<TextMeshProUGUI>("supportTextComponent").Value;

      Debug.Log("C");
      var buttons = traverse.Field<List<ButtonController>>("buttons").Value;

      Debug.Log("D");
      var buttonStyleControllers = traverse.Field<List<MenuButtonStyleController>>("buttonStyleControllers").Value;

      Debug.Log("E");
      buttons.InsertRange(buttons.Count - 1, extraOption.Buttons.Select(b => b.ButtonController));

      Debug.Log("F");
      buttonStyleControllers.InsertRange(buttonStyleControllers.Count - 1, extraOption.Buttons.Select(b => b.Style));
    }
  }
}
