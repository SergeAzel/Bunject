using Bunject.NewYardSystem.Model;
using HarmonyLib;
using Items;
using Misc;
using System;
using System.Collections.Generic;
using Tiling.Visuals;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace Bunject.NewYardSystem.Levels
{
	public class BNYSBunburrowTileSets : BunburrowTileSets
	{
		public static BNYSBunburrowTileSets Create(TileSetData tileSetData)
		{
			var @new = ScriptableObject.CreateInstance<BNYSBunburrowTileSets>();
			@new.FloorPropProbability = tileSetData.FloorPropProbability;
			@new.WallPropProbability = tileSetData.WallPropProbability;
			@new.IndestructibleWallPropProbability = tileSetData.UnbreakableWallPropProbability;
			return @new;
		}
		private Traverse traverse;
		private Traverse Traverse => traverse ?? (traverse = Traverse.Create(this));
		public new TileSetObject FloorTileSetObject
		{
			get => base.FloorTileSetObject;
			set => Traverse.Field("floorTileSetObject").SetValue(value);
		}
		public new TileSetObject WallTileSetObject
		{
			get => base.WallTileSetObject;
			set => Traverse.Field("wallTileSetObject").SetValue(value);
		}
		public new TileSetObject IndestructibleWallTileSetObject
		{
			get => base.IndestructibleWallTileSetObject;
			set => Traverse.Field("indestructibleWallTileSetObject").SetValue(value);
		}
		public new List<TileSetObject> FloorPropsTileSetObjects
		{
			get => base.FloorPropsTileSetObjects;
			set => Traverse.Field("floorPropsTileSetObjects").SetValue(value);
		}
		public new List<TileSetObject> WallPropsTileSetObjects
		{
			get => base.WallPropsTileSetObjects;
			set => Traverse.Field("wallPropsTileSetObjects").SetValue(value);
		}
		public new List<TileSetObject> IndestructibleWallPropsTileSetObjects
		{
			get => base.IndestructibleWallPropsTileSetObjects;
			set => Traverse.Field("indestructibleWallPropsTileSetObjects").SetValue(value);
		}
		public new TunnelTileSetObject TunnelTileSetObject
		{
			get => base.TunnelTileSetObject;
			set => Traverse.Field("tunnelTileSetObject").SetValue(value);
		}
		public new DirectionsListOf<Tile> TunnelEntriesWallOverlayTile
		{
			get => base.TunnelEntriesWallOverlayTile;
			set => Traverse.Field("tunnelEntriesWallOverlayTile").SetValue(value);
		}
		public new DirectionsListOf<Tile> IndestructibleWallCorners
		{
			get => base.IndestructibleWallCorners;
			set => Traverse.Field("indestructibleWallCorners").SetValue(value);
		}
		public new Tile UnbreakableFloorTile
		{
			get => base.UnbreakableFloorTile;
			set => Traverse.Field("unbreakableFloorTile").SetValue(value);
		}
		public new Tile ExitTile
		{
			get => base.ExitTile;
			set => Traverse.Field("exitTile").SetValue(value);
		}
		public new Tile ClosedExitTile
		{
			get => base.ClosedExitTile;
			set => Traverse.Field("closedExitTile").SetValue(value);
		}
		public new Tile TrapTile
		{
			get => base.TrapTile;
			set => Traverse.Field("trapTile").SetValue(value);
		}
		public new Tile CarrotTile
		{
			get => base.CarrotTile;
			set => Traverse.Field("carrotTile").SetValue(value);
		}
		public new Tile ElevatorOpenTile
		{
			get => base.ElevatorOpenTile;
			set => Traverse.Field("elevatorOpenTile").SetValue(value);
		}
		public new Tile ElevatorClosedTile
		{
			get => base.ElevatorClosedTile;
			set => Traverse.Field("elevatorClosedTile").SetValue(value);
		}
		public new Sprite ElevatorDown
		{
			get => base.ElevatorDown;
			set => Traverse.Field("elevatorDown").SetValue(value);
		}
		public new Sprite ElevatorUp
		{
			get => base.ElevatorUp;
			set => Traverse.Field("elevatorUp").SetValue(value);
		}
		public new Tile RopeTile
		{
			get => base.RopeTile;
			set => Traverse.Field("ropeTile").SetValue(value);
		}
		public new Tile PickaxeOverlayTile
		{
			get => base.PickaxeOverlayTile;
			set => Traverse.Field("pickaxeOverlayTile").SetValue(value);
		}
		public new Tile DarkPickaxeOverlayTile
		{
			get => base.DarkPickaxeOverlayTile;
			set => Traverse.Field("darkPickaxeOverlayTile").SetValue(value);
		}
		public new Tile UpTunnelAdjacentFloorOverlayTile
		{
			get => base.UpTunnelAdjacentFloorOverlayTile;
			set => Traverse.Field("upTunnelAdjacentFloorOverlayTile").SetValue(value);
		}
		public new Tile LeftTunnelAdjacentFloorOverlayTile
		{
			get => base.LeftTunnelAdjacentFloorOverlayTile;
			set => Traverse.Field("leftTunnelAdjacentFloorOverlayTile").SetValue(value);
		}
		public new ItemListOf<Sprite> UIItemsSprites
		{
			get => base.UIItemsSprites;
			set => Traverse.Field("uiItemsSprites").SetValue(value);
		}
		public new Sprite UIBunnySprite
		{
			get => base.UIBunnySprite;
			set => Traverse.Field("uiBunnySprite").SetValue(value);
		}
		public new Sprite UIBabyBunnySprite
		{
			get => base.UIBabyBunnySprite;
			set => Traverse.Field("uiBabyBunnySprite").SetValue(value);
		}
		public new Sprite UICounterBackgroundSprite
		{
			get => base.UICounterBackgroundSprite;
			set => Traverse.Field("uiCounterBackgroundSprite").SetValue(value);
		}
		public new IReadOnlyList<TileBase> SpecialTiles
		{
			get => base.SpecialTiles;
			set => Traverse.Field("specialTiles").SetValue(value);
		}
		public new Tile WallBreakingTile
		{
			get => base.WallBreakingTile;
			set => Traverse.Field("wallBreakingTile").SetValue(value);
		}
		public new Tile WallBrokenTile
		{
			get => base.WallBrokenTile;
			set => Traverse.Field("wallBrokenTile").SetValue(value);
		}
		public new Tile BurningTile
		{
			get => base.BurningTile;
			set => Traverse.Field("burningTile").SetValue(value);
		}
		public float FloorPropProbability
		{
			get => Traverse.Field<float>("floorPropProbability").Value;
			set => Traverse.Field("floorPropProbability").SetValue(value);
		}
		public float WallPropProbability
		{
			get => Traverse.Field<float>("wallPropProbability").Value;
			set => Traverse.Field("wallPropProbability").SetValue(value);
		}
		public float IndestructibleWallPropProbability
		{
			get => Traverse.Field<float>("indestructibleWallPropProbability").Value;
			set => Traverse.Field("indestructibleWallPropProbability").SetValue(value);
		}
	}
}
