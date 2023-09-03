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
  internal class ExtendedBurrowLevelGenerator
  {
    public const string WallRow = "W,W,W,W,W,W,W,W,W,W,W,W,W,W,W";
    public const string TopRow = "W,T,T,T,T,T,T,T,T,T,T,T,W{0},W,W";
    public const string SpaceRow = "W,W{0},T,T,T,T,T,T,T,T,T,T,T,W,W";
    public const string EntryRow = "W,T,T,T,{0},T,T,{1},T,T,{2},T,T,W,W";
    public const string OpenRow = "T,T,T,T,T,T,T,T,T,T,T,T,T,T,T";

    //Should only be run after burrows are registered.  Burrow ID is required for generation.
    public static void CreateSurfaceLevels(CustomWorld world, LevelObject precedingLevel)
    {
      if (world.GeneratedSurfaceLevels == null)
      {
        var accessibleBurrows = world.Burrows.Where(b => b.HasSurfaceEntry && b.Depth > 0).ToList();

        world.GeneratedSurfaceLevels = GenerateLevels(precedingLevel, world, accessibleBurrows).ToList();
      }
    }

    private static IEnumerable<LevelObject> GenerateLevels(LevelObject precedingLevel, CustomWorld world, IEnumerable<Burrow> entries)
    {
      while (entries.Any())
      {
        var first = GetNext(ref entries);
        var second = GetNext(ref entries);
        var third = GetNext(ref entries);

        precedingLevel = GenerateLevel(world, FormatLevelString(first, second, third), precedingLevel);
        yield return precedingLevel;
      }
    }

    private static Burrow GetNext(ref IEnumerable<Burrow> entries)
    {
      var result = entries.FirstOrDefault();
      if (result != null)
      {
        entries = entries.Skip(1);
      }
      return result;
    }

    private static string GetLevelEntryCode(Burrow burrow)
    {
      if (burrow != null)
      {
        return "N" + burrow.ID;
      }
      return "T";
    }

    private static LevelObject GenerateLevel(CustomWorld world, string levelContent, LevelObject previousLevel)
    {
      var levelObject = ScriptableObject.CreateInstance<LevelObject>();
      var level = Traverse.Create(levelObject);
      level.Field("content").SetValue(levelContent);
      level.Field("isSurface").SetValue(true);
      level.Field("customNameKey").SetValue(world.Title);
      level.Field("specificBackground").SetValue(SurfaceBurrowsPatch.ExtendedBackground);

      //Set empty / defaults
      level.Field("dialogues").SetValue(new List<DialogueObject>());
      level.Field("contextualDialogues").SetValue(new List<ContextualDialogueInfo>());

      level.Field("bunburrowStyle").SetValue(previousLevel.BunburrowStyle);
      level.Field("sideLevels").SetValue(new DirectionsListOf<LevelObject>(previousLevel, null, null, null));

      //link up with previous
      if (previousLevel != null)
      {
        previousLevel.SideLevels.SetPart(Direction.Right, levelObject);
      }
      return levelObject;
    }

    private static string FormatLevelString(Burrow first, Burrow second, Burrow third)
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
