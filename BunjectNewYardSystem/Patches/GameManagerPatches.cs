using HarmonyLib;
using Levels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;

namespace Bunject.NewYardSystem.Patches.GameManagerPatches
{
	[HarmonyPatch(typeof(GameManager), nameof(GameManager.HandleSurfaceElevatorUse))]
	internal class HandleSurfaceElevatorUsePatch
	{
		private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator il)
		{
			var codes = new List<CodeInstruction>(instructions);
			var TryGetValue = AccessTools.Method(typeof(IReadOnlyDictionary<string, LevelIdentity>), nameof(IReadOnlyDictionary<string, LevelIdentity>.TryGetValue));
			var index = codes.FindIndex(x => x.Calls(TryGetValue));
			if (index >= 0 && index + 2 < codes.Count && codes[index + 1].Branches(out _))
			{
				Label l = il.DefineLabel();
				codes.Insert(index + 2, new CodeInstruction(OpCodes.Nop).WithLabels(l));
				codes.InsertRange(index + 1, new List<CodeInstruction>()
				{
					new CodeInstruction(OpCodes.Brtrue, l),
					new CodeInstruction(OpCodes.Ldarg_0),
					new CodeInstruction(OpCodes.Ldloca, 0),
					new CodeInstruction(OpCodes.Call, AccessTools.Method(typeof(HandleSurfaceElevatorUsePatch), nameof(HandleSurfaceElevatorUsePatch.Infix))),
				});
			}
			return codes;
		}
		private static bool Infix(string elevatorName, out LevelIdentity levelIdentity)
		{
			return ModElevatorController.Instance.TryGetLevel(elevatorName, out levelIdentity);
		}
	}
}