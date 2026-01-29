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

namespace Bunject.Patches.LevelIndicatorGeneratorPatches
{
  [HarmonyPatch(typeof(LevelIndicatorGenerator), nameof(LevelIndicatorGenerator.GetLongLevelIndicator))]
  internal class GetLongLevelIndicatorPatches
  {
    private static string Postfix(string __result, bool useWhite)
    {
      var result = __result;
      var identity = GameManager.LevelStates.CurrentLevelState.LevelIdentity;
      if (identity.Bunburrow.IsCustomBunburrow())
      {
        var shortIndicator = LevelIndicatorGenerator.GetShortLevelIndicator()
          + Traverse.Create(typeof(LevelIndicatorGenerator)).Method("GenerateBunniesStringForLevelIndicator", useWhite).GetValue<string>()
          + " ";
        var name = GameManager.CurrentLevel.BaseData.CustomNameKey;
        result = shortIndicator + (identity.Bunburrow.IsVoidBunburrow() && string.IsNullOrWhiteSpace(name)
          ? LevelIndicatorGenerator.GenerateVoidLevelName()
          : name);
      }

      return BunjectAPI.Forward.OnLevelTitle(result, identity, useWhite);
    }
  }


  [HarmonyPatch(typeof(LevelIndicatorGenerator), nameof(LevelIndicatorGenerator.GetLevelBunburrowStyle))]
  internal class GetLevelBunburrowStylePatch
  {
    public static BunburrowStyle Postfix(BunburrowStyle __result, LevelIdentity levelIdentity)
    {
      var levelIdent = levelIdentity;
      if (levelIdent.Bunburrow.IsCustomBunburrow())
      {
        return AssetsManager.LevelsLists[levelIdent.Bunburrow.ToBunburrowName()][levelIdent.Depth].BunburrowStyle;
      }
      return __result;
    }
  }
}
