using Bunject.Computer;
using Bunject.Utility;
using Computer.Opheline;
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

namespace Bunject.Patches.OphelineComputerCapturesTabControllerPatches
{
  [HarmonyPatch(typeof(OphelineComputerCapturesTabController), nameof(HandleOpen))]
  internal class HandleOpen 
  {
    internal static void Postfix(OphelineComputerCapturesTabController __instance)
    {
      try
      {
        GameObject objlog = __instance.gameObject;

        DebugUtil.WholeThing(objlog);
      }
      catch (Exception e)
      {
        Debug.LogException(e);
      }
    }
  }
}
