using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;
using Bunject.Dialogue;
using Bunject.Levels;
using Dialogue;
using Items;
using Levels;
using Tiling.Behaviour;

namespace Bunject.Patches.PaqueretteActionResolverPatches
{
  //Total hack to ensure signs properly passthrough the burrow we need to clear out instead of relying on burrowstyle
  [HarmonyPatch(typeof(PaqueretteActionResolver), nameof(PaqueretteActionResolver.HandleTalkButtonPress))]
  internal class HandleTalkButtonPressPatches
  {
    public static Bunburrows.Bunburrow? targetBurrow;

    private static MethodInfo GetBunburrowUnlockStatus = typeof(GeneralProgression).GetProperty(nameof(GeneralProgression.BunburrowsUnlockStatus)).GetGetMethod();

    private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
    {
      int detectionState = 0;
      foreach (var instruction in instructions)
      {
        switch (detectionState)
        {
          case 0:
            if (instruction.Calls(GetBunburrowUnlockStatus))
              detectionState++;
            break;
          case 1:
            if (instruction.Branches(out Label? _))
            {
              yield return instruction;
              yield return new CodeInstruction(OpCodes.Ldloc_S, 4);
              yield return CodeInstruction.Call(typeof(BunburrowSignController), "get_Bunburrow"); // Calling a getter.. I guess it worked
              yield return CodeInstruction.Call(typeof(HandleTalkButtonPressPatches), nameof(HandleTalkButtonPressPatches.ExtractBurrow));
              detectionState++;
              continue;
            }
            break;
        }
        yield return instruction;
      }
    }

    private static void ExtractBurrow(Bunburrows.Bunburrow bunburrow)
    {
      targetBurrow = bunburrow;
    }

    // Allow Opheline and Herbe "idle" dialogues
    private static bool Prefix()
    {
      LevelObject currentLevel = GameManager.CurrentLevel.BaseData;
      
      if (currentLevel is ModLevelObject && !currentLevel.IsSurface)
      {
        ModLevelObject modLevel = (ModLevelObject)currentLevel;
        TileLevelData paqueretteTile = GameManager.PaqueretteController.CurrentTile;

        if (paqueretteTile != null)
        {
          TileLevelData facedTile = paqueretteTile.GetAdjacentTile(GameManager.PaqueretteController.FacedDirection);
          if (GameManager.OphelineController != null && GameManager.OphelineController.CurrentTile == facedTile)
          {
            if (GameManager.OphelineController.NextDialogue != null)
            {
              GameManager.OphelineController.StartDialogue(false);
            }
            else
            {
              if (modLevel.OphelineDialogues != null && modLevel.OphelineDialogues.Count != 0)
              {
                foreach (NPCDialogueObject ophelineDialogue in modLevel.OphelineDialogues)
                {
                  if (ophelineDialogue.AreConditionsSatisfied(GameManager.GeneralProgression) && !ophelineDialogue.ShouldOnlyPlayOnce)
                  {
                    GameManager.OphelineController.StartDialogue(true);
                    GameManager.UIController.DisplayDialogue(ophelineDialogue, GameManager.CurrentBunburrowStyle);
                    break;
                  }
                }
              }
            }
          }

          if (GameManager.HerbeController != null && GameManager.HerbeController.CurrentTile == facedTile)
          {
            if (GameManager.HerbeController.NextDialogue != null)
            {
              GameManager.HerbeController.StartDialogue(false);
            }
            else
            {
              if (modLevel.HerbeDialogues != null && modLevel.HerbeDialogues.Count != 0)
              {
                foreach (NPCDialogueObject herbeDialogue in modLevel.HerbeDialogues)
                {
                  if (herbeDialogue.AreConditionsSatisfied(GameManager.GeneralProgression) && !herbeDialogue.ShouldOnlyPlayOnce)
                  {
                    GameManager.HerbeController.StartDialogue(true);
                    GameManager.UIController.DisplayDialogue(herbeDialogue, GameManager.CurrentBunburrowStyle);
                    break;
                  }
                }
              }
            }
          }
        }
        return false;
      }
      return true;
    }
  }

  [HarmonyPatch(typeof(PaqueretteActionResolver), "HandleDirectionFacedChange")]
  internal class HandleDirectionFacedChangePatches
  {
    private static void Postfix()
    {
      if (GameManager.PaqueretteController.CanTalk)
      {
        GameManager.PaqueretteController.HandleContextualItemSwitch(Item.Talk);
      }
    }
  }
}