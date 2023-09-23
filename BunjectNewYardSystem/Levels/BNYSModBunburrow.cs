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

    public BNYSLevelObject GetLevel(int depth)
    {
      var levels = GetLevels();
      if (depth > 0 && depth <= levels.MaximumDepth)
      {
        var level = levels[depth];
        if (level == null)
        {
          level = ScriptableObject.CreateInstance<BNYSLevelObject>();
          // Store off the ever-important burrow names and such
          level.BunburrowName = burrowModel.Name;
          level.Depth = depth;
        }

        if (level.ShouldReload)
        {
          ReloadLevel(depth, level);

          levels[depth] = level;
        }

        return level;
      }
      else
      {
        bnys.Logger.LogWarning($"{Name}: Depth requested, {depth}, exceeds burrow depth of {levels.MaximumDepth}");
        return null;
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

    LevelObject IModBunburrow.GetLevel(int depth)
    {
      return GetLevel(depth);
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

      levelsList.name = burrowModel.Name;
      levelsList.MaximumDepth = burrowModel.Depth;
      levelsList.NumberOfRegularBunnies = burrowModel.UpperBunnyCount;
      levelsList.NumberOfTempleBunnies = burrowModel.TempleBunnyCount;
      levelsList.NumberOfHellBunnies = burrowModel.HellBunnyCount;
      return levelsList;
    }

    // Repopulates the levelObject from file contents
    private void ReloadLevel(int depth, BNYSLevelObject levelObject)
    {
      var metadata = LoadLevelFromFile(depth);

      PopulateLevel(levelObject, metadata, depth);
    }

    private LevelMetadata LoadLevelFromFile(int depth)
    {
      var levelContentPath = Path.Combine(burrowModel.Directory, $"{depth}.level");
      var levelConfigPath = Path.Combine(burrowModel.Directory, $"{depth}.json");

      string content = null;
      LevelMetadata levelConfig = null;

      //Logger.LogInfo("Creating Level from: " + levelContentPath);

      try
      {
        using (var reader = new StreamReader(levelConfigPath))
        {
          levelConfig = (LevelMetadata)new JsonSerializer().Deserialize(reader, typeof(LevelMetadata));
        }
      }
      catch (Exception e)
      {
        bnys.Logger.LogError($"{LocalName} - {depth}: Level json failed to load.  Ensure {depth}.json exists and conforms to JSON standards.");

        bnys.Logger.LogError("Error loading files related to level:");
        bnys.Logger.LogError(Path.Combine(burrowModel.Directory, depth.ToString()));
        bnys.Logger.LogError(e.Message);
        bnys.Logger.LogError(e);

        levelConfig = new LevelMetadata()
        {
          Name = "Failed Level Load",
          LiveReloading = true,
          IsHell = false,
          IsTemple = false,
          Tools = new LevelTools()
        };
      }

      if (string.IsNullOrEmpty(levelConfig.Content))
      {
        try
        {
          content = File.ReadAllText(levelContentPath);
        }
        catch (Exception e)
        {
          bnys.Logger.LogError("Error loading files related to level:");
          bnys.Logger.LogError(Path.Combine(burrowModel.Directory, depth.ToString()));
          bnys.Logger.LogError(e.Message);
          bnys.Logger.LogError(e);
        }
      }

      if (string.IsNullOrEmpty(content))
      {
        bnys.Logger.LogError($"{LocalName} - {depth}: Level content failed to load.  Ensure {depth}.level exists and is appropriately formatted.");
        content = DefaultLevel.Content;
      }
      else if (!ContentValidator.ValidateLevelContent(content))
      {
        bnys.Logger.LogError($"{LocalName} - {depth}: Invalid tiles detected.");
        content = DefaultLevel.Content;
      } 

      levelConfig.Style = levelConfig.Style ?? burrowModel.Style;
      levelConfig.Content = content;

      return levelConfig;
    }

    private void PopulateLevel(BNYSLevelObject levelObject, LevelMetadata levelConfig, int depth)
    {
      levelObject.name = $"Level {burrowModel.Name} - {levelConfig.Name}";

      // Prepend name with space -- hack
      levelObject.CustomNameKey = " " + levelConfig.Name;
      levelObject.BunburrowStyle = BNYSPlugin.ResolveStyle(levelConfig.Style);

      if (levelConfig.Tools is LevelTools tools)
      {
        levelObject.NumberOfTraps = tools.Traps;
        levelObject.NumberOfPickaxes = tools.Pickaxes;
        levelObject.NumberOfCarrots = tools.Carrots;
        levelObject.NumberOfShovels = tools.Shovels;
      }
      levelObject.IsTemple = levelConfig.IsTemple;
      levelObject.IsHell = levelConfig.IsHell;

      levelObject.Content = levelConfig.Content;
    }
  }
}
