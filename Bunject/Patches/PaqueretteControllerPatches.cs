using System;
using System.Collections.Generic;
using System.Reflection;
using Bunject.Dialogue;
using Bunject.Levels;
using Characters;
using HarmonyLib;
using Levels;
using Misc;
using Tiling.Behaviour;

namespace Bunject.Patches.PaqueretteControllerPatches
{
    [HarmonyPatch(typeof(PaqueretteController), "CanTalk", MethodType.Getter)]
    internal class CanTalkPatches
    {
        private static void Postfix(ref bool __result)
        {
            TileLevelData paqueretteTile = GameManager.PaqueretteController.CurrentTile;
            TileLevelData facedTile = null;

            if (paqueretteTile != null)
            {
                facedTile = paqueretteTile.GetAdjacentTile(GameManager.PaqueretteController.FacedDirection);
            }
            
            if ((GameManager.OphelineController != null)
                && facedTile != null
                && facedTile == GameManager.OphelineController.CurrentTile
                && GameManager.CurrentLevel.BaseData is ModLevelObject)
            {
                ModLevelObject level = (ModLevelObject)GameManager.CurrentLevel.BaseData;
                if (level.OphelineDialogues != null && level.OphelineDialogues.Count != 0)
                {
                    foreach (NPCDialogueObject ophelineDialogue in level.OphelineDialogues)
                    {
                        if (ophelineDialogue.AreConditionsSatisfied(GameManager.GeneralProgression)
                            && !(ophelineDialogue.ShouldOnlyPlayOnce
                            && GameManager.GeneralProgression.PlayedOphelineDialogues.ContainsEquatable(ophelineDialogue.Name)))
                        {
                            __result = true;
                            break;
                        }
                    }
                }
            }
            
            if ((GameManager.HerbeController != null)
                && facedTile != null
                && facedTile == GameManager.HerbeController.CurrentTile
                && GameManager.CurrentLevel.BaseData is ModLevelObject)
            {
                ModLevelObject level = (ModLevelObject)GameManager.CurrentLevel.BaseData;
                if (level.HerbeDialogues != null && level.HerbeDialogues.Count != 0)
                {
                    foreach (NPCDialogueObject herbeDialogue in level.HerbeDialogues)
                    {
                        if (herbeDialogue.AreConditionsSatisfied(GameManager.GeneralProgression)
                            && !(herbeDialogue.ShouldOnlyPlayOnce
                            && GameManager.GeneralProgression.PlayedHerbeDialogues.ContainsEquatable(herbeDialogue.Name)))
                        {
                            __result = true;
                            break;
                        }
                    }
                }
            }
        }
    }
}