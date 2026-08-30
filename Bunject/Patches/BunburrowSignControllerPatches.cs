using Bunburrows;
using Bunject.Internal;
using Bunject.Levels;
using HarmonyLib;
using UnityEngine;

namespace Bunject.Patches.BunburrowSignControllerPatches
{
  [HarmonyPatch(typeof(BunburrowSignController), nameof(BunburrowSignController.UpdateContent))]
  internal class UpdateContentPatch
  {
    private static void Postfix(BunburrowSignController __instance)
    {
      var bunburrow = __instance.Bunburrow;
      if (!bunburrow.IsCustomBunburrow() || bunburrow.IsUnlocked())
        return;

      var requirements = bunburrow.GetModBunburrow()?.Requirements ?? new BunburrowRequirements();
      var progression = GameManager.GeneralProgression;

      int displayed;
      if (progression.HistoryCapturedBunnies.Count < requirements.Bunnies)
        displayed = requirements.Bunnies;
      else if (progression.ExistingCouples.Count < requirements.Babies)
        displayed = requirements.Babies;
      else if (progression.HomeCapturedBunnies.Count < requirements.HomeCaptures)
        displayed = requirements.HomeCaptures;
      else
        return;

      displayed = Mathf.Clamp(displayed, 0, 99);

      var traverse = Traverse.Create(__instance);
      traverse.Field("progressPercentageParentObject").GetValue<GameObject>().SetActive(false);
      traverse.Field("completeIcon").GetValue<GameObject>().SetActive(false);
      traverse.Field("homeIcon").GetValue<GameObject>().SetActive(false);
      traverse.Field("extraSparkles").GetValue<GameObject>().SetActive(false);
      traverse.Field("bunnyRequirementParentObject").GetValue<GameObject>().SetActive(true);
      traverse.Field("requirementFirstDigitSpriteRenderer").GetValue<SpriteRenderer>().sprite = AssetsManager.ItemCounterAssets[displayed / 10];
      traverse.Field("requirementSecondDigitSpriteRenderer").GetValue<SpriteRenderer>().sprite = AssetsManager.ItemCounterAssets[displayed % 10];
    }
  }
}
