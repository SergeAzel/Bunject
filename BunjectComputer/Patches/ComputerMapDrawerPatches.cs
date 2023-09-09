using Bunburrows;
using Bunject.Internal;
using Computer;
using HarmonyLib;
using Levels;
using Misc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Bunject.Computer.Patches.ComputerMapDrawerPatches
{
	[HarmonyPatch(typeof(ComputerMapDrawer), "HandleOpen")]
	internal class HandleOpenPatch
	{
		public static Bunburrow FocussedBurrow { get; set; }
		public static ComputerMapLevelCellsController FocussedController { get; set; }
		public static void Prefix(ComputerMapDrawer __instance)
		{
			var identity = GameManager.LevelStates.CurrentLevelState.LevelIdentity;
			FocussedBurrow = identity.Depth != 0
				? identity.Bunburrow
				: Bunburrow.Hay;
			FocussedController = Traverse.Create(__instance).Field<BunburrowsCompleteListOf<ComputerMapLevelCellsController>>("levelCellsControllers").Value[Bunburrow.Hay];
		}
	}
	[HarmonyPatch(typeof(ComputerMapDrawer), "CenterOnStart")]
	internal class CenterOnStartPatch
	{
		private static void Postfix(ComputerMapDrawer __instance)
		{
			if (HandleOpenPatch.FocussedBurrow.IsCustomBunburrow())
			{
				var @this = Traverse.Create(__instance);
				@this.Field<RectTransform>("cellsHolderTransform").Value.anchoredPosition
					= @this.Field<BunburrowsCompleteListOf<ComputerMapLevelCellsController>>("levelCellsControllers").Value[Bunburrow.Hay].CenterPosition;
			}
		}
	}
	[HarmonyPatch(typeof(ComputerMapDrawer), "UpdateSelector")]
	internal class UpdateSelectorPatch
	{
		private static void Postfix(ComputerMapDrawer __instance)
		{
			if (HandleOpenPatch.FocussedBurrow.IsCustomBunburrow())
			{
				var @this = Traverse.Create(__instance); 
				LevelIdentity? identity = @this.Field<LevelIdentity?>("currentlyHoveredLevel").Value;
				if (identity.HasValue)
				{
					var current = new ComputerMapCellController[1];
					AccessTools.Method(typeof(ComputerMapDrawer), "TryGetCellOnCenter").Invoke(__instance, current);
					if (identity.Value.Bunburrow != HandleOpenPatch.FocussedBurrow || HandleOpenPatch.FocussedController != current[0].LevelCellsController)
					{
						//@this.Method("TryGetCellOnCenter", current).GetValue();
						if (identity.Value.Bunburrow != HandleOpenPatch.FocussedBurrow)
						{
							HandleOpenPatch.FocussedBurrow = identity.Value.Bunburrow;
							DrawMapAtDepthPatch.Postfix(__instance, identity.Value.Depth);
							//@this.Method("DrawMapAtDepth", identity.Value.Depth).GetValue();
						}
						var this_cellHolderTransform = @this.Field<RectTransform>("cellsHolderTransform").Value;
						var this_levelCellsControllers = @this.Field<BunburrowsCompleteListOf<ComputerMapLevelCellsController>>("levelCellsControllers").Value;
						this_cellHolderTransform.anchoredPosition -= -current[0].LevelCellsController.CenterPosition + this_levelCellsControllers[Bunburrow.Hay].CenterPosition;
					}
				}
			}
		}
	}
	[HarmonyPatch(typeof(ComputerMapDrawer), "DrawMapAtDepth")]
	internal class DrawMapAtDepthPatch
	{
		private static LevelsList GetBurrow(LevelsList burrow1, Direction dirFrom1, LevelsList burrow2, Direction dirFrom2)
			=> burrow1 != null && burrow2 != null
			? burrow1.AdjacentBunburrows[dirFrom1] == burrow2.AdjacentBunburrows[dirFrom2] ? burrow1.AdjacentBunburrows[dirFrom1] : null
			: burrow1?.AdjacentBunburrows[dirFrom1] ?? burrow2?.AdjacentBunburrows[dirFrom2];
		private static Dictionary<Bunburrow, LevelsList> GenerateMap(Bunburrow burrow)
		{
			var res = new Dictionary<Bunburrow, LevelsList>();
			var center	= res[Bunburrow.Hay]			= AssetsManager.LevelsLists[burrow.ToBunburrowName()];
			var left		= res[Bunburrow.Pink]			= center.AdjacentBunburrows[Direction.Left];
			var up			= res[Bunburrow.Aquatic]	= center.AdjacentBunburrows[Direction.Up];
			var down		= res[Bunburrow.Ghostly]	= center.AdjacentBunburrows[Direction.Down];
			var right		= res[Bunburrow.Purple]		= center.AdjacentBunburrows[Direction.Right];
			res[Bunburrow.NWVoid]									= GetBurrow(left, Direction.Up, up, Direction.Left);
			res[Bunburrow.NEVoid]									= GetBurrow(up, Direction.Right, right, Direction.Up);
			res[Bunburrow.SEVoid]									= GetBurrow(right, Direction.Down, down, Direction.Right);
			res[Bunburrow.SWVoid]									= GetBurrow(down, Direction.Left, left, Direction.Down);
			return res;
		}
		public static void Postfix(ComputerMapDrawer __instance, int depth)
		{
			if (!HandleOpenPatch.FocussedBurrow.IsCustomBunburrow())
				return;
			var map = GenerateMap(HandleOpenPatch.FocussedBurrow);
			var this_levelCellsControllers = Traverse.Create(__instance).Field<BunburrowsCompleteListOf<ComputerMapLevelCellsController>>("levelCellsControllers").Value;
			foreach (Bunburrow burrow in BunburrowExtension.GetBunburrowsInMapOrderList())
			{
				if (map[burrow] != null 
					&& new LevelIdentity(map[burrow].name.ToBunburrow(), depth) is LevelIdentity levelIdentity
					&& GameManager.GeneralProgression.LevelsVisited.ContainsEquatable(levelIdentity))
				{
					this_levelCellsControllers[burrow].DrawMap(map[burrow][depth], levelIdentity);
				}
				else
				{
					this_levelCellsControllers[burrow].ClearMap();
				}
			}
		}
	}
}
