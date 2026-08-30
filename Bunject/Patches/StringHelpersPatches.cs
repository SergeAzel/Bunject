using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using Bunburrows;
using Bunject.Internal;
using Bunject.Levels;
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

      var requirements = bunburrow.GetModBunburrow()?.Requirements ?? new BunburrowRequirements();
      var progression = GameManager.GeneralProgression;

      var clauses = new List<string>();
      if (progression.HistoryCapturedBunnies.Count < requirements.Bunnies)
        clauses.Add(requirements.Bunnies.ToString() + EndColor);
      if (progression.ExistingCouples.Count < requirements.Babies)
        clauses.Add(requirements.Babies + EndColor + " baby");
      if (progression.HomeCapturedBunnies.Count < requirements.HomeCaptures)
        clauses.Add(requirements.HomeCaptures + EndColor + " home-captured");
      if (clauses.Count == 0)
        clauses.Add(requirements.Bunnies.ToString());

      var builder = new StringBuilder(clauses[0]);
      for (int i = 1; i < clauses.Count; i++)
      {
        builder.Append(" bunnies");
        builder.Append(i == clauses.Count - 1 ? " and " : ", ");
        builder.Append(BunburrowColor);
        builder.Append(clauses[i]);
      }
      return builder.ToString();
    }

    private static string BunburrowColor = "<color=$bunburrowColor>";
    private static string EndColor = "</color>";
  }
}
