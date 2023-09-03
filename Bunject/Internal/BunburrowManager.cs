using Bunburrows;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bunject.Internal
{
  internal class BunburrowManager
  {
    private static BunburrowManager instance = null;
    internal static bool IsInitialized => instance != null;
    internal static IReadOnlyList<ModBunburrow> Bunburrows
    {
      get
      {
        if (!IsInitialized)
        {
          instance = new BunburrowManager();
        }
        return instance.bunburrows;
      }
    }

    public static int CustomBunburrowThreshold => 50;

    internal static int RegisterBurrow(string name, string indicator, bool isVoid = false)
    {
      if (Bunburrows.Any(bb => bb.Name == name))
        throw new ArgumentException($"Bunburrow name {name} is already in use!  Please use a unique name.");

      if (Bunburrows.Any(bb => bb.Indicator == indicator))
        throw new ArgumentException($"Bunburrow indicator {indicator} is already in use!  Please use a unique indicator.");

      var id = ++instance.maxID;

      var bunburrow = new ModBunburrow()
      {
        ID = id,
        Name = name, 
        ComparisonIndex = id,
        IsCustom = true,
        Indicator = indicator,
        IsVoid = isVoid
      };
      instance.bunburrows.Add(bunburrow);

      return id;
    }

    // [Done]
    // having 3 methods might be a little excessive
    // (considering the ID one should be more than sufficient cause RegisterBurrow returns ID)
    // consider removing Indicator variant but *maybe* not name and refactor both methods to "RegisterElevator"
    internal static void RegisterElevator(int id, int depth)
    {
      var burrow = Bunburrows.FirstOrDefault(x => x.ID == id);
      if (burrow is null)
        throw new ArgumentException($"Bunburrow id {id} does not exist!  Please use an existing id.");
      if (burrow.Elevators.Contains(depth))
        throw new ArgumentException($"Bunburrow depth {depth} already contains elevator!  Please use a depth without one.");
      burrow.Elevators.Add(depth);
    }
    internal static void RegisterElevator(string name, int depth)
    {
      var burrow = Bunburrows.FirstOrDefault(x => x.Name == name);
      if (burrow is null)
        throw new ArgumentException($"Bunburrow name {name} does not exist!  Please use an existing id.");
      if (burrow.Elevators.Contains(depth))
        throw new ArgumentException($"Bunburrow depth {depth} already contains elevator!  Please use a depth without one.");
      burrow.Elevators.Add(depth);
    }
    /*
    internal static void RegisterElevatorByBurrowIndicator(string indicator, int depth)
    {
      var burrow = Bunburrows.FirstOrDefault(x => x.Indicator == indicator);
      if (burrow is null)
        throw new ArgumentException($"Bunburrow indicator {indicator} does not exist!  Please use an existing id.");
      if (burrow.Elevators.Contains(depth))
        throw new ArgumentException($"Bunburrow depth {depth} already contains elevator!  Please use a depth without one.");
      burrow.Elevators.Add(depth);
    }
    */
    private BunburrowManager()
    {
      bunburrows = Enum.GetValues(typeof(global::Bunburrows.Bunburrow)).OfType<global::Bunburrows.Bunburrow>().Select(burrowEnum =>
        new ModBunburrow()
        {
          ID = (int)burrowEnum,
          Name = burrowEnum.ToBunburrowName(),
          ComparisonIndex = burrowEnum.ToComparisonIndex(),
          IsCustom = false,
          Indicator = burrowEnum.ToIndicator()
        }).ToList();

      maxID = bunburrows.Max(bb => bb.ID) + CustomBunburrowThreshold; 
      // pad out some space - ids are internally generated in our mods, but we're reusing them for a bunch of different internal identifiers
    }

    private List<ModBunburrow> bunburrows;
    private int maxID;
  }
}
