using Bunburrows;
using Bunject.Internal;
using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Bunject.Patches.BunburrowSignControllerPatches
{ 
  [HarmonyPatch(typeof(BunburrowSignController), nameof(BunburrowSignController.Init))]
  internal class InitPatch
  {
    public static void Postfix(BunburrowSignController __instance)
    {
      var b = __instance.Bunburrow;
      if (!b.IsCustomBunburrow() || __instance.Bunburrow.IsVoidBunburrow() || AssetsManager.LevelsLists[b.ToBunburrowName()].Length <= 0) return;
      BunburrowStyle bunburrowStyle = AssetsManager.LevelsLists[b.ToBunburrowName()][1].BunburrowStyle;
      var @this = Traverse.Create(__instance);
      @this.Field<SpriteRenderer>("progressFirstDigitSpriteRenderer").Value.color = bunburrowStyle.SkyboxColor;
      @this.Field<SpriteRenderer>("progressSecondDigitSpriteRenderer").Value.color = bunburrowStyle.SkyboxColor;
      @this.Field<SpriteRenderer>("progressPercentSpriteRenderer").Value.color = bunburrowStyle.SkyboxColor;
      @this.Field<SpriteRenderer>("requirementFirstDigitSpriteRenderer").Value.color = bunburrowStyle.SkyboxColor;
      @this.Field<SpriteRenderer>("requirementSecondDigitSpriteRenderer").Value.color = bunburrowStyle.SkyboxColor;
      @this.Field<SpriteRenderer>("completeIconSpriteRenderer").Value.color = bunburrowStyle.SignCompleteIconColor;
      @this.Field<SpriteRenderer>("homeIconSpriteRenderer").Value.color = bunburrowStyle.SignHomeIconColor;
      @this.Field<SpriteRenderer>("sign").Value.material.SetInt(@this.Field<int>("ShouldGlitchHash").Value, 0);
      __instance.UpdateContent();
    }
  }
}
