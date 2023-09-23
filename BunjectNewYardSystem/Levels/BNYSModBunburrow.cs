using Bunburrows;
using Bunject.Levels;
using Bunject.NewYardSystem.Levels;
using Bunject.NewYardSystem.Model;
using Bunject.NewYardSystem.Resources;
using Levels;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Bunject.NewYardSystem.Levels
{
  public class BNYSModBunburrow : IModBunburrow
  {
    private BNYSPlugin bnys;
    private CustomWorld worldModel;
    private Burrow burrowModel;
    public BNYSModBunburrow(BNYSPlugin bnys, CustomWorld worldModel, Burrow burrowModel)
    {
      this.bnys = bnys;
      this.worldModel = worldModel;
      this.burrowModel = burrowModel;

      WorldName = worldModel.Title;
      LocalName = burrowModel.Name;

      WorldPrefix = null;
      LocalIndicator = burrowModel.Indicator;

      IsVoid = burrowModel.IsVoid;

      InitializeCustomSignCoordinate();
    }

    public int ID { get; set; }

    public Burrow Model => burrowModel;
    public CustomWorld World => worldModel;

    public string WorldName { get; set; }
    public string LocalName { get; set; }

    // To be uncommented when world caching is refined
    public string Name => /*WorldName + "::" +*/ LocalName;

    public string WorldPrefix { get; set; }
    public string LocalIndicator { get; set; }
    public string Indicator => /*WorldPrefix + "-" +*/ LocalIndicator;

    public bool IsVoid { get; set; }

    private BunburrowStyle style = null;
    public BunburrowStyle Style
    {
      get
      {
        if (style == null)
        {
          style = BNYSPlugin.ResolveStyle(Model.Style);
        }
        return style;
      }
    }

    public bool HasEntrance => Model.HasSurfaceEntry;
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

    BNYSLevelsList levels = null;
    public BNYSLevelsList GetLevels()
    {
      if (levels == null)
      {
        levels = GenerateLevelsList();
      }
      return levels;
    }

    LevelsList IModBunburrow.GetLevels()
    {
      return GetLevels();
    }

    public LevelObject SurfaceLevel { get; set; }
    public LevelObject GetSurfaceLevel()
    {
      return SurfaceLevel;
    }

    private BNYSLevelsList GenerateLevelsList()
    {
      var levelsList = ScriptableObject.CreateInstance<BNYSLevelsList>();

      levelsList.ModBunburrow = this;
      levelsList.Bnys = bnys;

      levelsList.name = burrowModel.Name;
      levelsList.MaximumDepth = burrowModel.Depth;
      levelsList.NumberOfRegularBunnies = burrowModel.UpperBunnyCount;
      levelsList.NumberOfTempleBunnies = burrowModel.TempleBunnyCount;
      levelsList.NumberOfHellBunnies = burrowModel.HellBunnyCount;
      return levelsList;
    }
  }
}
