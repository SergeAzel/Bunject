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
    private const string RESTRICTED_TILES = @"^(F|D[0-9]+|P[0-9]+|N[0-9]+|Oph|X|C|PU|A|Y(?:{.*})?)|T\{\.*[PS].*\}$";
    private static readonly Regex restrictedTileRegex = new Regex(RESTRICTED_TILES);

    public static List<LevelValidationError> ValidateLevelContent(string content)
    {
      var tiles = TileValidator.GetTilesFromContent(content);
      var validationErrors = new List<LevelValidationError>();

      if (tiles.Count > 15 * 9)
        validationErrors.Add(new LevelValidationError("Level has too many tiles", true));

      if (tiles.Count < 15 * 9)
        validationErrors.Add(new LevelValidationError("Level has too few tiles"));

      foreach (var (tile, index) in tiles.Select((t, i) => ( t, i )))
      {
        if (restrictedTileRegex.IsMatch(tile) || !TileValidator.ValidateTile(tile))
        {
          validationErrors.Add(new TileValidationError($"Tile '{tile}' is not valid", index / 15, index % 15));
        }
      }

      return validationErrors;
    }
  }
}
