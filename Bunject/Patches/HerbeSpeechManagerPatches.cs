using System;
using Bunject.Dialogue;
using Bunject.Levels;
using Characters;
using Dialogue;
using Levels;
using Misc;
using HarmonyLib;

namespace Bunject.Patches.HerbeSpeechManagerPatches
{
    [HarmonyPatch(typeof(HerbeSpeechManager), "TryGetNextDialogue")]
    internal class TryGetNextDialoguePatch
    {
        private static bool Prefix(ref bool __result, out DialogueObject dialogue)
        {
            LevelObject currentLevel = GameManager.CurrentLevel.BaseData;
            if (currentLevel is ModLevelObject)
            {
                ModLevelObject modLevel = (ModLevelObject)currentLevel;
                if (modLevel.HerbeDialogues != null || modLevel.HerbeDialogues.Count == 0)
                {
                    foreach (NPCDialogueObject herbeDialogue in modLevel.HerbeDialogues)
                    {
                        if (herbeDialogue.AreConditionsSatisfied(GameManager.GeneralProgression) && herbeDialogue.ShouldOnlyPlayOnce && !GameManager.GeneralProgression.PlayedHerbeDialogues.ContainsEquatable(herbeDialogue.Name))
                        {
                            dialogue = herbeDialogue;
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

    [HarmonyPatch(typeof(HerbeSpeechManager), "TryGetNextTempleDialogue")]
    internal class TryGetNextTempleDialogue
    {
        private static bool Prefix(ref bool __result, out DialogueObject dialogue)
        {
            LevelObject currentLevel = GameManager.CurrentLevel.BaseData;
            if (currentLevel is ModLevelObject)
            {
                ModLevelObject modLevel = (ModLevelObject)currentLevel;
                if (modLevel.HerbeDialogues != null || modLevel.HerbeDialogues.Count == 0)
                {
                    foreach (NPCDialogueObject herbeDialogue in modLevel.HerbeDialogues)
                    {
                        if (herbeDialogue.AreConditionsSatisfied(GameManager.GeneralProgression) && herbeDialogue.ShouldOnlyPlayOnce && !GameManager.GeneralProgression.PlayedHerbeDialogues.ContainsEquatable(herbeDialogue.Name))
                        {
                            dialogue = herbeDialogue;
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