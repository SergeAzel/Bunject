using Bunburrows;
using Bunject.Internal;
using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Bunject.Patches.BunburrowExtensionPatches
{
  [HarmonyPatch(typeof(BunburrowExtension))]
  internal class ToBunburrowStringPatch
  {
    public static MethodInfo TargetMethod()
    {
      return typeof(BunburrowExtension).GetMethod("ToBunburrow", new Type[] { typeof(string) });
    }

    public static bool Prefix(string bunburrowName, ref Bunburrows.Bunburrow __result)
    {
      if (BunburrowManager.IsInitialized)
      {
        __result = (Bunburrows.Bunburrow)(BunburrowManager.Bunburrows.FirstOrDefault(bb => bb.Name == bunburrowName)?.ID ?? (int)Bunburrows.Bunburrow.Pink);
        return false;
      }
      return true;
    }
  }

  [HarmonyPatch(typeof(BunburrowExtension))]
  // updates ToBunburrow, which indices burrows unusually for core burrows
  internal class ToBunburrowIntPatch
  {
    private static MethodInfo TargetMethod()
    {
      return typeof(BunburrowExtension).GetMethod("ToBunburrow", new Type[] { typeof(int) });
    }

    private static bool Prefix(int bunburrowID, ref Bunburrows.Bunburrow __result)
    {
      if (BunburrowManager.IsInitialized)
      {
        if (bunburrowID > BunburrowManager.CustomBunburrowThreshold)
        {
          __result = (Bunburrows.Bunburrow)bunburrowID;
          return false;
        }
      }
      return true;
    }
  }

  [HarmonyPatch(typeof(BunburrowExtension), "ToBunburrowName")]
  internal class ToBunburrowNamePatch
  {
    private static bool Prefix(Bunburrows.Bunburrow bunburrow, ref string __result)
    {
      if (BunburrowManager.IsInitialized)
      {
        __result = BunburrowManager.Bunburrows.FirstOrDefault(bb => bb.ID == (int)bunburrow)?.Name;
        return (__result == null);
      }
      return true;
    }
  }

  [HarmonyPatch(typeof(BunburrowExtension), "ToComparisonIndex")]
  internal class ToComparisonIndexPatch
  {
    private static bool Prefix(Bunburrows.Bunburrow bunburrow, ref int __result)
    {
      if (BunburrowManager.IsInitialized)
      {
        var result = BunburrowManager.Bunburrows.FirstOrDefault(bb => bb.ID == (int)bunburrow)?.ComparisonIndex;
        if (result.HasValue)
        {
          __result = result.Value;
          return false;
        }
      }
      return true;
    }
  }

  [HarmonyPatch(typeof(BunburrowExtension), "ToIndicator")]
  internal class ToIndicatorPatch
  {
    private static bool Prefix(Bunburrows.Bunburrow bunburrow, ref string __result)
    {
      if (BunburrowManager.IsInitialized)
      {
        __result = BunburrowManager.Bunburrows.FirstOrDefault(bb => bb.ID == (int)bunburrow)?.Indicator;
        return (__result == null);
      }
      return true;
    }
  }


  [HarmonyPatch(typeof(BunburrowExtension), "IsNonVoidBunburrow")]
  internal class IsNonVoidBunburrowPatch
  {
    private static bool Prefix(Bunburrows.Bunburrow bunburrow, ref bool __result)
    {
      // redirect to the other function, which needs no extending
      __result = !BunburrowExtension.IsVoidBunburrow(bunburrow);
      return false;
    }
  }

  [HarmonyPatch(typeof(BunburrowExtension), "IsVoidBunburrow")]
  internal class IsVoidBunburrowPatch
  {
    private static bool Postfix(bool __result, Bunburrows.Bunburrow bunburrow)
    {
      // redirect to the other function, which needs no extending
      if (bunburrow.IsCustomBunburrow() && BunburrowManager.IsInitialized)
        return BunburrowManager.Bunburrows.FirstOrDefault(bb => bb.ID == (int)bunburrow)?.IsVoid ?? __result;
      return __result;
    }
  }
}
