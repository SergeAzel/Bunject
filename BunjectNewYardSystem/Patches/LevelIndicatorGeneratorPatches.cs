using Bunburrows;
using Bunject.Internal;
using HarmonyLib;
using Levels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bunject.NewYardSystem.Addon.Patches.LevelIndicatorGeneratorPatches
{
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
}