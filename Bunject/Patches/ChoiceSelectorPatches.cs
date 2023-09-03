using Bunburrows;
using Bunject.Internal;
using Dialogue;
using HarmonyLib;
using Levels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Bunject.Patches.ChoiceSelectorPatches
{
	[HarmonyPatch(typeof(ChoiceSelector), nameof(ChoiceSelector.StartListeningToInputs))]
	internal class StartListeningToInputsPatch
	{
		private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
		{
			var codes = new List<CodeInstruction>(instructions);
			var GeneralInputManager = AccessTools.Property(typeof(GameManager), nameof(GameManager.GeneralInputManager)).GetGetMethod();
			var labels = codes.FindLast(x => x.Calls(GeneralInputManager))?.labels;
			if (!(labels is null))
			{
				int index = codes.FindIndex(x => labels.Any(y => y.Equals(x.operand)));
				if (index >= 0)
				{
					codes.InsertRange(index, new List<CodeInstruction>()
					{
						new CodeInstruction(OpCodes.Ldarg_0),
						new CodeInstruction(OpCodes.Call, AccessTools.Method(typeof(StartListeningToInputsPatch), nameof(StartListeningToInputsPatch.Infix)))
					});
				}
			}
			return codes;
		}

		private static void Infix(ChoiceSelector @this)
		{
			var elevators = new List<ChoiceObject>();
			var levels = new List<LevelIdentity>();
			foreach (var elevator in GameManager.GeneralProgression.UnlockedElevators)
			{
				if (ModElevatorController.Instance.TryGetLevel(elevator, out var level))
				{
					levels.Add(level);
					elevators.Add(new ChoiceObject(elevator, elevator, false));
				}
			}
			IReadOnlyList<ChoiceObject> readOnlyList = elevators;
			var traverse = Traverse.Create(@this);
			var choicesLineControllers = traverse.Field<List<ChoiceLineController>>("choicesLineControllers").Value;
			var contentRectTransform = traverse.Field<RectTransform>("contentRectTransform").Value;
			for (int i = 0; i < readOnlyList.Count; i++)
			{
				LevelIdentity levelIdentity = levels[i];
				BunburrowStyle levelBunburrowStyle = LevelIndicatorGenerator.GetLevelBunburrowStyle(levelIdentity);
				var component = UnityEngine.Object.Instantiate(AssetsManager.ChoiceLinePrefab, contentRectTransform).GetComponent<ChoiceLineController>();
				choicesLineControllers.Add(component);
				component.Init(readOnlyList[i], LevelIndicatorGenerator.GetShortLevelIndicator(levelIdentity));
				component.UpdateStyle(Color.black, levelBunburrowStyle.SkyboxColor, levelBunburrowStyle.UIWhiteColor, levelBunburrowStyle.ButtonDefaultColor, levelBunburrowStyle.FlowerSprite);
			}
		}
	}
}