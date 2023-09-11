using Bunject.Tiling;
using HarmonyLib;
using Levels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Bunject.Patches.LevelLoaderPatches
{
  [HarmonyPatch(typeof(LevelLoader), "LoadLevel")]
  internal class LoadLevelPatch
  {

    /*
    private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator il)
    {
      var regex_IsMatch = AccessTools.Method(typeof(Regex), nameof(Regex.IsMatch), new Type[] { typeof(string) });
      var codes = new List<CodeInstruction>(instructions);
      for (int i = 0; i < codes.Count; i++)
      {
        if (codes[i].Calls(regex_IsMatch))
        {
          if (codes[++i].Branches(out var l) && l is Label elseBlock)
          {
            Label ifBlock = il.DefineLabel();
            codes.Insert(i + 1, new CodeInstruction(OpCodes.Nop).WithLabels(ifBlock));
            codes.InsertRange(i, new List<CodeInstruction> {
              new CodeInstruction(OpCodes.Brtrue, ifBlock),
              new CodeInstruction(OpCodes.Ldarg_0),
              new CodeInstruction(OpCodes.Ldloc, 4),
              new CodeInstruction(OpCodes.Call, AccessTools.Method(typeof(LoadLevelPatch), nameof(LoadLevelPatch.CheckIfModSupportsTile))),
            });
            break;
          }
        }
      }
      return codes;
    }*/

    private static bool Prefix(LevelObject levelObject, out List<string> __result)
    {
      var tiles = TileValidator.GetTilesFromContent(levelObject.Content);

      foreach (var tile in tiles)
      {
        if (!TileValidator.ValidateTile(tile))
          UnityEngine.Debug.LogWarning("Invalid tile string: " + tile);
      }

      __result = tiles;

      return false;
    }
  }
}
