using Bunburrows;
using Bunject.Internal;
using HarmonyLib;
using Levels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
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
      // get previous bunburrow
      Bunburrows.Bunburrow previous = Traverse.Create<GameManager>().Field<Bunburrows.Bunburrow>("previousBunburrow").Value;

      var targetSurfaceLevel = BunjectAPI.Forward.LoadBurrowSurfaceLevel(AssetsManager.LevelsLists[previous.ToBunburrowName()].name, surfaceLevel);

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


	[HarmonyPatch(typeof(GameManager), nameof(GameManager.HandleSurfaceElevatorUse))]
	internal class HandleSurfaceElevatorUsePatch
	{
		private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator il)
		{
			var codes = new List<CodeInstruction>(instructions);
			var TryGetValue = AccessTools.Method(typeof(IReadOnlyDictionary<string, LevelIdentity>), nameof(IReadOnlyDictionary<string, LevelIdentity>.TryGetValue));
			var index = codes.FindIndex(x => x.Calls(TryGetValue));
			if (index >= 0 && index + 2 < codes.Count && codes[index + 1].Branches(out _))
			{
				Label l = il.DefineLabel();
				codes.Insert(index + 2, new CodeInstruction(OpCodes.Nop).WithLabels(l));
				codes.InsertRange(index + 1, new List<CodeInstruction>()
				{
					new CodeInstruction(OpCodes.Brtrue, l),
					new CodeInstruction(OpCodes.Ldarg_0),
					new CodeInstruction(OpCodes.Ldloca, 0),
					new CodeInstruction(OpCodes.Call, AccessTools.Method(typeof(HandleSurfaceElevatorUsePatch), nameof(HandleSurfaceElevatorUsePatch.Infix))),
				});
			}
			return codes;
		}

		private static bool Infix(string elevatorName, out LevelIdentity levelIdentity)
		{
			return ModElevatorController.Instance.TryGetLevel(elevatorName, out levelIdentity);
		}
	}
}
