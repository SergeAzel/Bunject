using System;
using System.Collections.Generic;
using Computer.Paquerette.Logs;
using HarmonyLib;

namespace Bunject.Patches.PaqueretteLogsWindowControllerPatches
{
    [HarmonyPatch(typeof(PaqueretteLogsWindowController), "<OpenWindow>g__AddDialogues|18_0")]
    internal class AddDialoguesPatches
    {
        static void Prefix(ref IReadOnlyList<string> dialogueNames)
        {
            // Prevent custom dialogues from breaking Paquerette's laptop
            List<string> result = new List<string>();

            foreach (string dialogue in dialogueNames)
            {
                if (AssetsManager.Dialogues.ContainsKey(dialogue))
                {
                    result.Add(dialogue);
                }
            }

            dialogueNames = result;
        }
    }
}