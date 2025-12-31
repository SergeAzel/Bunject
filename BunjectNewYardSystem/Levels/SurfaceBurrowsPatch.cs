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

    public static void PatchSurfaceBurrows(LevelObject shop, LevelObject originalBurrows, LevelObject newRightExit)
    {
      if (EndingBackground == null)
        EndingBackground = originalBurrows.SpecificBackground;

      if (ExtendedBackground == null)
      {
        // Directory used by thunderstore variant
        var filePath = Path.Combine(BNYSPlugin.rootDirectory, PatchedTexturePath);

        // If the file doesnt exist there, try in the original location variant
        if (!File.Exists(filePath))
          filePath = Path.Combine(BNYSPlugin.inlineDirectory, PatchedTexturePath);

        ExtendedBackground = ImportImage.ImportSprite(PatchedTexture, filePath, new Vector2(0.0f, 1.0f), 16);
      }
    }
  }
}
