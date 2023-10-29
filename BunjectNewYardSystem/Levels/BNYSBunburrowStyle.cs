using Bunburrows;
using Bunject.NewYardSystem.Model;
using Bunject.NewYardSystem.Resources;
using HarmonyLib;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tiling.Visuals;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace Bunject.NewYardSystem.Levels
{
  public class BNYSBunburrowStyle : BunburrowStyle
  {
    internal static BNYSBunburrowStyle CopyFrom(BunburrowStyle @from, Bunburrow burrow)
    {
      var copy = ScriptableObject.CreateInstance<BNYSBunburrowStyle>();
      copy.name = $"{@from.name} ({burrow.ToBunburrowName()})";
      copy.Bunburrow = burrow;
      if (@from is BNYSBunburrowStyle getStyle)
        copy.StyleName = getStyle.StyleName;
      copy.SpriteSheetsID = @from.SpriteSheetsID;
      copy.BunnySpriteSheetsID = @from.BunnySpriteSheetsID;
      copy.ButtonDefaultColor = @from.ButtonDefaultColor;
      copy.ButtonHoverColor = @from.ButtonHoverColor;
      copy.ButtonSelectColor = @from.ButtonSelectColor;
      copy.ClockSprite = @from.ClockSprite;
      copy.CursorTexture = @from.CursorTexture;
      copy.DialogueNameSprite = @from.DialogueNameSprite;
      copy.DialogueSprite = @from.DialogueSprite;
      copy.FlowerSprite = @from.FlowerSprite;
      copy.GridColor = @from.GridColor;
      copy.InTextBoxColor = @from.InTextBoxColor;
      copy.IsVoidBunburrowStyle = @from.IsVoidBunburrowStyle;
      copy.Music = @from.Music;
      copy.SignCompleteIconColor = @from.SignCompleteIconColor;
      copy.SignHomeIconColor = @from.SignHomeIconColor;
      copy.SkyboxColor = @from.SkyboxColor;
      copy.SpriteSheetsID = @from.SpriteSheetsID;
      copy.TextSpritePrefix = @from.TextSpritePrefix;
      copy.TileSets = @from.TileSets;
      copy.UIColor = @from.UIColor;
      copy.UIWhiteColor = @from.UIWhiteColor;
      return copy;
    }
    public static BNYSBunburrowStyle Create(Bunburrow burrow, BurrowStyleMetadata style)
    {
      var @new = ScriptableObject.CreateInstance<BNYSBunburrowStyle>();
      try
      {
        @new.name = $"BunburrowStyle{style.StyleName} ({burrow.ToBunburrowName()})";
        @new.Bunburrow = burrow;
        @new.StyleName = style.StyleName;
        /*
         * these will be supported when we start supporting player and bnuuy reskins
        @new.TextSpritePrefix = style.TextSpritePrefix;
        @new.SpriteSheetsID = style.SpriteSheetsID;
        @new.BunnySpriteSheetsID = style.BunnySpriteSheetsID;
         * do you think I know what a FMOD reference is???
        @new.Music = style.Music;
        */
        Color color;
        if (ColorUtility.TryParseHtmlString(style.SkyboxColor, out color)) @new.SkyboxColor = color;
        if (ColorUtility.TryParseHtmlString(style.ButtonDefaultColor, out color)) @new.ButtonDefaultColor = color;
        if (ColorUtility.TryParseHtmlString(style.ButtonHoverColor, out color)) @new.ButtonHoverColor = color;
        if (ColorUtility.TryParseHtmlString(style.ButtonSelectColor, out color)) @new.ButtonSelectColor = color;
        if (ColorUtility.TryParseHtmlString(style.UIColor, out color)) @new.UIColor = color;
        if (ColorUtility.TryParseHtmlString(style.UIWhiteColor, out color)) @new.UIWhiteColor = color;
        if (ColorUtility.TryParseHtmlString(style.SignCompleteIconColor, out color)) @new.SignCompleteIconColor = color;
        if (ColorUtility.TryParseHtmlString(style.SignHomeIconColor, out color)) @new.SignHomeIconColor = color;
        if (ColorUtility.TryParseHtmlString(style.ParticleColor, out color)) @new.ParticleColor = color;
        if (ColorUtility.TryParseHtmlString(style.GridColor, out color)) @new.GridColor = color;
        if (ColorUtility.TryParseHtmlString(style.InTextBoxColor, out color)) @new.InTextBoxColor = color;
        var tileSetData = style.TileSets;
        BNYSBunburrowTileSets new_tileSets = BNYSBunburrowTileSets.Create(tileSetData);
        @new.TileSets = new_tileSets;
        @new.TileSets.name = $"{style.StyleName}TileSetsList";
        // assume all Paths have been changed via GetFullPath()
        HashSet<string> textures = new HashSet<string>
      {
        style.FlowerSprite.Path,
        style.DialogueSprite.Path,
        style.DialogueNameSprite.Path,
        style.ClockSprite.Path,
        style.Cursor.Path,
        tileSetData.Floors.Path,
        tileSetData.UnbreakableFloor.Path,
        tileSetData.BurningFloorOverlay.Path,
        tileSetData.Walls.Path,
        tileSetData.UnbreakableWalls.Path,
        tileSetData.Tunnels.Path,
        tileSetData.Rope.Path,
        tileSetData.Exit.Path,
        tileSetData.ClosedExit.Path,
        tileSetData.Carrot.Path,
        tileSetData.Trap.Path,
        tileSetData.ElevatorOpen.Path,
        tileSetData.ElevatorClosed.Path,
        tileSetData.ElevatorDown.Path,
        tileSetData.ElevatorUp.Path,
        tileSetData.UIItemCount.Path,
        tileSetData.DarkPickaxeOverlay.Path
      };
        textures.UnionWith(tileSetData.FloorProps.Select(x => x.Path));
        textures.UnionWith(tileSetData.WallProps.Select(x => x.Path));
        textures.UnionWith(tileSetData.WallBreak.Select(x => x.Path));
        textures.UnionWith(tileSetData.UnbreakableWallProps.Select(x => x.Path));
        textures.UnionWith(tileSetData.TunnelEntries.Select(x => x.Path));
        textures.UnionWith(tileSetData.UnbreakableWallCorners.Select(x => x.Path));
        textures.UnionWith(tileSetData.TunnelFloorOverlays.Select(x => x.Path));
        textures.UnionWith(tileSetData.SpecialTiles.Select(x => x.Path));
        textures.UnionWith(tileSetData.UIItems.Select(x => x.Path));
        textures.UnionWith(tileSetData.UIBunnies.Select(x => x.Path));
        var textureDict = textures.ToDictionary(path => path, path => ImportImage.ImportTexture(Path.GetFileNameWithoutExtension(path), path));
        const int size = 16;
        var tSize = new Vector2Int(size, size);
        var tPivot = new Vector2(.5f, .5f);
        @new.FlowerSprite = textureDict[style.FlowerSprite.Path]
          .ImportSprite(style.FlowerSprite.Position, new Vector2Int(8, 8), tPivot, size,
          name: $"{@new.StyleName}_selectIndicator");
        @new.ClockSprite = textureDict[style.ClockSprite.Path]
          .ImportSprite(style.ClockSprite.Position, new Vector2Int(7, 8), tPivot, size,
          name: $"{@new.StyleName}_clock");
        @new.DialogueSprite = Sprite.Create(textureDict[style.DialogueSprite.Path], new Rect(style.DialogueSprite.Position, new Vector2Int(32, 32)), tPivot,
          16, 0, SpriteMeshType.Tight, new Vector4(7, 7, 7, 7));
        @new.DialogueSprite.name = $"{@new.StyleName}_UIdialogueBox";
        @new.DialogueNameSprite = Sprite.Create(textureDict[style.DialogueNameSprite.Path], new Rect(style.DialogueNameSprite.Position, new Vector2Int(16, 16)), tPivot,
          16, 0, SpriteMeshType.Tight, new Vector4(3, 3, 3, 3));
        @new.DialogueNameSprite.name = $"{@new.StyleName}_UIdialogueBox";
        @new.DialogueNameSprite = textureDict[style.DialogueNameSprite.Path]
          .ImportSprite(style.DialogueNameSprite.Position, new Vector2Int(16, 16), tPivot, size,
          name: $"{@new.StyleName}_UIdialogueBoxName");
        @new.CursorTexture = new Texture2D(32, 32);
        @new.CursorTexture.SetPixels(textureDict[style.Cursor.Path].GetPixels(style.Cursor.X, style.Cursor.Y, 32, 32));
        @new.CursorTexture.name = $"{@new.StyleName}_Cursor";
        new_tileSets.FloorTileSetObject = CreateTileSetObject(textureDict[tileSetData.Floors.Path], tileSetData.Floors.Position,
          tSize, tPivot, size, tileSetData.Floors.Frames, new Vector2Int(4 * size, 0), tileSetData.Floors.Speed,
          $"{@new.StyleName}_tileMapLevel_floors");
        new_tileSets.FloorPropsTileSetObjects = tileSetData.FloorProps.Select(x => CreateTileSetObject(textureDict[x.Path], x.Position,
          tSize, tPivot, size, x.Frames, new Vector2Int(4 * size, 0), x.Speed,
          $"{@new.StyleName}_floor_props")).ToList();
        new_tileSets.WallTileSetObject = CreateTileSetObject(textureDict[tileSetData.Walls.Path], tileSetData.Walls.Position,
          tSize, tPivot, size, tileSetData.Walls.Frames, new Vector2Int(4 * size, 0), tileSetData.Walls.Speed,
          $"{@new.StyleName}_tileMapLevel_walls");
        new_tileSets.WallPropsTileSetObjects = tileSetData.WallProps.Select(x => CreateTileSetObject(textureDict[x.Path], x.Position,
          tSize, tPivot, size, x.Frames, new Vector2Int(4 * size, 0), x.Speed,
          $"{@new.StyleName}_floor_props")).ToList();
        new_tileSets.IndestructibleWallTileSetObject = CreateTileSetObject(textureDict[tileSetData.UnbreakableWalls.Path], tileSetData.UnbreakableWalls.Position,
          tSize, tPivot, size, tileSetData.UnbreakableWalls.Frames, new Vector2Int(4 * size, 0), tileSetData.UnbreakableWalls.Speed,
          $"{@new.StyleName}_incassables");
        new_tileSets.IndestructibleWallPropsTileSetObjects = tileSetData.UnbreakableWallProps.Select(x => CreateTileSetObject(textureDict[x.Path], x.Position,
          tSize, tPivot, size, x.Frames, new Vector2Int(4 * size, 0), x.Speed,
          $"{@new.StyleName}_floor_props")).ToList();
        new_tileSets.TunnelTileSetObject = CreateTunnelTileSetObject(textureDict[tileSetData.Tunnels.Path], tileSetData.Tunnels.Position,
          tSize, tPivot, size, tileSetData.Tunnels.Frames, new Vector2Int(4 * size, 0), tileSetData.Tunnels.Speed,
          $"{@new.StyleName}_tunnels");
        new_tileSets.BurningTile = textureDict[tileSetData.BurningFloorOverlay.Path]
          .ImportTile(tileSetData.BurningFloorOverlay.Position, tSize, tPivot, size,
          name: $"{@new.StyleName}_sol_cursed") as Tile;
        new_tileSets.UnbreakableFloorTile = textureDict[tileSetData.UnbreakableFloor.Path]
          .ImportTile(tileSetData.UnbreakableFloor.Position, tSize, tPivot, size,
          name: $"{@new.StyleName}_sol_incassable") as Tile;
        new_tileSets.CarrotTile = textureDict[tileSetData.Carrot.Path]
          .ImportTile(tileSetData.Carrot.Position, tSize, tPivot, size,
          name: $"{@new.StyleName}_carrotte") as Tile;
        new_tileSets.TrapTile = textureDict[tileSetData.Trap.Path]
          .ImportTile(tileSetData.Trap.Position, tSize, tPivot, size,
          name: $"{@new.StyleName}_cage") as Tile;
        new_tileSets.PickaxeOverlayTile = textureDict[tileSetData.UIItems[1].Path]
          .ImportTile(tileSetData.UIItems[1].Position, tSize, tPivot, size,
          name: $"{@new.StyleName}_pickaxe") as Tile;
        new_tileSets.DarkPickaxeOverlayTile = textureDict[tileSetData.DarkPickaxeOverlay.Path]
          .ImportTile(tileSetData.DarkPickaxeOverlay.Position, tSize, tPivot, size,
          name: $"{@new.StyleName}_pickaxe") as Tile;
        new_tileSets.ExitTile = textureDict[tileSetData.Exit.Path]
          .ImportTile(tileSetData.Exit.Position, tSize, tPivot, size,
          name: $"{@new.StyleName}_hole") as Tile;
        new_tileSets.ClosedExitTile = textureDict[tileSetData.ClosedExit.Path]
          .ImportTile(tileSetData.ClosedExit.Position, tSize, tPivot, size,
          name: $"{@new.StyleName}_holePlugged") as Tile;
        new_tileSets.RopeTile = textureDict[tileSetData.Rope.Path]
          .ImportTile(tileSetData.Rope.Position, tSize, tPivot, size,
          name: $"{@new.StyleName}_holeAscent") as Tile;
        new_tileSets.ElevatorOpenTile = textureDict[tileSetData.ElevatorOpen.Path]
          .ImportTile(tileSetData.ElevatorOpen.Position, tSize, tPivot, size,
          name: $"{@new.StyleName}_elevatorOpen") as Tile;
        new_tileSets.ElevatorClosedTile = textureDict[tileSetData.ElevatorClosed.Path]
          .ImportTile(tileSetData.ElevatorClosed.Position, tSize, tPivot, size,
          name: $"{@new.StyleName}_elevatorClose") as Tile;
        new_tileSets.ElevatorUp = textureDict[tileSetData.ElevatorUp.Path]
          .ImportSprite(tileSetData.ElevatorUp.Position, new Vector2Int(size * 3, size * 4), tPivot, size,
          $"{@new.StyleName}_backgroundElevatorUp");
        new_tileSets.ElevatorDown = textureDict[tileSetData.ElevatorDown.Path]
          .ImportSprite(tileSetData.ElevatorDown.Position, new Vector2Int(size * 3, size * 4), tPivot, size,
          $"{@new.StyleName}_backgroundElevatorDown");
        new_tileSets.SpecialTiles = tileSetData.SpecialTiles.Select((x, i) => textureDict[x.Path].ImportTile(x.Position,
          tSize, tPivot, size, x.Frames, new Vector2Int(size, 0), x.Speed,
          $"{@new.StyleName}_special_{i}")).ToList();
        new_tileSets.UpTunnelAdjacentFloorOverlayTile = textureDict[tileSetData.TunnelFloorOverlays[0].Path]
          .ImportTile(tileSetData.TunnelFloorOverlays[0].Position, tSize, tPivot, size,
          name: $"{@new.StyleName}_overlay_tunnel") as Tile;
        new_tileSets.LeftTunnelAdjacentFloorOverlayTile = textureDict[tileSetData.TunnelFloorOverlays[1].Path]
          .ImportTile(tileSetData.TunnelFloorOverlays[1].Position, tSize, tPivot, size,
          name: $"{@new.StyleName}_overlay_tunnel") as Tile;
        new_tileSets.WallBreakingTile = textureDict[tileSetData.WallBreak[0].Path]
          .ImportTile(tileSetData.WallBreak[0].Position, tSize, tPivot, size,
          name: $"{@new.StyleName}_debris") as Tile;
        new_tileSets.WallBrokenTile = textureDict[tileSetData.WallBreak[1].Path]
          .ImportTile(tileSetData.WallBreak[1].Position, tSize, tPivot, size,
          name: $"{@new.StyleName}_debris") as Tile;
        new_tileSets.UIBabyBunnySprite = textureDict[tileSetData.UIBunnies[0].Path]
          .ImportSprite(tileSetData.UIBunnies[0].Position, tSize, tPivot, size,
          name: $"{@new.StyleName}_UIbebeLapin");
        new_tileSets.UIBunnySprite = textureDict[tileSetData.UIBunnies[1].Path]
          .ImportSprite(tileSetData.UIBunnies[1].Position, tSize, tPivot, size,
          name: $"{@new.StyleName}_UIlapin");
        new_tileSets.UICounterBackgroundSprite = textureDict[tileSetData.UIItemCount.Path]
          .ImportSprite(tileSetData.UIItemCount.Position, new Vector2Int(7, 7), tPivot, size,
          name: $"{@new.StyleName}_UIitemCount");
        var entries = tileSetData.TunnelEntries.Select((x, i) => textureDict[x.Path].ImportTile(x.Position,
          tSize, tPivot, size, name: $"{@new.StyleName}_tunnel_entries") as Tile).ToArray();
        new_tileSets.TunnelEntriesWallOverlayTile = new Misc.DirectionsListOf<Tile>(entries[0], entries[1], entries[3], entries[2]);
        var corners = tileSetData.UnbreakableWallCorners.Select((x, i) => textureDict[x.Path].ImportTile(x.Position,
          tSize, tPivot, size, name: $"{@new.StyleName}_incassables_corners") as Tile).ToArray();
        new_tileSets.IndestructibleWallCorners = new Misc.DirectionsListOf<Tile>(corners[2], corners[0], corners[1], corners[3]);
        var items = tileSetData.UIItems.Select((x, i) => textureDict[x.Path].ImportSprite(x.Position,
          tSize, tPivot, size, $"{@new.StyleName}_UIItem")).ToArray();
        new_tileSets.UIItemsSprites = new Items.ItemListOf<Sprite>(items[0], items[1], items[3], items[2], items[4], items[1], items[3]);
        foreach (var value in textureDict.Values)
          value.Apply(true, true);
      }
      catch(Exception e)
      { 
        Debug.LogException(e);
      }
      return @new;
    }
    
    public static TileSetObject CreateTileSetObject(Texture2D texture, Vector2Int position, Vector2Int size, Vector2 pivot, int pixelsPerUnit, 
      int frames = 1, Vector2Int offset = default, (float min, float max) speed = default, string name = "")
    {
      var @new = ScriptableObject.CreateInstance<TileSetObject>();
      var t = Traverse.Create(@new);
      for (int x = 0; x < 4; x++)
      {
        for (int y = 0; y < 4; y++)
        {
          var tile = texture.ImportTile(position + new Vector2Int(size.x * x, size.y * y), size, pivot, pixelsPerUnit,
            frames, offset, speed, $"{name}_{y * 4 + x}");
          if ((x, y) == (3, 0)) t.Field("filled").SetValue(tile);

          if ((x, y) == (0, 0)) t.Field("leftDeadEnd").SetValue(tile);
          if ((x, y) == (2, 0)) t.Field("rightDeadEnd").SetValue(tile);
          if ((x, y) == (3, 3)) t.Field("upDeadEnd").SetValue(tile);
          if ((x, y) == (3, 1)) t.Field("downDeadEnd").SetValue(tile);

          if ((x, y) == (0, 3)) t.Field("upperLeftCorner").SetValue(tile);
          if ((x, y) == (2, 3)) t.Field("upperRightCorner").SetValue(tile);
          if ((x, y) == (2, 1)) t.Field("lowerRightCorner").SetValue(tile);
          if ((x, y) == (0, 1)) t.Field("lowerLeftCorner").SetValue(tile);

          if ((x, y) == (1, 0)) t.Field("horizontalTunnel").SetValue(tile);
          if ((x, y) == (3, 2)) t.Field("verticalTunnel").SetValue(tile);

          if ((x, y) == (0, 2)) t.Field("leftWall").SetValue(tile);
          if ((x, y) == (2, 2)) t.Field("rightWall").SetValue(tile);
          if ((x, y) == (1, 3)) t.Field("upWall").SetValue(tile);
          if ((x, y) == (1, 1)) t.Field("downWall").SetValue(tile);

          if ((x, y) == (1, 2)) t.Field("alone").SetValue(tile);
        }
      }
      @new.name = $"{name} TileSet";
      return @new;
    }
    public static TunnelTileSetObject CreateTunnelTileSetObject(Texture2D texture, Vector2Int position, Vector2Int size, Vector2 pivot, int pixelsPerUnit,
      int frames = 1, Vector2Int offset = default, (float min, float max) speed = default, string name = "")
    {
      var @new = ScriptableObject.CreateInstance<TunnelTileSetObject>();
      var t = Traverse.Create(@new);
      for (int x = 0; x < 4; x++)
      {
        for (int y = 0; y < 4; y++)
        {
          if ((x, y) == (3, 0)) continue;
          var tile = texture.ImportTile(position + new Vector2Int(size.x * x, size.y * y), size, pivot, pixelsPerUnit,
            frames, offset, speed, $"{name}_{y * 4 + x}");
          if ((x, y) == (1, 2)) t.Field("crossroad").SetValue(tile);

          if ((x, y) == (0, 0)) t.Field("rightEntry").SetValue(tile);
          if ((x, y) == (2, 0)) t.Field("leftEntry").SetValue(tile);
          if ((x, y) == (3, 3)) t.Field("downEntry").SetValue(tile);
          if ((x, y) == (3, 1)) t.Field("upEntry").SetValue(tile);

          if ((x, y) == (0, 3)) t.Field("rightDownEntries").SetValue(tile);
          if ((x, y) == (2, 3)) t.Field("leftDownEntries").SetValue(tile);
          if ((x, y) == (2, 1)) t.Field("leftUpEntries").SetValue(tile);
          if ((x, y) == (0, 1)) t.Field("rightUpEntries").SetValue(tile);

          if ((x, y) == (1, 0)) t.Field("horizontalCorridor").SetValue(tile);
          if ((x, y) == (3, 2)) t.Field("verticalCorridor").SetValue(tile);

          if ((x, y) == (0, 2)) t.Field("noLeftEntry").SetValue(tile);
          if ((x, y) == (2, 2)) t.Field("noRightEntry").SetValue(tile);
          if ((x, y) == (1, 3)) t.Field("noUpEntry").SetValue(tile);
          if ((x, y) == (1, 1)) t.Field("noDownEntry").SetValue(tile);
        }
      }
      @new.name = $"{name} TileSet";
      return @new;
    }

    private Traverse traverse;
    private Traverse Traverse => traverse ?? (traverse = Traverse.Create(this));
    public string StyleName { get; set; }
    public new Color SkyboxColor
    {
      get => base.SkyboxColor;
      set => Traverse.Field("skyboxColor").SetValue(value);
    }
    public new Color ButtonDefaultColor
    {
      get => base.ButtonDefaultColor;
      set => Traverse.Field("buttonDefaultColor").SetValue(value);
    }
    public new Color ButtonHoverColor
    {
      get => base.ButtonHoverColor;
      set => Traverse.Field("buttonHoverColor").SetValue(value);
    }
    public new Color ButtonSelectColor
    {
      get => base.ButtonSelectColor;
      set => Traverse.Field("buttonSelectColor").SetValue(value);
    }
    public new Color UIColor
    {
      get => base.UIColor;
      set => Traverse.Field("uiColor").SetValue(value);
    }
    public new Color UIWhiteColor
    {
      get => base.UIWhiteColor;
      set => Traverse.Field("uiWhiteColor").SetValue(value);
    }
    public new Color SignCompleteIconColor
    {
      get => base.SignCompleteIconColor;
      set => Traverse.Field("signCompleteIconColor").SetValue(value);
    }
    public new Color SignHomeIconColor
    {
      get => base.SignHomeIconColor;
      set => Traverse.Field("signHomeIconColor").SetValue(value);
    }
    public new Color ParticleColor
    {
      get => base.ParticleColor;
      set => Traverse.Field("particleColor").SetValue(value);
    }
    public new Color GridColor
    {
      get => base.GridColor;
      set => Traverse.Field("gridColor").SetValue(value);
    }
    public new Color InTextBoxColor
    {
      get => base.InTextBoxColor;
      set => Traverse.Field("inTextBoxColor").SetValue(value);
    }
    public new Sprite FlowerSprite
    {
      get => base.FlowerSprite;
      set => Traverse.Field("flowerSprite").SetValue(value);
    }
    public new Sprite ClockSprite
    {
      get => base.ClockSprite;
      set => Traverse.Field("clockSprite").SetValue(value);
    }
    public new Sprite DialogueSprite
    {
      get => base.DialogueSprite;
      set => Traverse.Field("dialogueSprite").SetValue(value);
    }
    public new Sprite DialogueNameSprite
    {
      get => base.DialogueNameSprite;
      set => Traverse.Field("dialogueNameSprite").SetValue(value);
    }
    public new Texture2D CursorTexture
    {
      get => base.CursorTexture;
      set => Traverse.Field("cursorTexture").SetValue(value);
    }
    public new BunburrowTileSets TileSets
    {
      get => base.TileSets;
      set => Traverse.Field("tileSets").SetValue(value);
    }
    public new int SpriteSheetsID
    {
      get => base.SpriteSheetsID;
      set => Traverse.Field("spriteSheetsID").SetValue(value);
    }
    public new int BunnySpriteSheetsID
    {
      get => base.BunnySpriteSheetsID;
      set => Traverse.Field("bunnySpriteSheetsID").SetValue(value);
    }
    public new string TextSpritePrefix
    {
      get => base.TextSpritePrefix;
      set => Traverse.Field("textSpritePrefix").SetValue(value);
    }
    public new Bunburrow Bunburrow
    {
      get => base.Bunburrow;
      set => Traverse.Field("bunburrow").SetValue(value);
    }
    public new bool IsVoidBunburrowStyle
    {
      get => base.IsVoidBunburrowStyle;
      set => Traverse.Field("isVoidBunburrowStyle").SetValue(value);
    }
    public new FMODUnity.EventReference Music
    {
      get => base.Music;
      set => Traverse.Field("musicReference").SetValue(value);
    }
  }
}
