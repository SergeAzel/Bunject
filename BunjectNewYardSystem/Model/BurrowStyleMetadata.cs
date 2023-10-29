using Bunburrows;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Bunject.NewYardSystem.Model
{
  public class BurrowStyleMetadata
  {
    [JsonIgnore]
    public string StyleName { get; set; }
    // TODO: remove Pink once it can handle creating new paq and bnuuy spreasheet
    public string AltStyle { get; set; } = "Pink";
    // Unsupported
    // TODO: support
    // public string TextSpritePrefix { get; set; }
    public string SkyboxColor { get; set; } = "#FFFFFF";
    public string ButtonDefaultColor { get; set; } = "#FFFFFF";
    public string ButtonHoverColor { get; set; } = "#FFFFFF";
    public string ButtonSelectColor { get; set; } = "#FFFFFF";
    public string UIColor { get; set; } = "#FFFFFF";
    public string UIWhiteColor { get; set; } = "#FFFFFF";
    public string SignCompleteIconColor { get; set; } = "#FFFFFF";
    public string SignHomeIconColor { get; set; } = "#FFFFFF";
    public string ParticleColor { get; set; } = "#FFFFFF";
    public string GridColor { get; set; } = "#FFFFFF";
    public string InTextBoxColor { get; set; } = "#FFFFFF";
    public SpriteData FlowerSprite { get; set; }
    public SpriteData ClockSprite { get; set; }
    public SpriteData DialogueSprite { get; set; }
    public SpriteData DialogueNameSprite { get; set; }
    public SpriteData Cursor { get; set; }
    public TileSetData TileSets { get; set; }
  }
  public class TileSetData
  {
    public AnimatedSpriteData Floors { get; set; }
    public List<AnimatedSpriteData> FloorProps { get; set; }
    public float FloorPropProbability { get; set; } = 0.05f;
    public SpriteData UnbreakableFloor { get; set; }
    public SpriteData BurningFloorOverlay { get; set; }
    public AnimatedSpriteData Walls { get; set; }
    public List<AnimatedSpriteData> WallProps { get; set; }
    public float WallPropProbability { get; set; } = 0.02f;
    public List<SpriteData> WallBreak { get; set; }
    public AnimatedSpriteData UnbreakableWalls { get; set; }
    public List<AnimatedSpriteData> UnbreakableWallProps { get; set; }
    public float UnbreakableWallPropProbability { get; set; } = 0.02f;
    public List<SpriteData> UnbreakableWallCorners { get; set; }
    public AnimatedSpriteData Tunnels { get; set; }
    public List<SpriteData> TunnelEntries { get; set; }
    public List<SpriteData> TunnelFloorOverlays { get; set; }
    public SpriteData Rope { get; set; }
    public SpriteData Exit { get; set; }
    public SpriteData ClosedExit { get; set; }
    public SpriteData Carrot { get; set; }
    public SpriteData Trap { get; set; }
    public SpriteData ElevatorOpen { get; set; }
    public SpriteData ElevatorClosed { get; set; }
    public SpriteData ElevatorDown { get; set; }
    public SpriteData ElevatorUp { get; set; }
    public List<AnimatedSpriteData> SpecialTiles { get; set; }
    public List<SpriteData> UIItems { get; set; }
    public List<SpriteData> UIBunnies { get; set; }
    public SpriteData UIItemCount { get; set; }
    public SpriteData DarkPickaxeOverlay { get; set; }
  }
  public class SpriteData
  {
    public string Path { get; set; } = "";
    public int X { get; set; } = 0;
    public int Y { get; set; } = 0;
    [JsonIgnore]
    public Vector2Int Position => new Vector2Int(X, Y);
  }
  public class AnimatedSpriteData : SpriteData
  {
    public int Frames { get; set; } = 1;
    public float MinSpeed { get; set; } = 1;
    public float MaxSpeed { get; set; } = 1;
    public (float min, float max) Speed => (MinSpeed, MaxSpeed);

  }
}
