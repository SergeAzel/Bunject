﻿using BepInEx;
using BepInEx.Configuration;
using BepInEx.Logging;
using Bunburrows;
using Bunject;
using Bunject.Levels;
using Bunject.Monitoring;
using Bunject.NewYardSystem.Exceptions;
using Bunject.NewYardSystem.Internal;
using Bunject.NewYardSystem.Levels;
using Bunject.NewYardSystem.Model;
using Bunject.NewYardSystem.Resources;
using Bunject.NewYardSystem.Utility;
using Bunject.Tiling;
using Dialogue;
using HarmonyLib;
using Levels;
using Misc;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Text;
using UnityEngine;
using static UnityEngine.UI.Image;

namespace Bunject.NewYardSystem 
{
  [BepInPlugin(pluginGuid, pluginName, pluginVersion)]
  public class BNYSPlugin : BaseUnityPlugin, IBunjectorPlugin, IMonitor
  {
    public const string pluginGuid = "sergedev.bunject.newyardsystem";
    public const string pluginName = "BNYS";
    public const string pluginVersion = "1.0.10.0";

    public static string rootDirectory = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "BNYS");

    private List<CustomWorld> CustomWorlds;
    private List<IModBunburrow> AllModBurrows;
    private List<BNYSModBunburrow> BNYSModBurrows;

    public new ManualLogSource Logger => base.Logger;

    public void Awake()
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

      var modBunburrows = new List<IModBunburrow>();

      if (CustomWorlds.Count > 0)
      {
        Logger.LogInfo("Initial Load - Building Cached Burrows!");

        foreach (var cachedBurrow in cache.CustomBurrows)
        {
          var burrowModel = CustomWorlds.Where(cw => cachedBurrow.World == cw.Title || string.IsNullOrEmpty(cachedBurrow.World))
            .SelectMany(cw => cw.Burrows).FirstOrDefault(b => b.Name == cachedBurrow.Name);

          var customWorld = CustomWorlds.FirstOrDefault(cw => cw.Burrows.Contains(burrowModel));
          if (burrowModel != null)
          {
            Logger.LogInfo($"Cached Burrow : {burrowModel.Name} found!");

            cachedBurrow.World = customWorld.Title;
            cachedBurrow.Prefix = customWorld.Prefix;
            cachedBurrow.Indicator = burrowModel.Indicator; //update cached indicator if needed

            modBunburrows.Add(new BNYSModBunburrow(this, customWorld, burrowModel));
          }
          else
          {
            Logger.LogInfo($"Cached Burrow : {cachedBurrow.Name} NOT found!");
            // assume levelpack was removed... register it for save file's sake
            modBunburrows.Add(new BNYSLostBunburrow(cachedBurrow));
          }
        }

        Logger.LogInfo("Initial Load - Building Uncached Burrows!");

        foreach (var burrow in CustomWorlds.SelectMany(cw => cw.Burrows))
        {
          var customWorld = CustomWorlds.FirstOrDefault(cw => cw.Burrows.Contains(burrow));
          var cachedBurrow = cache.CustomBurrows.FirstOrDefault(cb => cb.Name == burrow.Name && (cb.World == customWorld.Title || string.IsNullOrEmpty(cb.World)));
          if (cachedBurrow == null)
          {
            Logger.LogInfo($"Uncached Burrow : {burrow.Name} built!");
            var newBurrow = new BNYSModBunburrow(this, customWorld, burrow);
            modBunburrows.Add(newBurrow);
            cache.CacheBunburrow(newBurrow);
          }
        }

        cache.SaveCache();

        AllModBurrows = modBunburrows.ToList();
        BNYSModBurrows = modBunburrows.OfType<BNYSModBunburrow>().ToList();

        LinkLevelLists(BNYSModBurrows);

        foreach (var bunburrow in modBunburrows)
        {
          BunjectAPI.RegisterBunburrow(bunburrow);

          if (bunburrow is BNYSModBunburrow bnysBurrow)
          {
            foreach (var elevatorDepth in bnysBurrow.Model.ElevatorDepths)
            {
              BunjectAPI.RegisterElevator(bunburrow.ID, elevatorDepth);
            }
          }
        }

        BunjectAPI.RegisterPlugin(this);

        Logger.LogInfo("Initial Load Finished!");
      }
      else
      {
        Logger.LogInfo("All worlds empty! Please configure a burrow with a surface entrance, and depth of at least 1!");
      }
    }

    //IBunjector Members
    // IMPORTANT NOTE: DEPTH is 1-indexed.
    public void OnAssetsLoaded()
    {
      SurfaceBurrowsPatch.PatchSurfaceBurrows(AssetsManager.SurfaceRightLevel, null);

      //Now do our level generation if it hasn't been done.
      GenerateSurfaceLevels(AssetsManager.SurfaceRightLevel);
    }

    public void OnProgressionLoaded(GeneralProgression progression)
    {
      progression.HandleBunburrowSignsDiscovery();
      progression.HandleBackToSurfaceUnlock();
      progression.HandleOphelinePortableComputerUnlock();
    }

    public LevelObject StartLevelTransition(LevelObject level, LevelIdentity identity)
    {
      return level;
    }

    private EmergencyLevelsList emergencyList;
    public LevelsList LoadEmergencyLevelsList(LevelsList original)
    {
      if (emergencyList == null)
      {
        emergencyList = ScriptableObject.CreateInstance<EmergencyLevelsList>();
        emergencyList.Bnys = this;
      }
      return emergencyList;
    }

    public void LoadBurrowSurfaceLevel(string listName, LevelObject otherwise)
    {
      //Logger.LogInfo($"Rappelling from {listName}");
      var burrow = CustomWorlds.SelectMany(cw => cw.Burrows).FirstOrDefault(b => b.Name == listName);

      if (burrow != null)
      {
        //Logger.LogInfo($"Burrow found!");
        var world = CustomWorlds.FirstOrDefault(cw => cw.Burrows.Contains(burrow));
        var surfaceIndex = world.Burrows.Where(b => b.Depth > 0 && b.HasSurfaceEntry).ToList().IndexOf(burrow);
        /*
        if (surfaceIndex >= 0 && (surfaceIndex / 3) < world.GeneratedSurfaceLevels.Count)
          return world.GeneratedSurfaceLevels[surfaceIndex / 3];*/

        Logger.LogWarning($"Burrow was not surfaceable / not in the list of generated surface levels!");
      }

      //return base.LoadBurrowSurfaceLevel(listName, otherwise);
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
            if (!world.Burrows.Any(b => b.HasSurfaceEntry && b.Depth > 0) || !world.Enabled)
              continue;

            foreach (var burrow in world.Burrows)
            {
              PatchBurrowDetails(directory, burrow);
            }

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

      if (!string.IsNullOrEmpty(world.ProxyURL))
      {
        try
        {
          world.ProxyUri = new Uri(world.ProxyURL);

          var configUri = new Uri(world.ProxyUri, "config.json");

          CustomWorld proxyWorld = configUri.Load<CustomWorld>();

          if (proxyWorld != null)
          {
            world.Enabled = world.Enabled || proxyWorld.Enabled;
            world.LiveReloading = false; // force global reload off.

            if (string.IsNullOrEmpty(world.Title))
              world.Title = proxyWorld.Title;

            if (proxyWorld.Burrows != null)
            {
              foreach (var proxyBurrow in proxyWorld.Burrows)
              {
                proxyBurrow.ProxyUri = new Uri(world.ProxyUri, $"{proxyBurrow.Directory}/");
              }

              if (world.Burrows == null)
              {
                world.Burrows = proxyWorld.Burrows;
              }
              else
              {
                foreach (var proxyBurrow in proxyWorld.Burrows)
                {
                  if (!world.Burrows.Any(b => b.Name == proxyBurrow.Name))
                  {
                    world.Burrows.Add(proxyBurrow);
                  }
                  else
                  {
                    world.ProxyUri = proxyBurrow.ProxyUri;
                  }
                }
              }

              if (world.SurfaceEntries == null || world.SurfaceEntries.Count == 0)
              {
                world.SurfaceEntries = proxyWorld.SurfaceEntries;
              }
            }
          }
        }
        catch (Exception e)
        {
          Logger.LogError("Error loading Proxy World config.json");
          Logger.LogError(filename);
          Logger.LogError(e.Message);
          Logger.LogError(e);
        }
      }

      if (string.IsNullOrEmpty(world.Title))
        world.Title = "Untitled";

      return world;
    }

    public void PatchBurrowDetails(string directory, Burrow burrow)
    {
      if (!Path.IsPathRooted(burrow.Directory))
      {
        burrow.Directory = Path.Combine(directory, burrow.Directory);
      }
    }

    private bool surfaceLevelsGenerated = false;

    private void GenerateSurfaceLevels(LevelObject coreSurfaceRight)
    {
      if (surfaceLevelsGenerated)
        return;

      var previous = coreSurfaceRight;
      foreach (var world in CustomWorlds)
      {
        try
        {
          ExtendedSurfaceLevelGenerator.CreateSurfaceLevels(world, BNYSModBurrows.Where(b => b.World == world).ToList(), previous);
          previous = world.GeneratedSurfaceLevels.LastOrDefault() ?? previous;
        }
        catch (Exception e)
        {
          Logger.LogError($"Error occurred while generating surface levels for world: {world.Title}");
          Logger.LogError(e.Message);
          Logger.LogError(e);
        }
      }
      PatchLevelAsEndcap(previous);

      surfaceLevelsGenerated = true;
    }

    private void PatchLevelAsEndcap(LevelObject endcapLevel)
    {
      Traverse.Create(endcapLevel).Field("specificBackground").SetValue(SurfaceBurrowsPatch.EndingBackground);
    }

    private void LinkLevelLists(List<BNYSModBunburrow> burrows)
    {
      foreach (var burrow in burrows)
      {
        if (!string.IsNullOrEmpty(burrow.Model.Links.Left))
        {
          var target = burrows.FirstOrDefault(bb => bb.LocalName == burrow.Model.Links.Left);
          if (target != null)
            burrow.GetLevels().AdjacentBunburrows.SetPart(Direction.Left, target.GetLevels());
        }
        if (!string.IsNullOrEmpty(burrow.Model.Links.Up))
        {
          var target = burrows.FirstOrDefault(bb => bb.LocalName == burrow.Model.Links.Up);
          if (target != null)
            burrow.GetLevels().AdjacentBunburrows.SetPart(Direction.Up, target.GetLevels());
        }
        if (!string.IsNullOrEmpty(burrow.Model.Links.Right))
        {
          var target = burrows.FirstOrDefault(bb => bb.LocalName == burrow.Model.Links.Right);
          if (target != null)
            burrow.GetLevels().AdjacentBunburrows.SetPart(Direction.Right, target.GetLevels());
        }
        if (!string.IsNullOrEmpty(burrow.Model.Links.Down))
        {
          var target = burrows.FirstOrDefault(bb => bb.LocalName == burrow.Model.Links.Down);
          if (target != null)
            burrow.GetLevels().AdjacentBunburrows.SetPart(Direction.Down, target.GetLevels());
        }
      }
    }

    public static BunburrowStyle ResolveStyle(string style)
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
  }
}
