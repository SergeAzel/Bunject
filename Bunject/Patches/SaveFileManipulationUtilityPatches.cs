using HarmonyLib;
using Saving;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bunject.Patches.SaveFileManipulationUtilityPatches
{
  [HarmonyPatch(typeof(SaveFileManipulationUtility), "GetRootSaveDataPath")]
  internal class GetRootSaveDataPathPatches
  {
    private static string Postfix(string __result)
    {
      if (!string.IsNullOrEmpty(BunjectAPI.SaveFolder))
        return (Path.Combine(__result, BunjectAPI.SaveFolder));
      return __result;
    }
  }
}
