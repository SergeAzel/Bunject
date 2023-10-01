using Bunburrows;
using Bunject.Internal;
using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bunject.Patches.VisualEffectsPatches
{
  [HarmonyPatch(typeof(VisualEffects.VisualEffectsController), 
    nameof(VisualEffects.VisualEffectsController.UpdateVisualEffects),
    new Type[] {typeof(VisualEffects.VisualEffectsInfo) })]
  // TODO: make a more sustainable implementation when custom Styles become a thing
  internal class UpdateVisualEffectsPatch
  {
    [HarmonyPriority(Priority.High)]
    private static void Prefix(ref VisualEffects.VisualEffectsInfo visualEffectsInfo)
    {
      if (visualEffectsInfo.Bunburrow is Bunburrow b && b.IsCustomBunburrow())
      {
        var level = AssetsManager.LevelsLists[b.ToBunburrowName()][visualEffectsInfo.Depth];
        visualEffectsInfo = new VisualEffects.VisualEffectsInfo(level.BunburrowStyle.Bunburrow, visualEffectsInfo.Depth, level.IsHell, level.IsTemple, false);
      }
    }
  }
}
