using Bunject.Internal;
using HarmonyLib;
using Levels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Bunject.Patches.LevelsListPatches
{
  [HarmonyPatch()]
  class IndexerPatch
  {
    static MethodBase TargetMethod()
    {
      return typeof(LevelsList).GetProperties().First(pi => pi.GetIndexParameters().Length == 1).GetGetMethod();
    }

    static LevelObject Postfix(LevelObject __result, LevelsList __instance, int depth)
    {
      //return LevelsListRewiring.LoadLevel(__instance, depth, __result);
      return OnLoadLevel.LoadLevel(__result, __instance, depth);
    }
  }
}
