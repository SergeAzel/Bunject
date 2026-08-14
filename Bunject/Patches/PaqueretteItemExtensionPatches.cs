using System;
using Bunject.Levels;
using Characters;
using HarmonyLib;
using Items;
using Levels;

namespace Bunject.Patches.PaqueretteItemExtensionPatches
{
    [HarmonyPatch(typeof(PaqueretteItemExtension), "IsAvailableInContext")]
    internal class IsAvailableInContextPatch
    {
        static bool Prefix(ref bool __result, Item item)
        {
            if ((item == Item.Talk)
                && (GameManager.LevelStates.CurrentLevelState.LevelIdentity.Depth != 0)
                && (GameManager.CurrentLevel.BaseData is ModLevelObject)
                && ((GameManager.OphelineController != null) || (GameManager.HerbeController != null)))
            {
                __result = true;
                return false;
            }
            return true;
        }
    }
}