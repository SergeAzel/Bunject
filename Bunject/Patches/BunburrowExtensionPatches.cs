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
  [HarmonyPatch(typeof(BunburrowExtension), "ToBunburrow", new Type[] { typeof(string) })]
  internal class ToBunburrowStringPatch
  {
    private static Bunburrow Postfix(Bunburrow __result, string bunburrowName)
    {
      return ((Bunburrow?)BunburrowManager.Bunburrows.FirstOrDefault(bb => bb.Name == bunburrowName)?.ID) ?? __result;
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
      if (bunburrow.IsCustomBunburrow())
			{
        return BunburrowManager.Bunburrows.First(bb => bb.ID == (int)bunburrow).Name;
      }
      return __result;
		}
  }

  [HarmonyPatch(typeof(BunburrowExtension), "ToComparisonIndex")]
  internal class ToComparisonIndexPatch
  {
    private static int Postfix(int __result, Bunburrow bunburrow)
    {
      if (bunburrow.IsCustomBunburrow())
      {
        return BunburrowManager.Bunburrows.First(bb => bb.ID == (int)bunburrow).ComparisonIndex;
      }
      return __result;
    }
  }

  [HarmonyPatch(typeof(BunburrowExtension), "ToIndicator")]
  internal class ToIndicatorPatch
  {
    private static string Postfix(string __result, Bunburrow bunburrow)
    {
      if (bunburrow.IsCustomBunburrow())
      {
        return BunburrowManager.Bunburrows.First(bb => bb.ID == (int)bunburrow).Indicator;
      }
      return __result;
    }
  }


  [HarmonyPatch(typeof(BunburrowExtension), "IsNonVoidBunburrow")]
  internal class IsNonVoidBunburrowPatch
  {
    private static bool Postfix(bool __result, Bunburrow bunburrow)
    {
      if (bunburrow.IsCustomBunburrow())
      {
        return !bunburrow.IsVoidBunburrow();
      }
      return __result;
    }
  }

  [HarmonyPatch(typeof(BunburrowExtension), "IsVoidBunburrow")]
  internal class IsVoidBunburrowPatch
  {
    private static bool Postfix(bool __result, Bunburrow bunburrow)
    {
      // redirect to the other function, which needs no extending
      if (bunburrow.IsCustomBunburrow())
        return BunburrowManager.Bunburrows.First(bb => bb.ID == (int)bunburrow).IsVoid;
      return __result;
    }
  }
}
