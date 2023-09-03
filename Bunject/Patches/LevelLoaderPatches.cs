using HarmonyLib;
using Levels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Bunject.Patches.LevelLoaderPatches
{
  [HarmonyPatch(typeof(LevelLoader), "LoadLevel")]
  internal class LoadLevelPatch
  {
    //Same as core valid tiles string, but permits entrances (N#) with longer numbers
    private const string VALID_TILES = "^(S(?:{K})?|B(?:{[KB]*})?|E|F|D[0-9]|P[0-9]|N[0-9]+|\\!|Oph|X|C|T(?:{[URLDCTKB]+})?|W(?:{[URLD]*[0-9]?})?|R(?:{[URLD]*[0-9]?})?|EW|ER|PU|A|Y(?:{K})?)$";

    private static readonly Regex overrideRegex = new Regex(VALID_TILES);

    private static readonly string[] Separators = new string[4]
    {
      ",",
      "\r\n",
      "\r",
      "\n"
    };

    private static bool Prefix(LevelObject levelObject, out List<string> __result)
    {
      var contents = levelObject.Content.Split(Separators, StringSplitOptions.RemoveEmptyEntries).ToList();

      foreach (var input in contents)
      {
        if (!overrideRegex.IsMatch(input))
          UnityEngine.Debug.LogWarning("Invalid tile string: " + input);
      }

      __result = contents;

      return false;
    }
  }
}
