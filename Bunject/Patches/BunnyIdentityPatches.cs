using Bunject.Internal;
using Bunject.Levels;
using Characters.Bunny.Data;
using HarmonyLib;
using Levels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bunject.Patches.BunnyIdentityPatches
{
	[HarmonyPatch(typeof(BunnyIdentity), nameof(BunnyIdentity.CompareTo))]
	internal class CompareToPatch
	{
		private static int Postfix(int __result, BunnyIdentity __instance, BunnyIdentity other)
		{
			if (!(__instance.Bunburrow.GetModBunburrow() is IModBunburrow @this)
				|| !(other.Bunburrow.GetModBunburrow() is IModBunburrow o))
				return __result;
			int res = @this.CompareTo(o);
			return res != 0 ? res : __result;
			/*
			if (res < 0)
			{
				return -1;
			}
			if (res > 0)
			{
				return 1;
			}
			if (__instance.InitialDepth < other.InitialDepth)
			{
				return -1;
			}
			if (__instance.InitialDepth > other.InitialDepth)
			{
				return 1;
			}
			if (__instance.LevelID < other.LevelID)
			{
				return -1;
			}
			if (__instance.LevelID <= other.LevelID)
			{
				return 0;
			}
			return 1;
			*/
		}
	}
}
