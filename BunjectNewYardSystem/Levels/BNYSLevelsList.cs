using Bunburrows;
using Bunject.Levels;
using Bunject.NewYardSystem.Model;
using Bunject.NewYardSystem.Resources;
using Levels;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Bunject.NewYardSystem.Levels
{
  public class BNYSLevelsList : ModLevelsList
  {
    public BNYSModBunburrow ModBunburrow { get; set; }
    public BNYSPlugin Bnys { get; set; }
    
    public new BNYSLevelObject this[int depth]
    {
      get { return base[depth] as BNYSLevelObject; }
      set { base[depth] = value; }
    }

    public override LevelObject GetLevel(int depth)
    {
      if (depth > 0 && depth <= MaximumDepth)
      {
        var level = this[depth];
        if (level == null)
        {
          level = ScriptableObject.CreateInstance<BNYSLevelObject>();
          // Store off the ever-important burrow names and such
          level.BunburrowName = ModBunburrow.Model.Name;
          level.Depth = depth;
        }

        if (level.ShouldReload)
        {
          ReloadLevel(depth, level);

          this[depth] = level;
        }

        return level;
      }
      else
      {
        Bnys.Logger.LogWarning($"{ModBunburrow.Name}: Depth requested, {depth}, exceeds burrow depth of {MaximumDepth}");
        return null;
      }
    }

    // Repopulates the levelObject from file contents
    private void ReloadLevel(int depth, BNYSLevelObject levelObject)
    {
      var metadata = LoadLevelFromFile(depth);

      PopulateLevel(levelObject, metadata, depth);
    }

    private LevelMetadata LoadLevelFromFile(int depth)
    {
      var levelContentPath = Path.Combine(ModBunburrow.Model.Directory, $"{depth}.level");
      var levelConfigPath = Path.Combine(ModBunburrow.Model.Directory, $"{depth}.json");

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
        Bnys.Logger.LogError($"{ModBunburrow.Name} - {depth}: Level json failed to load.  Ensure {depth}.json exists and conforms to JSON standards.");

        Bnys.Logger.LogError("Error loading files related to level:");
        Bnys.Logger.LogError(Path.Combine(ModBunburrow.Model.Directory, depth.ToString()));
        Bnys.Logger.LogError(e.Message);
        Bnys.Logger.LogError(e);

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
          Bnys.Logger.LogError("Error loading files related to level:");
          Bnys.Logger.LogError(Path.Combine(ModBunburrow.Model.Directory, depth.ToString()));
          Bnys.Logger.LogError(e.Message);
          Bnys.Logger.LogError(e);
        }
      }

      if (string.IsNullOrEmpty(content))
      {
        Bnys.Logger.LogError($"{ModBunburrow.Name} - {depth}: Level content failed to load.  Ensure {depth}.level exists and is appropriately formatted.");
        content = DefaultLevel.Content;
      }
      else if (!ContentValidator.ValidateLevelContent(content))
      {
        Bnys.Logger.LogError($"{ModBunburrow.Name} - {depth}: Invalid tiles detected.");
        content = DefaultLevel.Content;
      }

      levelConfig.Style = levelConfig.Style ?? ModBunburrow.Model.Style;
      levelConfig.Content = content;

      return levelConfig;
    }

    private void PopulateLevel(BNYSLevelObject levelObject, LevelMetadata levelConfig, int depth)
    {
      levelObject.name = $"Level {ModBunburrow.Name} - {levelConfig.Name}";

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
