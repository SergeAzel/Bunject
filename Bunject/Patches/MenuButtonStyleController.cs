using HarmonyLib;
using Misc;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using TMPro;

namespace Bunject.Patches.MenuButtonStyleControllerPatches
{
  [HarmonyPatch(typeof(MenuButtonStyleController), nameof(Awake))]
  internal class Awake
  {
    private static void Prefix(MenuButtonStyleController __instance)
    {
      var buttonController = Traverse.Create(__instance).Field<ButtonController>("buttonController");
      var textComponent = Traverse.Create(__instance).Field<TextMeshProUGUI>("textComponent");

      if (buttonController.Value == null)
      {
        Debug.WriteLine("buttonController not set by serialization - setting now");
        buttonController.Value = __instance.GetComponent<ButtonController>();
      }

      if (textComponent.Value == null)
      {
        Debug.WriteLine("textComponent not set by serialization - setting now");
        textComponent.Value = __instance.GetComponent<TextMeshProUGUI>();
      }
    }
  }
}
