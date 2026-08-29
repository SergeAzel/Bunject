using System.Text.RegularExpressions;
using Bunburrows;
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
            return GameManager.GeneralProgression.BunburrowsUnlockRequirements[bunburrow].ToString();
          case "bunburrowExtraProgress":
            return Traverse.Create(typeof(StringHelpers)).Method("CreateAdditionalProgressLine", bunburrow).GetValue<string>();
          default:
            return match.Value;
        }
      });
    }
  }
}
