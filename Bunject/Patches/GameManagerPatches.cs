using Bunburrows;
using Bunject.Internal;
using HarmonyLib;
using Levels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Bunject.Patches.GameManagerPatches
{
  [HarmonyPatch(typeof(GameManager), "CheckForPotentialOutOfBoundsLevel")]
  class CheckForPotentialOutOfBoundsLevelPatch
  {
    private static bool Postfix(bool __result, Misc.Direction direction, ref LevelObject levelObject)
    {
      if (__result && levelObject != null)
      {
        levelObject = OnLoadLevel.LoadLevel(levelObject) ?? levelObject;
      }
      return __result;
    }
  }

  // Correct the burrow provided by dialogues to the HandleDialogueEvent proc
  [HarmonyPatch(typeof(GameManager), nameof(GameManager.HandleDialogueEvent))]
  class HandleDialogueEventPatches
  {
    private static void Prefix(string eventKey, ref Bunburrows.Bunburrow? bunburrowInfo)
    {
      if (eventKey == "release")
      {
        bunburrowInfo = PaqueretteActionResolverPatches.HandleTalkButtonPressPatches.targetBurrow;
        PaqueretteActionResolverPatches.HandleTalkButtonPressPatches.targetBurrow = null;
      }
    }
  }

  [HarmonyPatch(typeof(GameManager), nameof(GameManager.HandleRappelling))]
  class HandleRappellingPatches
  {
    static MethodInfo StartLevelTransition = typeof(GameManager).GetMethod("StartLevelTransition", BindingFlags.NonPublic | BindingFlags.Static);

    private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
    {
      int state = 0;
      foreach (var instruction in instructions)
      {
        switch (state)
        {
          case 0:
            // only replace second transition
            if (instruction.Calls(StartLevelTransition))
              state++;
            break;
          case 1:
            if (instruction.Calls(StartLevelTransition))
            {
              yield return CodeInstruction.Call(typeof(HandleRappellingPatches), nameof(HandleRappellingPatches.StartLevelRappellingTransition));
              state++;
              continue;
            }
            break;
        }
        yield return instruction;
      }
    }



    // signature must match StartLevelTransition
    private static void StartLevelRappellingTransition(LevelObject surfaceLevel, LevelTransitionType levelTransitionType, LevelIdentity levelIdentity, LevelIdentity? elevatorTargetLevelIdentity)
    {
      Console.WriteLine("Rapelling to Surface Transition!");
      // get previous bunburrow
      Bunburrows.Bunburrow previous = Traverse.Create<GameManager>().Field<Bunburrows.Bunburrow>("previousBunburrow").Value;

      var targetSurfaceLevel = BunjectAPI.Forward.RappelFromBurrow(AssetsManager.LevelsLists[previous.ToBunburrowName()].name, surfaceLevel);

      // slow but not like its called every frame
      StartLevelTransition.Invoke(null, new object[] { targetSurfaceLevel, levelTransitionType, levelIdentity, elevatorTargetLevelIdentity });
    }
  }

  [HarmonyPatch(typeof(GameManager), "StartLevelTransition")]
  class StartLevelTransitionPatches
  {
    private static void Prefix(ref LevelObject levelObject, LevelIdentity levelIdentity)
    {
      levelObject = BunjectAPI.Forward.StartLevelTransition(levelObject, levelIdentity);
    }
  }

  [HarmonyPatch(typeof(GameManager), "set_GeneralProgression")]
  class SetGeneralProgressionPatches
  {
    private static void Prefix(GeneralProgression value)
    {
      BunjectAPI.Forward.OnProgressionLoaded(value);
    }
  }
}
