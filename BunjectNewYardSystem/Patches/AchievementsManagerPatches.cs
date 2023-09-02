using Achievements;
using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace Bunject.NewYardSystem.Patches.AchievementsManagerPatches
{
	[HarmonyPatch(typeof(AchievementsManager), nameof(AchievementsManager.HandleElevatorUnlock))]
	internal class HandleElevatorUnlockPatch
	{
		private static void Postfix()
		{
			if (ElevatorController.FloorsDictionary.Keys.All(x => Enumerable.Contains(GameManager.GeneralProgression.UnlockedElevators, x)))
			{
				GameManager.GeneralProgression.HandleAchievementUnlock(Achievement.ALL_ELEVATORS_USED);
			}
		}
	}
}