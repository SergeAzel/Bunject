﻿using Bunburrows;
using Bunject.Levels;
using Bunject.NewYardSystem.Levels;
using Bunject.NewYardSystem.Model;
using Bunject.NewYardSystem.Resources;
using Bunject.NewYardSystem.Utility;
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

namespace Bunject.NewYardSystem.Levels.Local
{
  public class BNYSLocalModBunburrow : BNYSModBunburrowBase
  {
    public BNYSLocalModBunburrow(BNYSPlugin bnys, LocalCustomWorld worldModel, Burrow burrowModel) : base(bnys, worldModel, burrowModel)
    {
      World = worldModel;
    }

    public new LocalCustomWorld World { get; private set; }

    public override LevelMetadata LoadLevel(int depth)
    {
      var levelContentPath = Path.Combine(BurrowModel.Directory, $"{depth}.level");
      var levelConfigPath = Path.Combine(BurrowModel.Directory, $"{depth}.json");

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
          Bnys.Logger.LogError($"{Name} - {depth}: Level json failed to load.  Ensure {depth}.json exists and conforms to JSON standards.");

          Bnys.Logger.LogError("Error loading files related to level:");
          Bnys.Logger.LogError(Path.Combine(BurrowModel.Directory, depth.ToString()));
          Bnys.Logger.LogError(e.Message);
          Bnys.Logger.LogError(e);

          levelConfig = CreateDefaultLevelMetadata();
        }
      }
      else
      {
        Bnys.Logger.LogError($"{Name} - {depth}: Level json failed to load.  Ensure {depth}.json exists and conforms to JSON standards.");

        Bnys.Logger.LogError("Error loading files related to level:");
        Bnys.Logger.LogError(Path.Combine(BurrowModel.Directory, depth.ToString()));

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
            Bnys.Logger.LogError(Path.Combine(BurrowModel.Directory, depth.ToString()));
            Bnys.Logger.LogError(e.Message);
            Bnys.Logger.LogError(e);
          }
        }
      }

      if (string.IsNullOrEmpty(content))
      {
        Bnys.Logger.LogError($"{Name} - {depth}: Level content failed to load.  Ensure {depth}.level exists and is appropriately formatted.");
        content = DefaultLevel.Content;
      }
      else
      {
        var validationErrors = ContentValidator.ValidateLevelContent(content);
        if (validationErrors.Count > 0)
        {
          Bnys.Logger.LogWarning($"{Name} - {depth}: Invalid level content found: ");
          foreach (var err in validationErrors)
          {
            if (err.IsWarning)
            {
              Bnys.Logger.LogWarning(err);
            }
            else
            {
              Bnys.Logger.LogError(err);
            }
          }
        }

        if (validationErrors.Any(ve => !ve.IsWarning))
        {
          content = DefaultLevel.Content;
        }
      }

      levelConfig.Style = levelConfig.Style ?? BurrowModel.Style;
      levelConfig.Content = content;

      return levelConfig;
    }
  }
}