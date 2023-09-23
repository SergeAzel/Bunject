using Bunject.Internal;
using Bunject.Utility;
using HarmonyLib;
using Levels;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reflection;
using System.Text;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace Bunject.Patches.AssetsManagerPatches
{
  [HarmonyPatch(typeof(AssetsManager), "LevelsLists", MethodType.Setter)]
  class LevelsListsPatch
  {
    //Replace the levels lists dictionary with one that can be intercepted whenever a levelslist is indexed
    static void Prefix(ref ReadOnlyDictionary<string, LevelsList> value)
    {
      value = new ReadOnlyDictionary<string, LevelsList>(new InjectionDictionary<string, LevelsList>(AssetsManagerRewiring.LoadLevelsList, value));
    }
  }
}
