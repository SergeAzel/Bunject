using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bunject.Archipelago.Client
{
  public enum VictoryCondition
  {
    Credits = 0,
    GoldenBunny = 1,
    GoldenFluffle = 2,
    FullClear = 3
  }

  public enum Trap
  {
    Random,
    SurfaceTeleport,
    Elevator,
  }

  public class ArchipelagoOptions
  {
    public static ArchipelagoOptions ParseSlotData(Dictionary<string, object> keyValuePairs)
    {
      var options = new ArchipelagoOptions();

      foreach (var pair in keyValuePairs)
      {
        ArchipelagoPlugin.BepinLogger.LogInfo($"Option: '{pair.Key}': '{(Int64)pair.Value}'");

        switch (pair.Key)
        {
          case nameof(home_captures):
            options.home_captures = (Int64)pair.Value == 1;
            break;
          case nameof(victory_condition):
            options.victory_condition = (VictoryCondition)(Int64)pair.Value;
            break;
          case nameof(golden_fluffles):
            options.golden_fluffles = (int)(Int64)pair.Value;
            break;
          case nameof(unlock_computer):
            options.unlock_computer = (Int64)pair.Value == 1;
            break;
          case nameof(unlock_map):
            options.unlock_map = (Int64)pair.Value == 1;
            break;
          case nameof(death_link):
            options.death_link = (Int64)pair.Value == 1;
            break;
          case nameof(elevator_trap_depth):
            options.elevator_trap_depth = (int)(Int64)pair.Value;
            break;
          case nameof(elevator_trap_increment):
            options.elevator_trap_increment = (int)(Int64)pair.Value;
            break;
          case nameof(death_link_behavior):
            options.death_link_behavior = (Trap)(Int64)pair.Value;
            break;
        }
      }

      return options;
    }


    public bool home_captures { get; private set; }
    
    public VictoryCondition victory_condition { get; private set; }

    public int golden_fluffles { get; private set; }

    public bool unlock_computer { get; private set; }

    public bool unlock_map { get; private set; }

    public bool death_link { get; private set; }

    public Trap death_link_behavior { get; private set; } = Trap.Random;

    public int elevator_trap_depth { get; private set; } = 5;

    public int elevator_trap_increment { get; private set; }
  }
}
