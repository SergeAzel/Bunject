using Levels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Bunject.NewYardSystem.Levels
{
  internal class ContentValidator
  {
    private const string VALID_CUSTOM_LEVEL_TILES = "^(S(?:{K})?|B(?:{[KB]*})?|E|\\!|T(?:{[URLDCTKB]+})?|W(?:{[URLD]*[0-9]?})?|R(?:{[URLD]*[0-9]?})?|EW|ER)$";

    private static readonly Regex customLevelRegex = new Regex(VALID_CUSTOM_LEVEL_TILES);

    private static readonly string[] Separators = new string[4]
    {
    ",",
    "\r\n",
    "\r",
    "\n"
    };

    public static bool ValidateContentTiles(string content)
    {
      bool valid = true;
      var contents = content.Split(Separators, StringSplitOptions.RemoveEmptyEntries);

      foreach (var input in contents)
      {
        if (!customLevelRegex.IsMatch(input))
        {
          UnityEngine.Debug.LogWarning("Invalid tile string: " + input);
          valid = false;
        }
      }

      return valid;
    }
  }
}
