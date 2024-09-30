using Bunburrows;
using Bunject.Levels;
using Bunject.NewYardSystem.Levels.Archive;
using Bunject.NewYardSystem.Model;
using Levels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Bunject.NewYardSystem.Levels
{
  public abstract class BNYSModBunburrowBase : IBNYSModBunburrow, IModBunburrow
  {
    public BNYSModBunburrowBase(BNYSPlugin bnys, CustomWorld worldModel, Burrow burrowModel)
    {
      Bnys = bnys;
      World = worldModel;
      BurrowModel = burrowModel;

      InitializeCustomSignCoordinate();
    }

    protected BNYSPlugin Bnys { get; private set; }

    public CustomWorld World { get; private set;}

    public Burrow BurrowModel { get; private set; }

    public string WorldName => World.Title;

    public string LocalName => BurrowModel.Name;

    public string WorldPrefix => World.Prefix;

    public string LocalIndicator => BurrowModel.Indicator;

    public bool IsVoid => BurrowModel.IsVoid;

    // Set by Bunject
    public int ID { get; set; }

    public string Name => WorldName + "::" + LocalName;

    public string Indicator => (string.IsNullOrEmpty(WorldPrefix) ? string.Empty : WorldPrefix + "-") + LocalIndicator;

    private BunburrowStyle style = null;
    public BunburrowStyle Style
    {
      get
      {
        if (style == null)
        {
          style = BNYSPlugin.ResolveStyle(BurrowModel.Style);
        }
        return style;
      }
    }

    public bool HasEntrance => BurrowModel.HasSurfaceEntry;
    public bool HasSign { get; private set; } = true;

    private Vector2Int? customSignCoordinate;
    public Vector2Int? OverrideSignCoordinate()
    {
      return customSignCoordinate;
    }

    private void InitializeCustomSignCoordinate()
    {
      var surfaceCoordinate = World.SurfaceEntries?.Where(se => se.Coordinates != null)
                               ?.SelectMany(se => se.Coordinates)?.Where(kvp => kvp.Key == LocalName)
                               ?.Select(kvp => kvp.Value)?.FirstOrDefault();

      if (surfaceCoordinate != null)
      {
        if (!surfaceCoordinate.NoSign)
        {
          if (surfaceCoordinate.Sign != null && surfaceCoordinate.Sign.Length > 1)
          {
            customSignCoordinate = new Vector2Int(surfaceCoordinate.Sign[0], surfaceCoordinate.Sign[1]);
          }
        }
        else
        {
          HasSign = false;
        }
      }
    }


    public abstract LevelMetadata LoadLevel(int depth);


    private LevelsList levelsList;
    public LevelsList GetLevels()
    {
      if (levelsList == null)
      {
        levelsList = GenerateLevelsList();
      }
      return GetLevels();
    }

    public LevelObject SurfaceLevel { get; set; }
    public LevelObject GetSurfaceLevel()
    {
      return SurfaceLevel;
    }

    protected virtual BNYSLevelsList GenerateLevelsList()
    {
      var levelsList = ScriptableObject.CreateInstance<BNYSLevelsList>();

      levelsList.ModBunburrow = this;
      levelsList.Bnys = Bnys;

      levelsList.name = Name;
      levelsList.MaximumDepth = BurrowModel.Depth;
      levelsList.NumberOfRegularBunnies = BurrowModel.UpperBunnyCount;
      levelsList.NumberOfTempleBunnies = BurrowModel.TempleBunnyCount;
      levelsList.NumberOfHellBunnies = BurrowModel.HellBunnyCount;
      return levelsList;
    }

    protected LevelMetadata CreateDefaultLevelMetadata()
    {
      return new LevelMetadata()
      {
        Name = "Failed Level Load",
        LiveReloading = true,
        IsHell = false,
        IsTemple = false,
        Tools = new LevelTools()
      };
    }
  }
}
