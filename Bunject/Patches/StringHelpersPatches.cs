using System.Text.RegularExpressions;
using Bunburrows;
using Bunject.Internal;
using Bunject.Patches.PaqueretteActionResolverPatches;
using Misc;
using HarmonyLib;

namespace Bunject.Patches.StringHelpersPatches
{
  [HarmonyPatch(typeof(StringHelpers), "ReplaceVariables")]
  internal class ReplaceVariablesPatch
  {
    private static readonly Regex VariablePattern = new Regex("\\$[\\w_-]+");

    private static void Prefix(ref string text)
    {
      if (HandleTalkButtonPressPatches.targetBurrow == null)
        return;

      Bunburrow bunburrow = (Bunburrow)HandleTalkButtonPressPatches.targetBurrow;

      bool haveCount = false;
      BunniesCount count = default(BunniesCount);
      BunniesCount GetCount()
      {
        if (!haveCount)
        {
          count = GameManager.GeneralProgression.GetBunniesCountByBunburrow(bunburrow);
          haveCount = true;
        }
        return count;
      }

      text = VariablePattern.Replace(text, match =>
      {
        string token = match.Value.Substring(1);
        switch (token)
        {
          case "bunburrowProgress":
            return GetCount().RegularBunniesCount.ToString();
          case "bunburrowTotal":
            return GetCount().RegularBunniesTotal.ToString();
          case "bunburrowRequirement":
            return DescribeRequirement(bunburrow);
          case "bunburrowExtraProgress":
            return Traverse.Create(typeof(StringHelpers)).Method("CreateAdditionalProgressLine", bunburrow).GetValue<string>();
          default:
            return match.Value;
        }
      });
    }

    private static string DescribeRequirement(Bunburrow bunburrow)
    {
      if (!bunburrow.IsCustomBunburrow())
        return GameManager.GeneralProgression.BunburrowsUnlockRequirements[bunburrow].ToString();

      var mod = bunburrow.GetModBunburrow();
      var progression = GameManager.GeneralProgression;
      var bunniesRequired = mod?.RequiredBunnyCount ?? 0;
      var babiesRequired = mod?.RequiredBabyCount ?? 0;

      var insufficientBunnies = bunniesRequired > 0 && progression.HistoryCapturedBunnies.Count < bunniesRequired;
      var insufficientBabies = babiesRequired > 0 && progression.ExistingCouples.Count < babiesRequired;

      if (insufficientBabies)
      {
        if (insufficientBunnies)
        {
          return bunniesRequired + EndColor + " bunnies and " + BunburrowColor + babiesRequired + EndColor + " baby";
        }
        else
        {
          return babiesRequired + EndColor + " baby";
        }
      }

      return bunniesRequired.ToString();
    }

    private static string BunburrowColor = "<color=$bunburrowColor>";
    private static string EndColor = "</color>";
  }
}
