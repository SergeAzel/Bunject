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
		private static readonly int[] ordering = new int[] { 0, 2, 4, 6, 1, 3, 5, 7 };
		private static Dictionary<Bunburrow, LevelsList> Map(Bunburrow burrow)
		{
			var curr = AssetsManager.LevelsLists[burrow.ToBunburrowName()];
			const int SIZE = 8;
			var l = new LevelsList[SIZE];
			/*
			1 2 3
			0 X 4
			7 6 5
			*/
			l[0] = curr.AdjacentBunburrows[Direction.Left];
			l[2] = curr.AdjacentBunburrows[Direction.Up];
			l[4] = curr.AdjacentBunburrows[Direction.Right];
			l[6] = curr.AdjacentBunburrows[Direction.Down];
			bool progress;
			do
			{
				progress = false;
				foreach (var index in ordering)
				{
					if (l[index] == null)
					{
						int search_index = (index - 1) % SIZE;
						int direction = -1;
						if (l[search_index = ((index - 1) % SIZE)] != null)
						{
							direction = (index + (2 - (index % 2))) % SIZE;
						}
						else if (l[search_index = ((index + 1) % SIZE)] != null)
						{
							direction = (index - (2 - (index % 2))) % SIZE;
						}
						else
							continue;
						LevelsList searched;
						switch (direction)
						{
							case 0:
								searched = l[search_index].AdjacentBunburrows[Direction.Left];
								break;
							case 2:
								searched = l[search_index].AdjacentBunburrows[Direction.Up];
								break;
							case 4:
								searched = l[search_index].AdjacentBunburrows[Direction.Right];
								break;
							case 6:
								searched = l[search_index].AdjacentBunburrows[Direction.Down];
								break;
							default:
								searched = null;
								break;
						}
						if (searched != null)
						{
							l[index] = searched;
							progress = true;
						}
					}
				}
			}
			while (progress);
			Dictionary<Bunburrow, LevelsList> dictionary = new Dictionary<Bunburrow, LevelsList>()
			{
				{ Bunburrow.Pink , l[0]},
				{ Bunburrow.Aquatic , l[2]},
				{ Bunburrow.Hay , curr},
				{ Bunburrow.Ghostly , l[6]},
				{ Bunburrow.Purple , l[4]},
				{ Bunburrow.NWVoid , l[1]},
				{ Bunburrow.NEVoid , l[3]},
				{ Bunburrow.SWVoid , l[7]},
				{ Bunburrow.SEVoid , l[5]},
			};
			return dictionary;
		}
		public static void Postfix(ComputerMapDrawer __instance, int depth)
		{
			if (!HandleOpenPatch.FocussedBurrow.IsCustomBunburrow())
				return;
			var map = Map(HandleOpenPatch.FocussedBurrow);
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
