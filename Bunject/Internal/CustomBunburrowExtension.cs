using Bunburrows;
using Bunject.Levels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bunject.Internal
{
  public static class CustomBunburrowExtension
  {
    public static bool IsVoidOrCustomBunburrow(this Bunburrows.Bunburrow bunburrow)
    {
      return bunburrow.IsCustomBunburrow() || bunburrow.IsVoidBunburrow();
    }

    public static bool IsCoreBunburrow(this Bunburrows.Bunburrow bunburrow)
    {
      return !bunburrow.IsVoidOrCustomBunburrow();
    }

    public static bool IsCustomBunburrow(this Bunburrows.Bunburrow bunburrow)
    {
      return ((int)bunburrow > BunburrowManager.CustomBunburrowThreshold) && BunburrowManager.Bunburrows.Any(x => x.ID == (int)bunburrow);
    }

    // Whether the burrow's entrance is currently open. Core burrows defer to the vanilla
    // unlock table; custom burrows must satisfy every configured threshold - lifetime
    // (history) bunny captures and baby (existing couple) count - mirroring the counters
    // vanilla itself uses for gated unlocks. Both counters only ever grow, so a burrow
    // that has opened never re-locks.
    public static bool IsUnlocked(this Bunburrows.Bunburrow bunburrow)
    {
      if (bunburrow.IsCustomBunburrow())
      {
        var mod = bunburrow.GetModBunburrow();
        var progression = GameManager.GeneralProgression;
        var bunniesRequired = mod?.RequiredBunnyCount ?? 0;
        var babiesRequired = mod?.RequiredBabyCount ?? 0;

        return (bunniesRequired <= 0 || progression.HistoryCapturedBunnies.Count >= bunniesRequired)
            && (babiesRequired <= 0 || progression.ExistingCouples.Count >= babiesRequired);
      }

      return GameManager.GeneralProgression.BunburrowsUnlockStatus[bunburrow];
    }

    public static IModBunburrow GetModBunburrow(this Bunburrows.Bunburrow bunburrow)
    {
      return BunburrowManager.Bunburrows.FirstOrDefault(x => x.ID == (int)bunburrow)?.ModBunburrow;
    }

    public static IEnumerable<Bunburrows.Bunburrow> GetRegularAndCustomBunbrrowEnumerator()
    {
      foreach (var customBurrow in BunburrowManager.Bunburrows)
        yield return (Bunburrows.Bunburrow)customBurrow.ID;
    }
  }
}
