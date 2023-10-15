using Bunburrows;
using Bunject.Levels;
using Bunject.NewYardSystem.Model;
using Bunject.NewYardSystem.Resources;
using Bunject.NewYardSystem.Utility;
using Levels;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics.Eventing.Reader;
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

    public override LevelObject LoadLevel(int depth, LoadingContext loadingContext)
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

          ReloadLevel(depth, level);

          this[depth] = level;
        }
        else if (level.ShouldReload && loadingContext == LoadingContext.LevelTransition)
        {
          ReloadLevel(depth, level);
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

      levelObject.ShouldReload = ModBunburrow.World.LiveReloading || metadata.LiveReloading;
      levelObject.LastReloadTime = DateTime.Now;
      levelObject.IsWebLoad = metadata.IsWebLevel;

      PopulateLevel(levelObject, metadata, depth);
    }

    private LevelMetadata LoadLevelFromFile(int depth)
    {
      var levelContentPath = Path.Combine(ModBunburrow.Model.Directory, $"{depth}.level");
      var levelConfigPath = Path.Combine(ModBunburrow.Model.Directory, $"{depth}.json");

      string content = null;
      LevelMetadata levelConfig = null;

      //Logger.LogInfo("Creating Level from: " + levelContentPath);
      if (File.Exists(levelConfigPath))
      {
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

          levelConfig = CreateDefaultLevelMetadata();
        }
      }
      else if (ModBunburrow.Model.ProxyUri != null)
      {
        // Load level via web proxy
        var contentProxyUri = new Uri(ModBunburrow.Model.ProxyUri, $"{depth}.json");
        try
        {
          levelConfig = contentProxyUri.Load<LevelMetadata>();
          levelConfig.IsWebLevel = true;
        }
        catch (Exception e)
        {
          Bnys.Logger.LogError($"{ModBunburrow.Name} - {depth}: File doesn't exist, Level json failed to load from web.  Ensure {depth}.json exists and conforms to JSON standards.");

          Bnys.Logger.LogError("Error loading files related to level:");
          Bnys.Logger.LogError(Path.Combine(ModBunburrow.Model.Directory, depth.ToString()));
          Bnys.Logger.LogError("Expected web endpoint:");
          Bnys.Logger.LogError(contentProxyUri.ToString());
          Bnys.Logger.LogError(e.Message);
          Bnys.Logger.LogError(e);

          levelConfig = CreateDefaultLevelMetadata();
        }
      }
      else
      {
        Bnys.Logger.LogError($"{ModBunburrow.Name} - {depth}: Level json failed to load.  Ensure {depth}.json exists and conforms to JSON standards.");

        Bnys.Logger.LogError("Error loading files related to level:");
        Bnys.Logger.LogError(Path.Combine(ModBunburrow.Model.Directory, depth.ToString()));

        levelConfig = CreateDefaultLevelMetadata();
      }

      if (string.IsNullOrEmpty(levelConfig.Content))
      {
        if (File.Exists(levelContentPath))
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
        else if (ModBunburrow.Model.ProxyUri != null)
        {
          var contentUri = new Uri(ModBunburrow.Model.ProxyUri, $"{depth}.level");

          try
          {
            content = contentUri.Load();
            levelConfig.IsWebLevel = true;
          }
          catch (Exception e)
          {
            Bnys.Logger.LogError("Error loading files related to level:");
            Bnys.Logger.LogError(Path.Combine(ModBunburrow.Model.Directory, depth.ToString()));
            Bnys.Logger.LogError("Expected web path:");
            Bnys.Logger.LogError(contentUri);
            Bnys.Logger.LogError(e.Message);
            Bnys.Logger.LogError(e);
          }
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

      var effects = levelConfig.VisualEffects ?? ModBunburrow.Model.VisualEffects;
      if (effects == null)
			{
        effects = new List<string> { levelConfig.Style };
        if (ModBunburrow.Model.IsVoid)
          effects.Add("Void");
			}
      levelConfig.VisualEffects = effects;

      levelConfig.Content = content;

      return levelConfig;
    }

    private LevelMetadata CreateDefaultLevelMetadata()
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

    private void PopulateLevel(BNYSLevelObject levelObject, LevelMetadata levelConfig, int depth)
    {
      levelObject.name = $"Level {ModBunburrow.Name} - {levelConfig.Name}";

      levelObject.CustomNameKey = levelConfig.Name;
      levelObject.BunburrowStyle = BNYSPlugin.ResolveStyle(levelConfig.Style);

      levelObject.VisualEffects = levelConfig.VisualEffects.Select(BNYSPlugin.ResolveStyle).ToList();

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
