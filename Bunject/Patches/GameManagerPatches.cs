using Bunburrows;
using Bunject.Internal;
using Bunject.Levels;
using HarmonyLib;
using Levels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;
using Tiling.Behaviour;
using UnityEngine;

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

  [HarmonyPatch(typeof(GameManager), "InstantiateBunburrowEntrySigns")]
  internal class InstantiateBunburrowEntrySignsPatch
  {
    private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator il)
    {
      var tileLevelData_LeftTile = AccessTools.PropertyGetter(typeof(TileLevelData), nameof(TileLevelData.LeftTile));
      int state = 0;
      foreach (var code in instructions)
      {
        yield return code;
        switch (state)
        {
          case 0:
            if (code.Calls(tileLevelData_LeftTile))
            {
              state = 1;
            }
            break;
          case 1:
            if (code.IsStloc())
            {
              yield return new CodeInstruction(OpCodes.Ldloc_2);
              yield return new CodeInstruction(OpCodes.Ldloc_3);
              yield return new CodeInstruction(OpCodes.Call, AccessTools.Method(typeof(InstantiateBunburrowEntrySignsPatch), nameof(GetSignTile)));
              yield return new CodeInstruction(OpCodes.Stloc_3);
              state = 2;
            }
            break;
        }
      }
    }

    private static FloorTile GetSignTile(BunburrowEntryTile currentHole, FloorTile otherwise)
    {
      FloorTile res = otherwise;
      if (currentHole.Bunburrow is Bunburrow b && b.IsCustomBunburrow() && b.GetModBunburrow() is IModBunburrow modBurrow)
      {
        if (!modBurrow.HasSign)
        {
          // Override... prevent sign?
          res = null;
        }
        else if (!modBurrow.HasEntrance)
        {
          // If sign with no entrance.. replace entry hole with sign?
          res = currentHole;
        }
        else if (modBurrow.OverrideSignCoordinate() is Vector2Int coordinate)
        {
          // If it doesn't cast as FloorTile, implicit failure.
          res = LevelBuilderExtensions.GetTileInListByCoordinates(GameManager.CurrentLevel.Tiles.ToList(), coordinate.y, coordinate.x) as FloorTile ?? res;
        }
      }
      return res;
    }
  }
}
