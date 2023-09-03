using Bunject.NewYardSystem.Resources;
using HarmonyLib;
using Levels;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Bunject.NewYardSystem.Levels
{
  internal class SurfaceBurrowsPatch
  {
    private const string PatchedTexture = "PatchedTexture";
    private const string PatchedTexturePath = "ExtendedRegion.png";

    public static Sprite EndingBackground = null;
    public static Sprite ExtendedBackground = null;

    private static string lastContentPatch = null;
    public static void PatchSurfaceBurrows(LevelObject original, LevelObject newRightExit)
    {
      if (lastContentPatch == null)
      {
        lastContentPatch = GeneratePatchedContent(original.Content);

        EndingBackground = original.SpecificBackground;
        ExtendedBackground = ImportImage.ImportSprite(PatchedTexture, Path.Combine(BNYSPlugin.rootDirectory, PatchedTexturePath), new Vector2(0.0f, 1.0f), 16);

        PatchLevel(original, lastContentPatch, ExtendedBackground);
      }
      else
      {
        if (original.Content != lastContentPatch)
        {
          PatchLevel(original, lastContentPatch, ExtendedBackground);
        }
      }
    }

    private static string GeneratePatchedContent(string content)
    {
      var rows = content.Split(new string[] { "\r\n", "\r", "\n" }, StringSplitOptions.None);
      for (var i = 4; i < 7; i++)
      {
        var cells = rows[i].Split(',');
        cells[12] = "T";
        cells[13] = "T";
        cells[14] = "T";
        rows[i] = string.Join(",", cells);
      }
      return string.Join(System.Environment.NewLine, rows);
    }

    private static void PatchLevel(LevelObject level, string content, Sprite replacementSprite)
    {
      var traverse = Traverse.Create(level);
      traverse.Field("content").SetValue(content);
      traverse.Field("specificBackground").SetValue(replacementSprite);
    }
  }
}
