using Bunburrows;
using Bunject.Internal;
using Characters.Bunny.Data;
using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Bunject.Patches.BunnyIdentityPatchs
{
	[HarmonyPatch(typeof(BunnyIdentity), nameof(BunnyIdentity.GetIdentityStringWithStyle))]
	internal class GetIdentityStringWithStylePatch
	{
		public static string Postfix(string __result,  BunnyIdentity __instance)
		{
			if (!__instance.Bunburrow.IsCustomBunburrow())
				return __result;
			int spriteSheetsID = BunburrowManager.ResolveStyle(BunburrowManager.Bunburrows.First(x => x.ID == (int)__instance.Bunburrow).Style).SpriteSheetsID;
			if (__instance.SpriteSheetID != spriteSheetsID)
			{
				return string.Format("<color=#{0}>{1}-</color><color=#{2}>{3}-{4}</color>", new object[]
				{
					ColorUtility.ToHtmlStringRGB(AssetsManager.BunburrowsListOfStyles.GetBunburrowStyleFromID(spriteSheetsID).SkyboxColor),
					__instance.Bunburrow.ToIndicator(),
					ColorUtility.ToHtmlStringRGB(AssetsManager.BunburrowsListOfStyles.GetBunburrowStyleFromID(__instance.SpriteSheetID).SkyboxColor),
					__instance.InitialDepth,
					__instance.LevelID
				});
			}
			return string.Format("<color=#{0}>{1}-{2}-{3}</color>", new object[]
			{
				ColorUtility.ToHtmlStringRGB(AssetsManager.BunburrowsListOfStyles.GetBunburrowStyleFromID(__instance.SpriteSheetID).SkyboxColor),
				__instance.Bunburrow.ToIndicator(),
				__instance.InitialDepth,
				__instance.LevelID
			});
		}
	}
}
