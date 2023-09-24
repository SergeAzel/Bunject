using Bunject.Tiling;
using HarmonyLib;
using Levels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using UnityEngine;

namespace Bunject.NewYardSystem.Levels
{
  internal class ContentValidator
  {
    /* Tiles restricted from use in BNYS custom levels:
     *   Oph - Opheline tile
     *   D# - Dialogue prompting or interactable tile
     *   A - Hole at the bottom of C-27
     *   PU - Power Up in room C-13
     *   N# - Burrow Entrance
     *   P# - C-27 Pillar
     *   C - Cage in shop?
     *   X - Cage in shop?
     *   F - Fake bunny at start of game
     *   Y - SPOILERS
     */
    private const string RESTRICTED_TILES = "^(F|D[0-9]+|P[0-9]+|N[0-9]+|Oph|X|C|PU|A|Y(?:{.*})?)$";
    private static readonly Regex restrictedTileRegex = new Regex(RESTRICTED_TILES);

    public static bool ValidateLevelContent(string content)
    {
      var tiles = TileValidator.GetTilesFromContent(content);
      bool result = true;
      foreach (var tile in tiles)
      {
        result &= (!restrictedTileRegex.IsMatch(tile) && TileValidator.ValidateTile(tile));
      }
      return result;
    }
  }
}
