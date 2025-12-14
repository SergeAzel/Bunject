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
        ExtendedBackground = ImportImage.ImportSprite(PatchedTexture, Path.Combine(BNYSPlugin.rootDirectory, PatchedTexturePath), new Vector2(0.0f, 1.0f), 16);
    }
  }
}
