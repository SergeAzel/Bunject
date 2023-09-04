using Bunburrows;
using HarmonyLib;
using Levels;
using Misc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;
using Tiling.Behaviour;
using UnityEngine;

namespace Bunject.Patches.LevelBuilderPatches
{
  [HarmonyPatch]
  internal class BuildNewLevelPatch
  {
    static MethodInfo TargetMethod()
    {
      return typeof(LevelBuilder).GetMethod("BuildNewLevel", new Type[] { typeof(LevelObject), typeof(BunburrowStyle), typeof(bool) });
    }

    static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
    {
      // Goal of transpiler is to replace a Chars[1] with a Substring(1), so that it parses the full index of our target bunburrow
      // Not just the first character
      MethodInfo startsWith = typeof(String).GetMethod("StartsWith", new Type[] { typeof(string) });
      MethodInfo get_Chars = typeof(String).GetProperty("Chars").GetGetMethod();
      MethodInfo char_ToString = typeof(Char).GetMethod("ToString", new Type[] { } );

      int detectionStage = 0;
      foreach (var instructionIterate in instructions)
      {
        var instruction = instructionIterate;

        switch (detectionStage)
        {
          case 0:
            if (instruction.opcode == OpCodes.Ldstr && (string)instruction.operand == "N")
            {
              //Found our N
              detectionStage = 1;
            }
            break;
          case 1:
            if (instruction.Calls(startsWith))
            {
              // Found starts with - now we're committed, no resetting from here
              detectionStage = 2;
            }
            else
            {
              // reset
              detectionStage = 0;
            }
            break;
          case 2:
            if (instruction.Calls(get_Chars))
            {
              // Found getChars which we wish to replace
              detectionStage = 3;
              // Replace function call
              instruction = CodeInstruction.Call(typeof(String), "Substring", new Type[] { typeof(int) });
            }
            break;
          case 3:
            // skip until we find ToString - which we also skip, but then continue processing the rest of the IL
            if (instruction.Calls(char_ToString))
            {
              // From here on, we move on as normal
              detectionStage = 4;
            }
            continue;
        }

        yield return instruction;
      }
    }
  }
  [HarmonyPatch(typeof(LevelBuilder), "BuildNewLevel", argumentTypes: new Type[] { typeof(LevelObject), typeof(BunburrowStyle), typeof(bool) })]
  internal class BuildNewLevelPatch_CustomTiles
  {
    private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator il)
    {
      List<CodeInstruction> codes = new List<CodeInstruction>(instructions);

      // this entire process is kind of convoluted

      // find first if condition
      var string_StartsWith = AccessTools.Method(typeof(string), nameof(string.StartsWith), new Type[] { typeof(string) });
      int firstIf = codes.FindIndex(x => x.Is(OpCodes.Ldstr, "D")) - 1;
      if (firstIf < 0) return codes;

      // find second if condition
      int secondIf = -1;
      for (int i = firstIf; i < codes.Count; i++)
      {
        if (codes[i].Branches(out var l) && l is Label l1)
        {
          secondIf = codes.FindIndex(x => x.labels.Contains(l1));
          break;
        }
      }

      // backtrack to find operation that jumps at the end of the if-else chain
      for (int i = secondIf; i > firstIf; i--)
      {
        if (codes[i].opcode == OpCodes.Br && codes[i].operand is Label endOfIfElses)
        {
          Label firstIfLabel = il.DefineLabel();
          codes.InsertRange(firstIf, new List<CodeInstruction>
          {
            new CodeInstruction(OpCodes.Ldloc, 16), // tile
            new CodeInstruction(OpCodes.Ldloc, 17), // position
            new CodeInstruction(OpCodes.Ldloca, 18), // tileData
            CodeInstruction.Call(typeof(BuildNewLevelPatch_CustomTiles), nameof(BuildNewLevelPatch_CustomTiles.TryGetTile)),
            new CodeInstruction(OpCodes.Brfalse, firstIfLabel),
            new CodeInstruction(OpCodes.Br, endOfIfElses),
            new CodeInstruction(OpCodes.Nop).WithLabels(firstIfLabel)
          });
          break;
        }
			}
      return codes;
    }
    private static bool TryGetTile(string tile, Vector3Int position, out TileLevelData tileData)
		{
      tileData = BunjectAPI.Forward.LoadTile(tile, position.ToVector2Int());
      return tileData != null;
    }
  }
  [HarmonyPatch(typeof(LevelBuilder), nameof(LevelBuilder.UpdateTilesGraphics))]
  internal class UpdateTilesGraphicsPatch
  {
    private static void Postfix(BunburrowStyle bunburrowStyle, IReadOnlyList<TileLevelData> tiles)
    {
      foreach (var tile in tiles)
      {
        BunjectAPI.Forward.UpdateTileSprite(tile, bunburrowStyle);
      }
    }
  }
}
