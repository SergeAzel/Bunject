using Bunburrows;
using Bunject.Internal;
using Bunject.Levels;
using Bunject.NewYardSystem.Levels;
using Characters.Bunny.Data;
using Computer;
using HarmonyLib;
using Levels;
using Misc;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Tiling.Behaviour;
using TMPro;
using UnityEngine;

namespace Bunject.Computer.Internal
{
	internal class ModComputerMapDrawer
	{
		private static ModComputerMapDrawer _instance;
		public static ModComputerMapDrawer Instance
		{
			get
			{
				if (_instance?.This == null)
				{
					_instance = new ModComputerMapDrawer();
					Debug.Log("Bunject.Computer: Init Map Drawer");
				}
				return _instance;
			}
		}
			
		private ModComputerMapDrawer()
		{
			var t = Traverse.Create(GameManager.UIController.OphelineComputerCanvasController).Field("computerMapDrawer");
			This = t.GetValue<ComputerMapDrawer>();
			LevelCellsControllers = t.Field<BunburrowsCompleteListOf<ComputerMapLevelCellsController>>("levelCellsControllers").Value;
			CellsHolderTransform = t.Field<RectTransform>("cellsHolderTransform").Value;
			DepthInputHelpGameObject = t.Field<GameObject>("depthInputHelpGameObject").Value;
			DepthTextComponent = t.Field<TextMeshProUGUI>("depthTextComponent").Value;
			var type = typeof(ComputerMapDrawer);
			tryGetCellOnCenter = AccessTools.Method(type, "TryGetCellOnCenter");
			currentlyHoveredLevel = AccessTools.Field(type, "currentlyHoveredLevel");
			currentlyHoveredBunny = AccessTools.Field(type, "currentlyHoveredBunny");
			currentDepth = AccessTools.Field(type, "currentDepth");
		}
		public ComputerMapDrawer This { get; private set; }
		public BunburrowsCompleteListOf<ComputerMapLevelCellsController> LevelCellsControllers { get; private set; }
		public RectTransform CellsHolderTransform { get; private set; }
		public GameObject DepthInputHelpGameObject { get; private set; }
		public TextMeshProUGUI DepthTextComponent { get; private set; }

		private readonly FieldInfo currentlyHoveredLevel;
		public LevelIdentity? CurrentlyHoveredLevel => (LevelIdentity?)currentlyHoveredLevel.GetValue(This);
		private readonly FieldInfo currentlyHoveredBunny;
		public BunnyIdentity? CurrentlyHoveredBunny => (BunnyIdentity?)currentlyHoveredBunny.GetValue(This);

		private readonly FieldInfo currentDepth;
		public int CurrentDepth { get => (int)currentDepth.GetValue(This); private set => currentDepth.SetValue(This, value); }
		public Level OpeningLevel { get; private set; }
		public Bunburrow FocussedBurrow { get; private set; }
		public ComputerMapLevelCellsController FocussedController { get; private set; }
		public void HandleOpen()
		{
			OpeningLevel = GameManager.CurrentLevel;
			var identity = GameManager.LevelStates.CurrentLevelState.LevelIdentity;
			FocussedBurrow = identity.Depth != 0 ? identity.Bunburrow : Bunburrow.Hay;
			FocussedController = LevelCellsControllers[Bunburrow.Hay];
		}
		public bool HandleDownFloorInput()
		{
			bool runOriginal = true;
			if (TryGetCellOnCenter(out var center))
			{
				var coords = center.Coordinates;
				var levelObject = center.LevelCellsController.LevelObject;
				if (levelObject != null)
				{
					var level = LevelBuilder.BuildNewLevel(levelObject, null, false);
					var currTile = level.Tiles[coords.Item1 * 15 + coords.Item2];
					if (currTile is BunburrowEntryTile t && t.Bunburrow is Bunburrow b)
					{
						CurrentDepth = 0;
						FocussedBurrow = b;
						runOriginal = true;
					}
					else
					{
						runOriginal = !levelObject.IsSurface;
					}
				}
			}
			return runOriginal;
		}
		public void CenterOnStart()
		{
			if (FocussedBurrow.IsCustomBunburrow())
				CellsHolderTransform.anchoredPosition = LevelCellsControllers[Bunburrow.Hay].CenterPosition;
		}
		public void UpdateSelector()
		{
			if (FocussedBurrow.IsCustomBunburrow())
			{
				Bunburrow burrow;
				int depth;
				if (CurrentlyHoveredLevel is LevelIdentity level)
				{
					burrow = level.Bunburrow;
					depth = level.Depth;
				}
				else if (CurrentlyHoveredBunny is BunnyIdentity bunny)
				{
					burrow = bunny.Bunburrow;
					depth = bunny.InitialDepth;
				}
				else
					return;
				TryGetCellOnCenter(out var current);
				if (burrow != FocussedBurrow || FocussedController != current.LevelCellsController)
				{
					if (burrow != FocussedBurrow)
					{
						FocussedBurrow = burrow;
						DrawMapAtDepth(depth);
					}
					CellsHolderTransform.anchoredPosition -= -current.LevelCellsController.CenterPosition + LevelCellsControllers[Bunburrow.Hay].CenterPosition;
				}
			}
		}

		private readonly MethodInfo tryGetCellOnCenter;
		public bool TryGetCellOnCenter(out ComputerMapCellController cell)
		{
			var current = new ComputerMapCellController[1];
			var found = (bool)tryGetCellOnCenter.Invoke(This, current);
			cell = current[0];
			return found;
		}
		public bool DrawMapAtDepth(int depth)
		{
			bool runOriginal = true;
			if (FocussedBurrow.IsCustomBunburrow())
			{
				var map = GenerateMap(FocussedBurrow);
				DepthTextComponent.text = string.Format("{0} <color=#{1}>{2}</color>", 
					AssetsManager.UILocalizationObject.UIStrings.TryGetValue("computer_map_depth", out var text) ? text : "Depth", 
					ColorUtility.ToHtmlStringRGB(AssetsManager.BunburrowsListOfStyles.Computer.ButtonHoverColor), 
					depth);
				DepthTextComponent.UpdateFontData();
				foreach (Bunburrow burrow in BunburrowExtension.GetBunburrowsInMapOrderList())
				{
					if (map[burrow] != null
						&& new LevelIdentity(map[burrow].name.ToBunburrow(), depth) is LevelIdentity levelIdentity
						&& GameManager.GeneralProgression.LevelsVisited.ContainsEquatable(levelIdentity))
					{
						LevelCellsControllers[burrow].DrawMap(map[burrow][depth], levelIdentity);
					}
					else
					{
						LevelCellsControllers[burrow].ClearMap();
					}
				}
				runOriginal = false;
			} 
			else if (OpeningLevel.BaseData is ModLevelObject
				&& OpeningLevel.BaseData.name.Contains("Surface")
				&& OpeningLevel.BaseData.IsSurface 
				&& OpeningLevel.BunburrowEntryTiles.Count > 0)
			{
				CurrentDepth = 0;
				DepthTextComponent.text = string.Format("{0} <color=#{1}>{2}</color>",
					AssetsManager.UILocalizationObject.UIStrings.TryGetValue("computer_map_depth", out var text) ? text : "Depth",
					ColorUtility.ToHtmlStringRGB(AssetsManager.BunburrowsListOfStyles.Computer.ButtonHoverColor),
					0);
				DepthTextComponent.UpdateFontData();
				foreach (Bunburrow burrow in BunburrowExtension.GetBunburrowsInMapOrderList())
				{
					if (burrow == Bunburrow.Hay)
					{
						LevelCellsControllers[burrow].DrawMap(OpeningLevel.BaseData, default);
					}
					else
					{
						LevelCellsControllers[burrow].ClearMap();
					}
				}
				OpeningLevel = null;
				runOriginal = false;
			}
			return runOriginal;
		}
		private Dictionary<Bunburrow, LevelsList> GenerateMap(Bunburrow burrow)
		{
			var res = new Dictionary<Bunburrow, LevelsList>();
			var center = res[Bunburrow.Hay] = AssetsManager.LevelsLists[burrow.ToBunburrowName()];
			var left = res[Bunburrow.Pink] = center.AdjacentBunburrows[Direction.Left];
			var up = res[Bunburrow.Aquatic] = center.AdjacentBunburrows[Direction.Up];
			var down = res[Bunburrow.Ghostly] = center.AdjacentBunburrows[Direction.Down];
			var right = res[Bunburrow.Purple] = center.AdjacentBunburrows[Direction.Right];
			res[Bunburrow.NWVoid] = GetBurrow(left, Direction.Up, up, Direction.Left);
			res[Bunburrow.NEVoid] = GetBurrow(up, Direction.Right, right, Direction.Up);
			res[Bunburrow.SEVoid] = GetBurrow(right, Direction.Down, down, Direction.Right);
			res[Bunburrow.SWVoid] = GetBurrow(down, Direction.Left, left, Direction.Down);
			return res;
		}
		private LevelsList GetBurrow(LevelsList burrow1, Direction dirFrom1, LevelsList burrow2, Direction dirFrom2)
			=> burrow1 != null && burrow2 != null
			? burrow1.AdjacentBunburrows[dirFrom1] == burrow2.AdjacentBunburrows[dirFrom2] ? burrow1.AdjacentBunburrows[dirFrom1] : null
			: burrow1?.AdjacentBunburrows[dirFrom1] ?? burrow2?.AdjacentBunburrows[dirFrom2];
	}
}
