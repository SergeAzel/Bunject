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

  public class ArchipelagoOptions
  {
    public static ArchipelagoOptions ParseSlotData(Dictionary<string, object> keyValuePairs)
    {
      var options = new ArchipelagoOptions();

      foreach (var pair in keyValuePairs)
      {
        switch (pair.Key)
        {
          case nameof(ArchipelagoOptions.home_captures):
            options.home_captures = (Int64)pair.Value == 1;
            break;
          /*case nameof(ArchipelagoOptions.expert_routing):   -- Not needed locally
            options.expert_routing = (Int64)pair.Value == 1;
            break;*/
          case nameof(ArchipelagoOptions.victory_condition):
            options.victory_condition = (VictoryCondition)(Int64)pair.Value;
            break;
          case nameof(ArchipelagoOptions.golden_fluffles):
            options.golden_fluffles = (int)(Int64)pair.Value;
            break;
          case nameof(ArchipelagoOptions.unlock_computer):
            options.unlock_computer = (Int64)pair.Value == 1;
            break;
          case nameof(ArchipelagoOptions.unlock_map):
            options.unlock_map = (Int64)pair.Value == 1;
            break;
        }
      }

      return options;
    }


    public bool home_captures { get; private set; }
    
    //public bool expert_routing { get; private set; }

    public VictoryCondition victory_condition { get; private set; }

    public int golden_fluffles { get; private set; }

    public bool unlock_computer { get; private set; }

    public bool unlock_map { get; private set; }
  }
}
