using Bunburrows;
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

namespace Bunject.NewYardSystem.Levels.Web
{
  // Copied from BNYSFileModBunburrow - probably want to restructure everything at some point
  public class BNYSWebModBunburrow : BNYSModBunburrowBase
  {
    public BNYSWebModBunburrow(BNYSPlugin bnys, WebCustomWorld worldModel, Burrow burrowModel) : base(bnys, worldModel, burrowModel)
    {
      World = worldModel;
    }

    public new WebCustomWorld World { get; private set; }

    protected override BNYSLevelsList GenerateLevelsList()
    {
      var list = base.GenerateLevelsList();
      list.PermitReloading = true;
      list.DelayReloading = true;
      return list;
    }

    public override LevelMetadata LoadLevel(int depth)
    {
      string content = null;
      LevelMetadata levelConfig = null;

      // Load level via web proxy
      var contentProxyUri = new Uri(BurrowModel.ProxyUri, $"{depth}.json");
      try
      {
        levelConfig = contentProxyUri.Load<LevelMetadata>();
        levelConfig.IsWebLevel = true;
      }
      catch (Exception e)
      {
        Bnys.Logger.LogError($"{Name} - {depth}: File doesn't exist, Level json failed to load from web.  Ensure {depth}.json exists and conforms to JSON standards.");

        Bnys.Logger.LogError("Error loading files related to level:");
        Bnys.Logger.LogError(Path.Combine(BurrowModel.Directory, depth.ToString()));
        Bnys.Logger.LogError("Expected web endpoint:");
        Bnys.Logger.LogError(contentProxyUri.ToString());
        Bnys.Logger.LogError(e.Message);
        Bnys.Logger.LogError(e);

        levelConfig = CreateDefaultLevelMetadata();
      }

      if (string.IsNullOrEmpty(levelConfig.Content))
      {
        var contentUri = new Uri(BurrowModel.ProxyUri, $"{depth}.level");

        try
        {
          content = contentUri.Load();
          levelConfig.IsWebLevel = true;
        }
        catch (Exception e)
        {
          Bnys.Logger.LogError("Error loading files related to level:");
          Bnys.Logger.LogError(Path.Combine(BurrowModel.Directory, depth.ToString()));
          Bnys.Logger.LogError("Expected web path:");
          Bnys.Logger.LogError(contentUri);
          Bnys.Logger.LogError(e.Message);
          Bnys.Logger.LogError(e);
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
