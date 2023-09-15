using Bunject.Levels;
using Bunject.NewYardSystem.Model;
using Dialogue;
using HarmonyLib;
using Levels;
using Microsoft.SqlServer.Server;
using Misc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Bunject.NewYardSystem.Levels
{
  // TODO - refactor this whole thing.  Currently just getting it functional with the new structure paradigm
  internal class ExtendedBurrowLevelGenerator
  {
    public const string WallRow = "W,W,W,W,W,W,W,W,W,W,W,W,W,W,W";
    public const string TopRow = "W,T,T,T,T,T,T,T,T,T,T,T,W{0},W,W";
    public const string SpaceRow = "W,W{0},T,T,T,T,T,T,T,T,T,T,T,W,W";
    public const string EntryRow = "W,T,T,T,{0},T,T,{1},T,T,{2},T,T,W,W";
    public const string OpenRow = "T,T,T,T,T,T,T,T,T,T,T,T,T,T,T";

    //Should only be run after burrows are registered.  Burrow ID is required for generation.
    public static void CreateSurfaceLevels(CustomWorld world, List<BNYSModBunburrow> modBunburrows, LevelObject precedingLevel)
    {
      if (world.GeneratedSurfaceLevels == null)
      {
        var accessibleBurrows = modBunburrows.Where(b => b.Model.HasSurfaceEntry && b.Model.Depth > 0).ToDictionary(b => b.Name);

        world.GeneratedSurfaceLevels = GenerateLevels(precedingLevel, world, accessibleBurrows).ToList();
      }
    }

    private static IEnumerable<LevelObject> GenerateLevels(LevelObject precedingLevel, CustomWorld world, Dictionary<string, BNYSModBunburrow> enterableBurrows)
    {
      foreach (var surfaceEntry in world.SurfaceEntries ?? Enumerable.Empty<SurfaceEntry>())
      {
        string content = null;
        List<BNYSModBunburrow> consumedBurrows = null;

        switch (GetSurfaceType(surfaceEntry))
        {
          case "COORDINATES":
            (content, consumedBurrows) = GenerateCoordinatesSurfaceContent(surfaceEntry.Coordinates, enterableBurrows);
            break;
        }

        if (consumedBurrows != null && !string.IsNullOrEmpty(content) && consumedBurrows.Count > 0)
        {
          precedingLevel = GenerateLevel(world, content, precedingLevel);

          foreach (var consumedBurrow in consumedBurrows)
            consumedBurrow.SurfaceLevel = precedingLevel;

          yield return precedingLevel;
        }
      }

      while (enterableBurrows.Any())
      {
        var (content, consumedBurrows) = GenerateDefaultSurfaceLevel(enterableBurrows);
        precedingLevel = GenerateLevel(world, content, precedingLevel);

        foreach (var consumedBurrow in consumedBurrows)
          consumedBurrow.SurfaceLevel = precedingLevel;

        yield return precedingLevel;
      }
    }

    private static LevelObject GenerateLevel(CustomWorld world, string levelContent, LevelObject previousLevel)
    {
      var level = ScriptableObject.CreateInstance<ModLevelObject>();
      level.name = "SurfaceRight BNYS";
      level.Content = levelContent;
      level.IsSurface = true;
      level.CustomNameKey = world.Title;
      level.SpecificBackground = SurfaceBurrowsPatch.ExtendedBackground;

      //Set empty / defaults
      level.BunburrowStyle = previousLevel.BunburrowStyle;
      level.SideLevels.SetPart(Direction.Left, previousLevel);

      //link up with previous
      if (previousLevel != null)
      {
        previousLevel.SideLevels.SetPart(Direction.Right, level);
      }
      return level;
    }

    private static (string, List<BNYSModBunburrow>) GenerateCoordinatesSurfaceContent(Dictionary<string, int[]> coordinates, Dictionary<string, BNYSModBunburrow> bunburrows)
    {
      string[][] content = GetEmptyLevelContent();
      var consumedBurrows = new List<BNYSModBunburrow>();

      foreach (var coordinate in coordinates)
      {
        if (bunburrows.TryGetValue(coordinate.Key, out BNYSModBunburrow bunburrow))
        {
          if (coordinate.Value != null && coordinate.Value.Length > 0)
          {
            int x = coordinate.Value[0];
            int y = 3;
            if (coordinate.Value.Length > 1)
              y = coordinate.Value[1];

            //its split by rows first - y is first
            content[y][x] = "N" + bunburrow.ID;

            // Rip burrow out of the available list
            consumedBurrows.Add(bunburrow);
            bunburrows.Remove(coordinate.Key);
          }
        }
      }

      return (string.Join(",", content.Select(row => string.Join(",", row)).ToArray()), consumedBurrows);
    }

    private static (string, List<BNYSModBunburrow>) GenerateDefaultSurfaceLevel(Dictionary<string, BNYSModBunburrow> bunburrows)
    {
      var consumedBurrows = new List<BNYSModBunburrow>();

      var first = bunburrows.FirstOrDefault().Value;
      if (first != null)
      {
        bunburrows.Remove(first.Name);
        consumedBurrows.Add(first);
      }

      var second = bunburrows.FirstOrDefault().Value;
      if (second != null)
      {
        bunburrows.Remove(second.Name);
        consumedBurrows.Add(second);
      }

      var third = bunburrows.FirstOrDefault().Value;
      if (third != null)
      {
        bunburrows.Remove(third.Name);
        consumedBurrows.Add(third);
      }

      return (GetBasicLevelContent(first, second, third), consumedBurrows);
    }

    private static string[][] GetEmptyLevelContent()
    {
      string[] rows =
      {
        WallRow,
        TopRow,
        SpaceRow,
        string.Format(EntryRow, "T", "T", "T"),
        OpenRow,
        OpenRow,
        OpenRow,
        WallRow,
        WallRow
      };
      return rows.Select(r => r.Split(',')).ToArray();
    }

    private static string GetBasicLevelContent(BNYSModBunburrow first, BNYSModBunburrow second, BNYSModBunburrow third)
    {
      string[] rows =
      {
        WallRow,
        TopRow,
        SpaceRow,
        string.Format(EntryRow, GetLevelEntryCode(first), GetLevelEntryCode(second), GetLevelEntryCode(third)),
        OpenRow,
        OpenRow,
        OpenRow,
        WallRow,
        WallRow
      };
      return string.Join(System.Environment.NewLine, rows);
    }

    private static string GetLevelEntryCode(BNYSModBunburrow burrow)
    {
      if (burrow != null)
      {
        return "N" + burrow.ID;
      }
      return "T";
    }

    private static string GetSurfaceType(SurfaceEntry surfaceEntry)
    {
      // For more surface types?
      if (surfaceEntry.Coordinates != null)
      {
        return "COORDINATES";
      }
      return null;
    }
  }
}
