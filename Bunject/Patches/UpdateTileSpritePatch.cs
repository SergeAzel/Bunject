using Bunject.Tiling;
using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tiling.Behaviour;

namespace Bunject.Patches.TileLevelDataPatches
{
	[HarmonyPatch(typeof(TileLevelData), nameof(TileLevelData.UpdateTileSprite))]
	internal class UpdateTileSpritePatch
	{
		private static bool Prefix(TileLevelData __instance)
		{
			if (__instance is IUpdateTileSprite modTile)
			{
				modTile.UpdateTileSprite();
				return false;
			}
			return true;
		}
	}
}
