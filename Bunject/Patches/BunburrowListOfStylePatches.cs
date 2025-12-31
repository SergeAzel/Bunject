using Bunburrows;
using Bunject.Internal;
using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Bunject.Patches.BunburrowsListOfStylesPatches
{
  [HarmonyPatch]
  class IndexerPatch
  {
    private static MethodBase TargetMethod()
    {
      return typeof(BunburrowsListOfStyles).GetProperties().First(pi => pi.GetIndexParameters().Length == 1).GetGetMethod();
    }

    private static BunburrowStyle Postfix(BunburrowStyle __result, Bunburrow bunburrow)
    {
      if (bunburrow.IsCustomBunburrow())
        return BunburrowManager.Bunburrows.FirstOrDefault(bb => bb.ID == (int)bunburrow)?.ModBunburrow?.Style ?? __result;
      return __result;
    }
  }
}