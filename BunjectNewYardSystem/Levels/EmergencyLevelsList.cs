using Bunject.Levels;
using Bunject.NewYardSystem.Model;
using Bunject.NewYardSystem.Resources;
using Levels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Bunject.NewYardSystem.Levels
{
  public class EmergencyLevelsList : ModLevelsList
  {
    private LevelObject defaultLevel;
    public BNYSPlugin Bnys { get; set; }

    // Return the default level.
    public override LevelObject LoadLevel(int depth, LoadingContext loadingContext)
    {
      if (defaultLevel == null)
      {
        defaultLevel = GenerateDefaultLevel();
      }
      Bnys.Logger.LogError("Something went wrong - loading default level!");
      return defaultLevel;
    }

    private LevelObject GenerateDefaultLevel()
    {
      var result = ScriptableObject.CreateInstance<ModLevelObject>();

      result.CustomNameKey = "Default Level";
      result.BunburrowName = "Emergency List";
      result.Depth = 1;
      result.Content = DefaultLevel.Content;
      result.BunburrowStyle = AssetsManager.BunburrowsListOfStyles[Bunburrows.Bunburrow.Pink];

      return result;
    }
  }
}
