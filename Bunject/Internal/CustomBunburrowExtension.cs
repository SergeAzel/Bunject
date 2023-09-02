using Bunburrows;
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
      return ((int)bunburrow > BunburrowManager.CustomBunburrowThreshold);
    }

    public static IEnumerable<Bunburrows.Bunburrow> GetRegularAndCustomBunbrrowEnumerator()
    {
      foreach (var customBurrow in BunburrowManager.Bunburrows)
        yield return (Bunburrows.Bunburrow)customBurrow.ID;
    }
  }
}
