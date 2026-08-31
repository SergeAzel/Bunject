using System;
using Bunject.Dialogue;
using Bunject.Levels;
using Characters;
using Dialogue;
using Levels;
using Misc;
using HarmonyLib;

namespace Bunject.Patches.OphelineSpeechManagerPatches
{
    [HarmonyPatch(typeof(OphelineSpeechManager), "TryGetNextDialogue")]
    internal class TryGetNextDialoguePatch
    {
        private static bool Prefix(ref bool __result, out DialogueObject dialogue)
        {
            LevelObject currentLevel = GameManager.CurrentLevel.BaseData;
            if (currentLevel is ModLevelObject)
            {
                ModLevelObject modLevel = (ModLevelObject)currentLevel;
                if (modLevel.OphelineDialogues != null && modLevel.OphelineDialogues.Count != 0)
                {
                    foreach (NPCDialogueObject ophelineDialogue in modLevel.OphelineDialogues)
                    {
                        if (ophelineDialogue.AreConditionsSatisfied(GameManager.GeneralProgression) && ophelineDialogue.ShouldOnlyPlayOnce && !GameManager.GeneralProgression.PlayedOphelineDialogues.ContainsEquatable(ophelineDialogue.Name))
                        {
                            dialogue = ophelineDialogue;
                            __result = true;
                            return false;
                        }
                    }
                }
                dialogue = null;
                __result = false;
                return false;
            }
            dialogue = null;
            return true;
        }
    }

    [HarmonyPatch(typeof(OphelineSpeechManager), "TryGetNextHellDialogue")]
    internal class TryGetNextHellDialoguePatch
    {
        private static bool Prefix(ref bool __result, out DialogueObject dialogue)
        {
            LevelObject currentLevel = GameManager.CurrentLevel.BaseData;
            if (currentLevel is ModLevelObject)
            {
                ModLevelObject modLevel = (ModLevelObject)currentLevel;
                if (modLevel.OphelineDialogues != null && modLevel.OphelineDialogues.Count != 0)
                {
                    foreach (NPCDialogueObject ophelineDialogue in modLevel.OphelineDialogues)
                    {
                        if (ophelineDialogue.AreConditionsSatisfied(GameManager.GeneralProgression) && ophelineDialogue.ShouldOnlyPlayOnce && !GameManager.GeneralProgression.PlayedOphelineDialogues.ContainsEquatable(ophelineDialogue.Name))
                        {
                            dialogue = ophelineDialogue;
                            __result = true;
                            return false;
                        }
                    }
                }
                dialogue = null;
                __result = false;
                return false;
            }
            dialogue = null;
            return true;
        }
    }
}