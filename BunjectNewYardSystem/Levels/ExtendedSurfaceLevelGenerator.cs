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
  // TODO - Refactor how we built out our content strings?  The main structure of this is fine now, but these five constants still feel gross.
  internal class ExtendedSurfaceLevelGenerator
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
        var accessibleBurrows = modBunburrows.Where(b => b.Model.Depth > 0).ToDictionary(b => b.Name);

        world.GeneratedSurfaceLevels = GenerateLevels(precedingLevel, world, accessibleBurrows).ToList();
      }
    }

    private static IEnumerable<LevelObject> GenerateLevels(LevelObject precedingLevel, CustomWorld world, Dictionary<string, BNYSModBunburrow> burrows)
    {
      foreach (var surfaceEntry in world.SurfaceEntries ?? Enumerable.Empty<SurfaceEntry>())
      {
        string content = null;
        List<BNYSModBunburrow> consumedBurrows = null;

        switch (GetSurfaceType(surfaceEntry))
        {
          case SurfaceType.Coordinates:
            (content, consumedBurrows) = GenerateCoordinatesSurfaceContent(surfaceEntry.Coordinates, burrows);
            break;

          case SurfaceType.Grid:
            (content, consumedBurrows) = GenerateGridSurfaceContent(surfaceEntry.Grid, burrows);
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

      var enterableBurrows = burrows.Values.Where(b => b.Model.HasSurfaceEntry).ToList();
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

    private static (string, List<BNYSModBunburrow>) GenerateCoordinatesSurfaceContent(Dictionary<string, SurfaceCoordinate> coordinates, Dictionary<string, BNYSModBunburrow> bunburrows)
    {
      string[][] content = GetEmptyLevelContent();
      var consumedBurrows = new List<BNYSModBunburrow>();

      foreach (var coordinate in coordinates)
      {
        if (bunburrows.TryGetValue(coordinate.Key, out BNYSModBunburrow bunburrow))
        {
          if (coordinate.Value?.Hole != null && coordinate.Value.Hole.Length > 0)
          {
            int x = coordinate.Value.Hole[0];
            int y = 3;
            if (coordinate.Value.Hole.Length > 1)
              y = coordinate.Value.Hole[1];

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

    private static (string, List<BNYSModBunburrow>) GenerateGridSurfaceContent(SurfaceEntryGrid grid, Dictionary<string, BNYSModBunburrow> bunburrows)
    {
      // Cheating.. just convert grid to coordinates.
      var coordinates = new Dictionary<string, SurfaceCoordinate>();

      Action<string, SurfaceCoordinate> AddCoordinate = (string burrowName, SurfaceCoordinate coordinate) =>
      {
        if (!string.IsNullOrEmpty(burrowName))
          coordinates.Add(burrowName, coordinate);
      };

      AddCoordinate(grid.NW, new SurfaceCoordinate(4, 1));
      AddCoordinate(grid.N, new SurfaceCoordinate(7, 1));
      AddCoordinate(grid.NE, new SurfaceCoordinate(10, 1));
      AddCoordinate(grid.W, new SurfaceCoordinate(4, 3));
      AddCoordinate(grid.C, new SurfaceCoordinate(7, 3));
      AddCoordinate(grid.E, new SurfaceCoordinate(10, 3));
      AddCoordinate(grid.SW, new SurfaceCoordinate(4, 5));
      AddCoordinate(grid.S, new SurfaceCoordinate(7, 5));
      AddCoordinate(grid.SE, new SurfaceCoordinate(10, 5));

      return GenerateCoordinatesSurfaceContent(coordinates, bunburrows);
    }

    private static (string, List<BNYSModBunburrow>) GenerateDefaultSurfaceLevel(List<BNYSModBunburrow> bunburrows)
    {
      var consumedBurrows = new List<BNYSModBunburrow>();

      var first = bunburrows.FirstOrDefault();
      if (first != null)
      {
        bunburrows.Remove(first);
        consumedBurrows.Add(first);
      }

      var second = bunburrows.FirstOrDefault();
      if (second != null)
      {
        bunburrows.Remove(second);
        consumedBurrows.Add(second);
      }

      var third = bunburrows.FirstOrDefault();
      if (third != null)
      {
        bunburrows.Remove(third);
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

    private static SurfaceType GetSurfaceType(SurfaceEntry surfaceEntry)
    {
      var result = SurfaceType.None;
      var counter = 0;

      if (surfaceEntry.Coordinates != null)
      {
        result |= SurfaceType.Coordinates;
        counter++;
      }

      if (surfaceEntry.Grid != null)
      {
        result |= SurfaceType.Grid;
        counter++;
      }

      if (counter > 1)
      {
        var surfaceTypes = new List<string>();
        foreach (var match in Enum.GetValues(typeof(SurfaceType)).Cast<SurfaceType>())
        {
          if (result.HasFlag(match) && match != SurfaceType.None)
            surfaceTypes.Add(Enum.GetName(typeof(SurfaceType), match));
        }

        throw new Exception("SurfaceEntry cannot have both a Coordinates and Grid - please select only one.");
      }

      return result;
    }

    private static int CountFlags<T>(T instance) where T : System.Enum
    {
      var result = 0;

      foreach (var flag in Enum.GetValues(typeof(T)).Cast<T>())
      {
        if (instance.HasFlag(flag))
          result++;
      }

      return result;
    }

    [Flags]
    private enum SurfaceType
    {
      None = 0,
      Coordinates = 1,
      Grid = 2
      //NextItem = 4, flag based enum
    }
  }
}
