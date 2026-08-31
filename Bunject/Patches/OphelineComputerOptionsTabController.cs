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
      var extraOption = __instance.GetComponent<ComputerExtraButtonBehavior>();
      if (extraOption == null)
      {
        extraOption = __instance.gameObject.AddComponent<ComputerExtraButtonBehavior>();
      }

      var traverse = Traverse.Create(__instance);
      var buttons = traverse.Field<List<ButtonController>>("buttons").Value;
      var buttonStyleControllers = traverse.Field<List<MenuButtonStyleController>>("buttonStyleControllers").Value;

      var visible = new List<CustomExtraButton>();
      foreach (var button in extraOption.Buttons)
      {
        var show = button.Page.ShouldShow();
        button.ChildObject.SetActive(show);
        if (show)
        {
          visible.Add(button);
        }
      }

      buttons.InsertRange(buttons.Count - 1, visible.Select(b => b.ButtonController));
      buttonStyleControllers.InsertRange(buttonStyleControllers.Count - 1, visible.Select(b => b.Style));
    }
  }
}
