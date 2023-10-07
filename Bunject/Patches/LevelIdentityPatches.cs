using Bunject.Internal;
using Bunject.Levels;
using HarmonyLib;
using Levels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bunject.Patches.LevelIdentityPatches
{
	[HarmonyPatch(typeof(LevelIdentity), nameof(LevelIdentity.CompareTo))]
	internal class CompareToPatch
	{
		private static int Postfix(int __result, LevelIdentity __instance, LevelIdentity other)
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
			if (__instance.Depth < other.Depth)
			{
				return -1;
			}
			if (__instance.Depth <= other.Depth)
			{
				return 0;
			}
			return 1;
			*/
		}
	}
}
