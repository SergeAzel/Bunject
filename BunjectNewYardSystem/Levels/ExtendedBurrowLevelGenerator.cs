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
        var accessibleBurrows = modBunburrows.Where(b => b.Model.HasSurfaceEntry && b.Model.Depth > 0).ToList();

        world.GeneratedSurfaceLevels = GenerateLevels(precedingLevel, world, accessibleBurrows).ToList();
      }
    }

    private static IEnumerable<LevelObject> GenerateLevels(LevelObject precedingLevel, CustomWorld world, IEnumerable<BNYSModBunburrow> entries)
    {
      while (entries.Any())
      {
        var first = GetNext(ref entries);
        var second = GetNext(ref entries);
        var third = GetNext(ref entries);

        precedingLevel = GenerateLevel(world, FormatLevelString(first, second, third), precedingLevel);

        if (first != null)
          first.SurfaceLevel = precedingLevel;

        if (second != null)
          second.SurfaceLevel = precedingLevel;

        if (third != null)
          third.SurfaceLevel = precedingLevel;

        yield return precedingLevel;
      }
    }

    private static BNYSModBunburrow GetNext(ref IEnumerable<BNYSModBunburrow> entries)
    {
      var result = entries.FirstOrDefault();
      if (result != null)
      {
        entries = entries.Skip(1);
      }
      return result;
    }

    private static string GetLevelEntryCode(BNYSModBunburrow burrow)
    {
      if (burrow != null)
      {
        return "N" + burrow.ID;
      }
      return "T";
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

    private static string FormatLevelString(BNYSModBunburrow first, BNYSModBunburrow second, BNYSModBunburrow third)
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
  }
}
