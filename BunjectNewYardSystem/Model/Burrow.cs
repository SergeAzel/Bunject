using Bunject.Levels;
using Levels;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bunject.NewYardSystem.Model
{
  public class Burrow
  {
    public string Directory { get; set; }
    public string Name { get; set; }
    public string Indicator { get; set; }
    public string Style { get; set; } = "Pink";
    public bool HasSurfaceEntry { get; set; }
    public bool IsVoid { get; set; }

    public int UpperBunnyCount { get; set; }
    public int TempleBunnyCount { get; set; }
    public int HellBunnyCount { get; set; }
    public int Depth { get; set; }

    // Counts required to unlock a bunburrow :)
    // Future: HC requirements?
    public int RequiredBunnyCount { get; set; }
    public int RequiredBabyCount { get; set; }

    public BurrowLinks Links { get; set; } = new BurrowLinks();
    public List<int> ElevatorDepths { get; set; } = new List<int>();

    [JsonIgnore]
    public Uri ProxyUri { get; set; }

    [JsonIgnore]
    public ModLevelsList Levels { get; set; }
  }
}
