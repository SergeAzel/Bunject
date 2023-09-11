﻿using Bunject.Internal;
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

    // If the depths is outside the bounds of the internal list, there will be an exception.  Catch and ignore/redirect it.
    static Exception Finalizer(Exception __exception, ref LevelObject __result, LevelsList __instance, int depth)
    {
      __result = OnLoadLevel.LoadLevel(null, __instance, depth);
      return null;
    }
  }
}
