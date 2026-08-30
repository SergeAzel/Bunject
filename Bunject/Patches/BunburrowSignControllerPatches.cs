using Bunburrows;
using Bunject.Internal;
using HarmonyLib;
using UnityEngine;

namespace Bunject.Patches.BunburrowSignControllerPatches
{
  // Vanilla UpdateContent only draws the "N required" digits when
  // BunburrowsUnlockStatus[Bunburrow] is false, a state custom burrows can never reach
  // (the five-slot BunburrowsListOf falls back to Pink = always unlocked). This mirrors
  // that presentation for a locked custom burrow, showing the outstanding RequiredBunnyCount
  // and then, once that is met, the outstanding RequiredBabyCount. There is no icon to tell
  // the two apart - the sign dialogue carries that meaning.
  [HarmonyPatch(typeof(BunburrowSignController), nameof(BunburrowSignController.UpdateContent))]
  internal class UpdateContentPatch
  {
    private static void Postfix(BunburrowSignController __instance)
    {
      var bunburrow = __instance.Bunburrow;
      if (!bunburrow.IsCustomBunburrow() || bunburrow.IsUnlocked())
        return;

      var mod = bunburrow.GetModBunburrow();
      var progression = GameManager.GeneralProgression;
      var bunniesRequired = mod?.RequiredBunnyCount ?? 0;
      var babiesRequired = mod?.RequiredBabyCount ?? 0;

      int displayed;
      if (bunniesRequired > 0 && progression.HistoryCapturedBunnies.Count < bunniesRequired)
        displayed = bunniesRequired;
      else if (babiesRequired > 0 && progression.ExistingCouples.Count < babiesRequired)
        displayed = babiesRequired;
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
