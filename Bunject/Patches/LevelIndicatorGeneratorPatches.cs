using Bunburrows;
using Bunject.Internal;
using HarmonyLib;
using Levels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Bunject.Patches.LevelIndicatorGeneratorPatches
{
  [HarmonyPatch(typeof(LevelIndicatorGenerator), nameof(LevelIndicatorGenerator.GetLongLevelIndicator))]
  internal class GetLongLevelIndicatorPatches
  {
    private static string Postfix(string __result, bool useWhite)
		{
      var identity = GameManager.LevelStates.CurrentLevelState.LevelIdentity;
      if (identity.Bunburrow.IsCustomBunburrow())
			{
        var res = LevelIndicatorGenerator.GetShortLevelIndicator()
          + Traverse.Create(typeof(LevelIndicatorGenerator)).Method("GenerateBunniesStringForLevelIndicator", useWhite).GetValue<string>()
          + " ";
        var name = GameManager.CurrentLevel.BaseData.CustomNameKey;
				return res + (identity.Bunburrow.IsVoidBunburrow() && string.IsNullOrWhiteSpace(name)
					? LevelIndicatorGenerator.GenerateVoidLevelName()
					: name);
			}
      return __result;
    }
  }
  [HarmonyPatch(typeof(LevelIndicatorGenerator), nameof(LevelIndicatorGenerator.GetLevelBunburrowStyle))]
  internal class GetLevelBunburrowStylePatch
  {
    public static BunburrowStyle Postfix(BunburrowStyle __result, LevelIdentity levelIdentity)
    {
      if (levelIdentity.Bunburrow.IsCustomBunburrow())
      {
        return AssetsManager.LevelsLists[levelIdentity.Bunburrow.ToBunburrowName()][levelIdentity.Depth].BunburrowStyle;
      }
      return __result;
    }
  }
}
