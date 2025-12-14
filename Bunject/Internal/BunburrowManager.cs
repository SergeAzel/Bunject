using Bunburrows;
using Bunject.Levels;
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
    internal static IReadOnlyList<BunburrowMetadata> Bunburrows
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

    internal static void RegisterBurrow(IModBunburrow modBunburrow)
    {
      if (Bunburrows.Any(bb => bb.ModBunburrow.Name == modBunburrow.Name))
        throw new ArgumentException($"Bunburrow name {modBunburrow.Name} is already in use!  Please use a unique name.");

      var id = ++instance.maxID;

      modBunburrow.ID = id;
      var bunburrow = new BunburrowMetadata()
      {
        ID = id,
        ComparisonIndex = id,
        IsCustom = true,
        ModBunburrow = modBunburrow
      };
      instance.bunburrows.Add(bunburrow);
    }

    internal static void RegisterElevator(int bunburrowID, int depth)
    {
      var burrow = Bunburrows.FirstOrDefault(x => x.ID == bunburrowID);
      if (burrow is null)
        throw new ArgumentException($"Bunburrow id {bunburrowID} does not exist!  Please use an existing id.");
      if (!burrow.Elevators.Contains(depth))
        burrow.Elevators.Add(depth);
    }

    internal static void ClearRegisters()
    {
      instance.FullReset();
    }

    internal void FullReset()
    {
      bunburrows = Enum.GetValues(typeof(global::Bunburrows.Bunburrow)).OfType<global::Bunburrows.Bunburrow>().Select(burrowEnum =>
        new BunburrowMetadata()
        {
          ID = (int)burrowEnum,
          ComparisonIndex = burrowEnum.ToComparisonIndex(),
          IsCustom = false,
          ModBunburrow = new CoreBunburrow(burrowEnum.ToBunburrowName(), burrowEnum.ToIndicator(), burrowEnum.IsVoidBunburrow())
        }).ToList();

      maxID = bunburrows.Max(bb => bb.ID) + CustomBunburrowThreshold;
      // pad out some space - ids are internally generated in our mods, but we're reusing them for a bunch of different internal identifiers
    }

    private BunburrowManager()
    {
      FullReset();
    }

    private List<BunburrowMetadata> bunburrows;
    private int maxID;
  }
}
