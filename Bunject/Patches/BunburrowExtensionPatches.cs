using Bunburrows;
using Bunject.Internal;
using Bunject.Map;
using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Assertions.Must;

namespace Bunject.Patches.BunburrowExtensionPatches
{
  [HarmonyPatch(typeof(BunburrowExtension), "ToBunburrow", new Type[] { typeof(string)})]
  internal class ToBunburrowStringPatch
  {
    public static Bunburrow Postfix(Bunburrow __result, string bunburrowName)
    {
      return ((Bunburrow?)BunburrowManager.Bunburrows.FirstOrDefault(bb => bb.ModBunburrow?.Name == bunburrowName)?.ID) ?? __result;
    }
  }

  [HarmonyPatch(typeof(BunburrowExtension), "ToBunburrow", new Type[] { typeof(int) })]
  // updates ToBunburrow, which indices burrows unusually for core burrows
  internal class ToBunburrowIntPatch
  {
    private static Bunburrow Postfix(Bunburrow __result, int bunburrowID)
    {
      if (((Bunburrow)bunburrowID).IsCustomBunburrow())
      {
        return (Bunburrow)bunburrowID;
      }
      return __result;
    }
  }

  [HarmonyPatch(typeof(BunburrowExtension), "ToBunburrowName")]
  internal class ToBunburrowNamePatch
  {
    private static string Postfix(string __result, Bunburrow bunburrow)
    {
      var bunb = bunburrow;
      if (bunb.IsCustomBunburrow())
      {
        return BunburrowManager.Bunburrows.FirstOrDefault(bb => bb.ID == (int)bunburrow)?.ModBunburrow.Name;
      }
      return __result;
    }
  }

  [HarmonyPatch(typeof(BunburrowExtension), "ToComparisonIndex")]
  internal class ToComparisonIndexPatch
  {
    private static int Postfix(int __result, Bunburrow bunburrow)
    {
      var bunb = bunburrow;
      if (bunb.IsCustomBunburrow())
      {
        return BunburrowManager.Bunburrows.FirstOrDefault(bb => bb.ID == (int)bunburrow)?.ComparisonIndex ?? __result;
      }
      return __result;
    }
  }

  [HarmonyPatch(typeof(BunburrowExtension), "ToIndicator")]
  internal class ToIndicatorPatch
  {
    private static string Postfix(string __result, Bunburrow bunburrow)
    {
      var bunb = bunburrow;
      if (bunb.IsCustomBunburrow())
      {
        return BunburrowManager.Bunburrows.FirstOrDefault(bb => bb.ID == (int)bunburrow)?.ModBunburrow?.Indicator;
      }
      return __result;
    }
  }


  [HarmonyPatch(typeof(BunburrowExtension), "IsNonVoidBunburrow")]
  internal class IsNonVoidBunburrowPatch
  {
    private static bool Postfix(bool __result, Bunburrow bunburrow)
    {
      var bunb = bunburrow;
      if (bunb.IsCustomBunburrow())
      {
        return !bunb.IsVoidBunburrow();
      }
      // redirect to the other function, which needs no extending
      return __result;
    }
  }

  [HarmonyPatch(typeof(BunburrowExtension), "IsVoidBunburrow")]
  internal class IsVoidBunburrowPatch
  {
    private static bool Postfix(bool __result, Bunburrow bunburrow)
    {
      // redirect to the other function, which needs no extending
      var bunb = bunburrow;
      if (bunb.IsCustomBunburrow())
      {
        return BunburrowManager.Bunburrows.FirstOrDefault(bb => bb.ID == (int)bunburrow)?.ModBunburrow?.IsVoid ?? __result;
      }
      return __result;
    }
  }

  [HarmonyPatch(typeof(BunburrowExtension), nameof(BunburrowExtension.GetBunburrowsInMapOrderList))]
  internal class GetBunburrowsInMapOrderListPatch
  {
    private static void Postfix(ref IEnumerable<Bunburrows.Bunburrow> __result)
    {
      if (MapContext.Instance != null)
      {
        __result = MapContext.Instance;
      }
    }
  }


  [HarmonyPatch(typeof(BunburrowExtension), nameof(BunburrowExtension.GetMapIndex))]
  internal class GetMapIndexPatch
  {
    private static void Postfix(ref Vector2Int __result)
    {
      if (MapContext.Instance != null)
      {
        __result = MapContext.Instance.CurrentCoordinates;
      }
    }
  }

  [HarmonyPatch(typeof(BunburrowExtension), nameof(BunburrowExtension.TryGetBunburrowFromMapIndex))]
  internal class TryGetBunburrowFromMapIndexPatch
  {
    private static void Postfix(ref bool __result, Vector2Int mapIndex, ref Bunburrows.Bunburrow bunburrow)
    {
      if (MapContext.Instance != null)
      {
        var foundBunburrow = MapContext.Instance.Coordinates.FirstOrDefault(c => c.MapIndex == mapIndex);
        if (foundBunburrow != null)
        {
          bunburrow = foundBunburrow.Bunburrow;
          __result = true;
        }
        else
        {
          __result = false;
        }
      }
    }
  }
}
