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
    private const string VALID_BASE_LEVEL_TILES_FOR_CUSTOM = "^(S(?:{K})?|B(?:{[KB]*})?|E|\\!|T(?:{[URLDCTKB]+})?|W(?:{[URLD]*[0-9]?})?|R(?:{[URLD]*[0-9]?})?|EW|ER)$";
    private const string VALID_CUSTOM_LEVEL_TILES_FOR_SURFACE = "N[0-9]+";

    private static readonly Regex customLevelLimitRegex = new Regex(VALID_BASE_LEVEL_TILES_FOR_CUSTOM);

    private static readonly Regex surfaceLevelRegex = new Regex(VALID_CUSTOM_LEVEL_TILES_FOR_SURFACE);

    private static readonly string[] Separators = Traverse.Create(typeof(LevelLoader)).Field<string[]>("Separators").Value;
    public static bool ValidateBaseTile(LevelObject levelObject, string tile)
    {
      var matched = !(levelObject is CustomLevelObject || levelObject is SurfaceLevelObject) || customLevelLimitRegex.IsMatch(tile);
      //Debug.Log(levelObject.name + " good for base? " + matched);
      return matched;
    }
    public static bool ValidateModTile(LevelObject levelObject, string tile)
    {
      bool matched = levelObject is SurfaceLevelObject && surfaceLevelRegex.IsMatch(tile);
      //Debug.Log(levelObject.name + " good for mod? " + matched);
      return matched;
    }
    public static bool ValidateLevelObject(LevelObject levelObject, string content)
    {
      return ValidateLevelObject(Traverse.Create(levelObject), content);
    }
    public static bool ValidateLevelObject(Traverse levelObject, string content)
    {
      return LevelLoader.LoadLevel(levelObject.GetValue<LevelObject>()).SequenceEqual(content.Split(Separators, StringSplitOptions.RemoveEmptyEntries));
    }
  }
}
