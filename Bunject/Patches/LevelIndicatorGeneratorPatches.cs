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
    private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
    {
      foreach (var instruction in instructions)
      {
        if (instruction.opcode == OpCodes.Ldstr && (string)instruction.operand == "")
        {
          yield return CodeInstruction.Call(typeof(GameManager), "get_CurrentLevel");
          yield return CodeInstruction.LoadField(typeof(Level), nameof(Level.BaseData));
          yield return CodeInstruction.Call(typeof(LevelObject), "get_CustomNameKey");
        }
        else
        {
          yield return instruction;
        }
      }
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

  [HarmonyPatch(typeof(LevelIndicatorGenerator), nameof(LevelIndicatorGenerator.GetShortLevelIndicatorWithStyle))]
  internal class GetShortLevelIndicatorWithStylePatch
  {
    private static string Postfix(string __result, LevelIdentity levelIdentity)
    {
      if (levelIdentity.Bunburrow.IsCustomBunburrow())
      {
        string text = ColorUtility.ToHtmlStringRGB(BunburrowManager.ResolveStyle(BunburrowManager.Bunburrows.First(x => x.ID == (int)levelIdentity.Bunburrow).Style).SkyboxColor);
        string text2 = ColorUtility.ToHtmlStringRGB(LevelIndicatorGenerator.GetLevelBunburrowStyle(levelIdentity).SkyboxColor);
        return string.Format("<color=#{0}>{1}-</color><color=#{2}>{3}</color>", new object[]
        {
        text,
        levelIdentity.Bunburrow.ToIndicator(),
        text2,
        levelIdentity.Depth
        });
      }
      return __result;
    }
  }
}
