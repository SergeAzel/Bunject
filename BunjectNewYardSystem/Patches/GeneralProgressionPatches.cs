using Achievements;
using HarmonyLib;
using Misc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace Bunject.NewYardSystem.GeneralProgressionPatches
{
	[HarmonyPatch(typeof(GeneralProgression), nameof(GeneralProgression.HandleElevatorUnlock))]
	internal class HandleElevatorUnlockPatch
	{
		public static void Postfix(GeneralProgression __instance)
		{
			var identity = GameManager.LevelStates.CurrentLevelState.LevelIdentity;
			if (ModElevatorController.Instance.TryGetElevator(identity, out var elevator))
			{
				if (!string.IsNullOrWhiteSpace(elevator) && !__instance.UnlockedElevators.ContainsEquatable(elevator))
				{
					Traverse.Create(__instance).Field<List<string>>("unlockedElevators").Value.Add(elevator);
					AchievementsManager.HandleElevatorUnlock();
				}
			}
		}
	}
}