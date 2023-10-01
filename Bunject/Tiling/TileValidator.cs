using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Bunject.Tiling
{
  public static class TileValidator
  {
    private const string VALID_TILES = "^(S(?:{K})?|B(?:{[KB]*})?|E|F|D[0-9]|P[0-9]|N[0-9]+|\\!|Oph|X|C|T(?:{[URLDCTKB]+})?|W(?:{[URLD]*[0-9]?})?|R(?:{[URLD]*[0-9]?})?|EW|ER|PU|A|Y(?:{K})?)$";
    private static readonly Regex ValidTileRegex = new Regex(VALID_TILES);

    private static readonly string[] Separators = new string[4]
    {
      ",",
      "\r\n",
      "\r",
      "\n"
    };

    public static List<string> GetTilesFromContent(string content)
    {
      return content.Split(Separators, StringSplitOptions.RemoveEmptyEntries).ToList();
    }

    public static bool ValidateTiles(string content)
    {
      return ValidateTiles(GetTilesFromContent(content));
    }

    public static bool ValidateTiles(IEnumerable<string> tiles)
    {
      var result = true;
      foreach (var tile in tiles)
      {
        result &= ValidateTiles(tile);
      }
      return result;
    }

    public static bool ValidateTile(string tile)
    {
      return ValidTileRegex.IsMatch(tile)
        || BunjectAPI.Forward.SupportsTile(tile);
    }
  }
}
