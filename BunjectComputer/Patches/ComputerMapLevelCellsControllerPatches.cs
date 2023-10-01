using Bunburrows;
using Characters.Bunny.Data;
using Computer;
using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;
using Tiling.Behaviour;

namespace Bunject.Computer.Patches.ComputerMapLevelCellsControllerPatches
{
	[HarmonyPatch(typeof(ComputerMapLevelCellsController), nameof(ComputerMapLevelCellsController.DrawMap))]
	internal class DrawMapPatch
	{
		private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
		{
			int state = 0;
			var type = typeof(ExitTile);
			var constructor = AccessTools.Constructor(typeof(BunnyIdentity), new Type[] { typeof(Bunburrows.Bunburrow), typeof(int), typeof(int), typeof(int) });
			foreach (var code in instructions)
			{
				yield return code;
				if (state == 0)
				{
					if (code.Is(OpCodes.Isinst, type))
						state = 1;
				}
				else if (state == 1)
				{
					yield return new CodeInstruction(OpCodes.Ldloc_S, 7);
					yield return new CodeInstruction(OpCodes.Isinst, typeof(BunburrowEntryTile));
					yield return code;
					state = 2;
				}
				else if (state == 2)
				{
					if (code.opcode == OpCodes.Call && code.operand != null && code.operand.Equals(constructor))
					{
						yield return new CodeInstruction(OpCodes.Ldloc_S, 18);
						yield return new CodeInstruction(OpCodes.Call, AccessTools.Method(typeof(DrawMapPatch), nameof(FixIdentity)));
						yield return new CodeInstruction(OpCodes.Stloc_S, 18);
					}
				}
			}
		}
		private static BunnyIdentity FixIdentity(BunnyIdentity original)
		{
			return new BunnyIdentity(original.Bunburrow, original.InitialDepth, original.LevelID,
				AssetsManager.LevelsLists[original.Bunburrow.ToBunburrowName()][original.InitialDepth].BunburrowStyle.SpriteSheetsID);
		}
	}
}
