using Bunject.Internal;
using Bunject.Levels;
using Characters.Bunny.Data;
using Computer;
using HarmonyLib;
using Levels;
using Misc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;

namespace Bunject.Computer.Patches.ComputerMapInfoControllerPatches
{
  [HarmonyPatch(typeof(ComputerMapInfoController))]
  internal class DisplayLevelInfoPatch
  {
    static bool _init = false;
    private static TextMeshProUGUI levelNameTextComponent;
    private static TextMeshProUGUI levelBunniesTextComponent;
    private static TextMeshProUGUI bunnyNameTextComponent;
    private static TextMeshProUGUI bunnyStatusTextComponent;

    private static void Init(ComputerMapInfoController instance)
    {
      if (_init)
        return;
      levelNameTextComponent = Traverse.Create(instance).Field<TextMeshProUGUI>("levelNameTextComponent").Value;
      levelBunniesTextComponent = Traverse.Create(instance).Field<TextMeshProUGUI>("levelBunniesTextComponent").Value;
      bunnyNameTextComponent = Traverse.Create(instance).Field<TextMeshProUGUI>("bunnyNameTextComponent").Value;
      bunnyStatusTextComponent = Traverse.Create(instance).Field<TextMeshProUGUI>("bunnyStatusTextComponent").Value;
      // TODO: rewrite this to be more sensible
      levelNameTextComponent.spriteAsset = levelBunniesTextComponent.spriteAsset;
      bunnyNameTextComponent.spriteAsset = bunnyStatusTextComponent.spriteAsset;
    }
    
    [HarmonyPostfix, HarmonyPatch(nameof(ComputerMapInfoController.DisplayLevelInfo))]
    private static void LevelPatch(ComputerMapInfoController __instance, LevelIdentity levelIdentity, LevelObject levelObject)
    {
      if (levelObject is ModLevelObject)
      {
        Init(__instance);
        if (levelIdentity.Depth == 0)
        {
          string text = ColorUtility.ToHtmlStringRGB(AssetsManager.BunburrowsListOfStyles[Bunburrows.Bunburrow.Pink].SkyboxColor);
          levelNameTextComponent.text = $"<color=#{text}>Surface</color>";
          levelNameTextComponent.UpdateFontData();
        }
        else
        {
          levelNameTextComponent.text += levelBunniesTextComponent.text;
          levelBunniesTextComponent.text = string.Empty;
          levelBunniesTextComponent.UpdateFontData(1, true);
        }
      }
    }
    [HarmonyPostfix, HarmonyPatch(nameof(ComputerMapInfoController.DisplayBunnyInfo))]
    private static void BunnyPatch(ComputerMapInfoController __instance, BunnyIdentity bunnyIdentity)
    {
      if (bunnyIdentity.Bunburrow.IsCustomBunburrow())
      {
        Init(__instance);
        bunnyNameTextComponent.text += bunnyStatusTextComponent.text;
        bunnyStatusTextComponent.text = string.Empty;
        bunnyStatusTextComponent.UpdateFontData(1, true);
      }
    }
  }
}
