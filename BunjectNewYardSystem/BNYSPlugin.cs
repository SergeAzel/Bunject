using BepInEx;
using BepInEx.Configuration;
using BepInEx.Logging;
using Bunburrows;
using Bunject;
using Bunject.NewYardSystem.Internal;
using Bunject.NewYardSystem.Levels;
using Bunject.NewYardSystem.Model;
using Bunject.NewYardSystem.Resources;
using Dialogue;
using HarmonyLib;
using Levels;
using Misc;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using UnityEngine;
using static UnityEngine.UI.Image;

namespace Bunject.NewYardSystem 
{
  [BepInPlugin(pluginGuid, pluginName, pluginVersion)]
  public class BNYSPlugin : BaseBunjector
  {
    public const string pluginGuid = "sergedev.bunject.newyardsystem";
    public const string pluginName = "BNYS";
    public const string pluginVersion = "1.0.9";

    public static string rootDirectory = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "BNYS");

    private List<CustomWorld> CustomWorlds;

    public override void Awake()
    {
      Logger.LogInfo($"Bunject New Yard System [BNYS] Plugin Awakened. v{pluginVersion}");

      BunjectAPI.SaveFolder = "BNYS";

      // Get cached worlds first - to preserve registration order (which preserves IDs generated)
      // Not the most elegant solution, but I'm just trying to get it functional for now.
      var cache = new CustomBunburrowCache();

      try
      {
        CustomWorlds = LoadCustomWorlds().ToList();
      }
      catch (Exception e)
      {
        Logger.LogError("Error caught on loading custom burrows... ");
        Logger.LogError(e.Message);
        Logger.LogError(e);

        return;
      }

      if (CustomWorlds.Count > 0)
      {
        Logger.LogInfo("Initial Load - Registering Cached Burrows!");

        foreach (var cachedBurrow in cache.CustomBurrows)
        {
          var burrowModel = CustomWorlds.SelectMany(cw => cw.Burrows).FirstOrDefault(b => b.Name == cachedBurrow.Name);
          if (burrowModel != null)
          {
            Logger.LogInfo($"Cached Burrow : {burrowModel.Name} found!");
            cachedBurrow.Indicator = burrowModel.Indicator; //update cached indicator if needed
            burrowModel.ID = BunjectAPI.RegisterBurrow(burrowModel.Name, burrowModel.Indicator, burrowModel.IsVoid);
          }
          else
          {
            Logger.LogInfo($"Cached Burrow : {cachedBurrow.Name} NOT found!");
            // assume levelpack was removed... register it for save file's sake
            BunjectAPI.RegisterBurrow(cachedBurrow.Name, cachedBurrow.Indicator, false);
          }
        }

        Logger.LogInfo("Initial Load - Registering Uncached Burrows!");

        foreach (var burrow in CustomWorlds.SelectMany(cw => cw.Burrows))
        {
          var cachedBurrow = cache.CustomBurrows.FirstOrDefault(cb => cb.Name == burrow.Name);
          if (cachedBurrow == null)
          {
            Logger.LogInfo($"Uncached Burrow : {burrow.Name} registered!");
            burrow.ID = BunjectAPI.RegisterBurrow(burrow.Name, burrow.Indicator, burrow.IsVoid);
            cache.CacheBunburrow(burrow.Name, burrow.Indicator);
          }
        }

        cache.SaveCache();

        BunjectAPI.Register(this);

        Logger.LogInfo("Initial Load Finished!");
      }
      else
      {
        Logger.LogInfo("All worlds empty! Please configure a burrow with a surface entrance, and depth of at least 1!");
      }
    }

    //IBunjector Members
    // IMPORTANT NOTE: DEPTH is 1-indexed.
    public override void OnAssetsLoaded()
    {
      SurfaceBurrowsPatch.PatchSurfaceBurrows(AssetsManager.SurfaceRightLevel, null);

      //Now do our level generation if it hasn't been done.
      GenerateSurfaceLevels(AssetsManager.SurfaceRightLevel);
    }

    public override void OnProgressionLoaded(GeneralProgression progression)
    {
      progression.HandleBackToSurfaceUnlock();
      progression.HandleOphelineComputerUnlock();
      progression.HandleOphelinePortableComputerUnlock();
    }

    public override LevelObject LoadLevel(string listName, int depth, LevelObject original)
    {
      if (original == null)
      {
        //Logger.LogInfo($"LoadLevel Endpoint Called: {listName}, {depth}");
        //Maybe dictionary these by name... but honestly given dataset size, doesnt matter right now.
        var world = CustomWorlds.FirstOrDefault(cw => cw.Burrows.Any(b => b.Name == listName));

        if (world == null)
        {
          Logger.LogWarning($"Burrow {listName} not found.");
          return GetDefaultLevel();
        }

        var burrow = CustomWorlds.SelectMany(cw => cw.Burrows).FirstOrDefault(b => b.Name == listName);
        if (burrow.Levels.Length >= depth && depth > 0)
        {
          //Bypass the LevelLoad hook
          var levelsTraverse = Traverse.Create(burrow.Levels).Field<List<LevelObject>>("list");

          var resultLevel = levelsTraverse.Value[depth - 1];
          if (resultLevel == null)
          {
            var levelConfig = LoadLevelFromFile(burrow.Directory, burrow.Name, depth, burrow.Style);
            resultLevel = levelConfig.Level;

            if (!world.LiveReloading && !levelConfig.LiveReloading)
              levelsTraverse.Value[depth - 1] = resultLevel;
          }

          if (resultLevel == null)
            Logger.LogError($"Level {listName}-{depth} failed to generate");

          return resultLevel ?? GetDefaultLevel();
        }

        Logger.LogError($"Level {listName}-{depth} failed to generate - Depth Check Failure!");
        return GetDefaultLevel();
      }
      return original;
    }

    public override LevelsList LoadLevelsList(string name, LevelsList original)
    {
      if (original == null)
      {
        //Logger.LogInfo($"LoadLevelsList Endpoint Called: {name}");
        var burrow = CustomWorlds.SelectMany(cw => cw.Burrows).FirstOrDefault(b => b.Name == name);
        return burrow?.Levels ?? GetDefaultLevelsList();
      }
      return original;
    }

    public override LevelObject RappelFromBurrow(string listName, LevelObject otherwise)
    {
      //Logger.LogInfo($"Rappelling from {listName}");
      var burrow = CustomWorlds.SelectMany(cw => cw.Burrows).FirstOrDefault(b => b.Name == listName);

      if (burrow != null)
      {
        //Logger.LogInfo($"Burrow found!");
        var world = CustomWorlds.FirstOrDefault(cw => cw.Burrows.Contains(burrow));
        var surfaceIndex = world.Burrows.Where(b => b.Depth > 0 && b.HasSurfaceEntry).ToList().IndexOf(burrow);
        if (surfaceIndex >= 0 && (surfaceIndex / 3) < world.GeneratedSurfaceLevels.Count)
          return world.GeneratedSurfaceLevels[surfaceIndex / 3];

        Logger.LogWarning($"Burrow was not surfaceable / not in the list of generated surface levels!");
      }

      return base.RappelFromBurrow(listName, otherwise);
    }

		public override bool ValidateBaseTile(LevelObject levelObject, string tile)
		{
      return ContentValidator.ValidateBaseTile(levelObject, tile);
		}
		public override bool ValidateModTile(LevelObject levelObject, string tile)
    {
      return ContentValidator.ValidateModTile(levelObject, tile);
    }
		//IBunjector Members End

		public IEnumerable<CustomWorld> LoadCustomWorlds()
    {
      // loop through all subfolders of our root
      foreach (var directory in Directory.EnumerateDirectories(rootDirectory))
      {
        var configFile = Path.Combine(directory, "config.json");
        if (File.Exists(configFile))
        {
          CustomWorld world = LoadWorldConfig(configFile);
          if (world != null)
          {
            if (world.Burrows.Any(x => x.ElevatorDepths.Any()))
            {
              foreach (var burrow in world.Burrows)
              {
                foreach (var depth in burrow.ElevatorDepths)
                  BunjectAPI.RegisterElevator(burrow.Indicator, depth);
              }
            }
            if (!world.Burrows.Any(b => b.HasSurfaceEntry && b.Depth > 0) || !world.Enabled)
              continue;

            foreach (var burrow in world.Burrows)
            {
              PatchBurrowDetails(directory, burrow);
            }

            LinkLevelLists(world.Burrows);
            yield return world;
          }
        }
      }
    }

    private CustomWorld LoadWorldConfig(string filename)
    {
      CustomWorld world = null;
      try
      {
        using (var reader = new StreamReader(filename))
        {
          world = (CustomWorld)new JsonSerializer().Deserialize(reader, typeof(CustomWorld));
        }
      }
      catch (Exception e)
      {
        Logger.LogError("Following file could not be parsed");
        Logger.LogError(filename);
        Logger.LogError(e.Message);
        Logger.LogError(e);
      }
      return world;
    }

    public void PatchBurrowDetails(string directory, Burrow burrow)
    {
      if (!Path.IsPathRooted(burrow.Directory))
      {
        burrow.Directory = Path.Combine(directory, burrow.Directory);
      }

      burrow.Levels = GenerateLevelsList(burrow);

    }

    private bool surfaceLevelsGenerated = false;

    private void GenerateSurfaceLevels(LevelObject original)
    {
      if (surfaceLevelsGenerated)
        return;

      var previous = original;
      foreach (var world in CustomWorlds)
      {
        ExtendedBurrowLevelGenerator.CreateSurfaceLevels(world, previous);
        previous = world.GeneratedSurfaceLevels.LastOrDefault() ?? previous;
      }

      PatchLevelAsEndcap(previous);

      surfaceLevelsGenerated = true;
    }

    private void PatchLevelAsEndcap(LevelObject endcapLevel)
    {
      Traverse.Create(endcapLevel).Field("specificBackground").SetValue(SurfaceBurrowsPatch.EndingBackground);
    }

    private LevelsList GenerateLevelsList(Burrow burrow)
    {
      var levelsList = ScriptableObject.CreateInstance<LevelsList>();

      levelsList.name = burrow.Name;

      var traverse = Traverse.Create(levelsList);

      var internalList = new LevelObject[burrow.Depth].ToList(); //emtpy level list - fill on load as we need them
      traverse.Field<List<LevelObject>>("list").Value = internalList;
      traverse.Field<int>("numberOfRegularBunnies").Value = burrow.UpperBunnyCount;
      traverse.Field<int>("numberOfTempleBunnies").Value = burrow.TempleBunnyCount;
      traverse.Field<int>("numberOfHellBunnies").Value = burrow.HellBunnyCount;
      traverse.Field<DirectionsListOf<LevelsList>>("adjacentBunburrows").Value = new DirectionsListOf<LevelsList>(null, null, null, null);
      return levelsList;
    }

    private void LinkLevelLists(List<Burrow> burrows)
    {
      foreach (var burrow in burrows)
      {
        if (burrow.Links == null)
          continue;

        if (!string.IsNullOrEmpty(burrow.Links.Left))
        {
          var target = burrows.FirstOrDefault(bb => bb.Name == burrow.Links.Left)?.Levels;
          burrow.Levels.AdjacentBunburrows.SetPart(Direction.Left, target);
        }
        if (!string.IsNullOrEmpty(burrow.Links.Up))
        {
          var target = burrows.FirstOrDefault(bb => bb.Name == burrow.Links.Up)?.Levels;
          burrow.Levels.AdjacentBunburrows.SetPart(Direction.Up, target);
        }
        if (!string.IsNullOrEmpty(burrow.Links.Right))
        {
          var target = burrows.FirstOrDefault(bb => bb.Name == burrow.Links.Right)?.Levels;
          burrow.Levels.AdjacentBunburrows.SetPart(Direction.Right, target);
        }
        if (!string.IsNullOrEmpty(burrow.Links.Down))
        {
          var target = burrows.FirstOrDefault(bb => bb.Name == burrow.Links.Down)?.Levels;
          burrow.Levels.AdjacentBunburrows.SetPart(Direction.Down, target);
        }
      }
    }

    private LevelMetadata LoadLevelFromFile(string directory, string burrowName, int depth, string defaultStyle)
    {
      var levelContentPath = Path.Combine(directory, $"{depth}.level");
      var levelConfigPath = Path.Combine(directory, $"{depth}.json");

      string content = null;
      LevelMetadata levelConfig = null;

      //Logger.LogInfo("Creating Level from: " + levelContentPath);
      try
      {
        content = File.ReadAllText(levelContentPath);
      }
      catch (Exception e)
      {
        Logger.LogError("Error loading files related to level:");
        Logger.LogError(Path.Combine(directory, depth.ToString()));
        Logger.LogError(e.Message);
        Logger.LogError(e);
      }

      try
      { 
        using (var reader = new StreamReader(levelConfigPath))
        {
          levelConfig = (LevelMetadata)new JsonSerializer().Deserialize(reader, typeof(LevelMetadata));
        }
      }
      catch (Exception e)
      {
        Logger.LogError("Error loading files related to level:");
        Logger.LogError(Path.Combine(directory, depth.ToString()));
        Logger.LogError(e.Message);
        Logger.LogError(e);
      }

      GenerateCustomLevelObject(levelConfig, content, defaultStyle, burrowName, depth);

      return levelConfig;
    }
    private void GenerateCustomLevelObject(LevelMetadata levelConfig, string content, string defaultStyle, string burrowName, int depth)
    {
      if (levelConfig is null)
      {
        Logger.LogError($"{burrowName} - {depth}: Level json failed to load.  Ensure {depth}.json exists and conforms to JSON standards.");

        levelConfig = new LevelMetadata()
        {
          Name = "Level Metadata Failed Load",
          LiveReloading = true,
          Style = defaultStyle,
          IsHell = false,
          IsTemple = false,
          Tools = new LevelTools()
        };
      }

      var resultLevel = ScriptableObject.CreateInstance<CustomLevelObject>();
      resultLevel.name = $"Level {burrowName} - {levelConfig.Name}";
      var level = Traverse.Create(resultLevel);
      level.Field("dialogues").SetValue(new List<DialogueObject>());
      level.Field("contextualDialogues").SetValue(new List<ContextualDialogueInfo>());
      level.Field("sideLevels").SetValue(new DirectionsListOf<LevelObject>(null, null, null, null));

        // Prepend name with space -- hack
      level.Field("customNameKey").SetValue(" " + levelConfig.Name);
      level.Field("bunburrowStyle").SetValue(StyleFromString(string.IsNullOrEmpty(levelConfig.Style) ? defaultStyle : levelConfig.Style));
      if (levelConfig.Tools is LevelTools tools)
      {
        level.Field("numberOfTraps").SetValue(tools.Traps);
        level.Field("numberOfPickaxes").SetValue(tools.Pickaxes);
        level.Field("numberOfCarrots").SetValue(tools.Carrots);
        level.Field("numberOfShovels").SetValue(tools.Shovels);
      }
      level.Field("isTemple").SetValue(levelConfig.IsTemple);
      level.Field("isHell").SetValue(levelConfig.IsHell);

      if (string.IsNullOrWhiteSpace(content))
      {
        Logger.LogError($"{burrowName} - {depth}: Level content failed to load.  Ensure {depth}.level exists and is appropriately formatted.");
        level.Field("content").SetValue(DefaultLevel.Content);
      }
      else
      {
        level.Field("content").SetValue(content);
        if (!ContentValidator.ValidateLevelObject(level, content))
        {
          Logger.LogError($"{burrowName} - {depth}: Level content failed to load.  Invalid tiles detected.");
          level.Field("content").SetValue(DefaultLevel.Content);
        }
      }

      levelConfig.Level = resultLevel;
    }
    public static BunburrowStyle StyleFromString(string style)
    {
      switch (style)
      {
        case "Aquatic":
        case "Sunken":
          return AssetsManager.BunburrowsListOfStyles[Bunburrow.Aquatic];
        case "Hay":
          return AssetsManager.BunburrowsListOfStyles[Bunburrow.Hay];
        case "Forgotten":
        case "Purple":
          return AssetsManager.BunburrowsListOfStyles[Bunburrow.Purple];
        case "Spooky":
        case "Ghostly":
          return AssetsManager.BunburrowsListOfStyles[Bunburrow.Ghostly];
        case "Void":
          return AssetsManager.BunburrowsListOfStyles.VoidB;
        case "Temple":
          return AssetsManager.BunburrowsListOfStyles.Temple;
        case "Hell":
          return AssetsManager.BunburrowsListOfStyles.Hell;
        case "HellTemple":
          return AssetsManager.BunburrowsListOfStyles.HellTemple;
        case "Pink":
        default:
          return AssetsManager.BunburrowsListOfStyles[Bunburrow.Pink];
      }
    }

    private LevelsList defaultLevelsList = null;
    private LevelsList GetDefaultLevelsList()
    {
      Logger.LogWarning("Getting default levels list");
      if (defaultLevelsList == null)
      {
        var levelsList = ScriptableObject.CreateInstance<LevelsList>();

        levelsList.name = Guid.NewGuid().ToString();

        var traverse = Traverse.Create(levelsList);

        traverse.Field<List<LevelObject>>("list").Value = new List<LevelObject>();
        traverse.Field<DirectionsListOf<LevelsList>>("adjacentBunburrows").Value = new DirectionsListOf<LevelsList>(null, null, null, null);

        defaultLevelsList = levelsList;
      }
      return defaultLevelsList;
    }

    private LevelObject defaultLevel = null;
    private LevelObject GetDefaultLevel()
    {
      if (defaultLevel == null)
      {
        var resultLevel = ScriptableObject.CreateInstance<LevelObject>();
        var level = Traverse.Create(resultLevel);

        // Prepend name with space -- hack
        level.Field("customNameKey").SetValue("Default Level");

        level.Field("dialogues").SetValue(new List<DialogueObject>());
        level.Field("contextualDialogues").SetValue(new List<ContextualDialogueInfo>());

        var targetStyle =  AssetsManager.BunburrowsListOfStyles[Bunburrow.Pink];
        level.Field("bunburrowStyle").SetValue(targetStyle);
        level.Field("content").SetValue(DefaultLevel.Content);
        level.Field("sideLevels").SetValue(new DirectionsListOf<LevelObject>(null, null, null, null));

        defaultLevel = resultLevel;
      }
      return defaultLevel;
    }
  }
}
