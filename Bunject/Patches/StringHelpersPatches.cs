using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using Bunburrows;
using Bunject.Patches.PaqueretteActionResolverPatches;
using Misc;
using HarmonyLib;
using UnityEngine;

namespace Bunject.Patches.StringHelpersPatches
{

    [HarmonyPatch(typeof(StringHelpers), "ReplaceVariables")]
    internal class ReplaceVariablesPatch
    {
        static void Prefix(ref string text)
        {
            // Fix burrow sign dialogue to not be dependent on the burrow style
            if (HandleTalkButtonPressPatches.targetBurrow != null)
            {
                Bunburrow bunburrow = (Bunburrow)HandleTalkButtonPressPatches.targetBurrow;
                BunniesCount bunniesCount = GameManager.GeneralProgression.GetBunniesCountByBunburrow(bunburrow);

                string result = new Regex("\\$[\\w_-]+").Replace(text, delegate(Match match)
                {
                    string thingToReplace = match.Value.Substring(1, match.Value.Length - 1);
                    switch (thingToReplace)
                    {
                        case "bunburrowProgress":
					        return bunniesCount.RegularBunniesCount.ToString();
                        case "bunburrowExtraProgress":
                            return Traverse.Create(typeof(StringHelpers)).Method("CreateAdditionalProgressLine", bunburrow).GetValue<string>();
                        case "bunburrowTotal":
                        	return bunniesCount.RegularBunniesTotal.ToString();
                        default:
                            return "$" + thingToReplace;
                    }
                });
                text = result;
            }
        }
    }
}