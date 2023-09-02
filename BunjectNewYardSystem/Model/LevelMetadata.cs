using Levels;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bunject.NewYardSystem.Model
{
  public class LevelMetadata
  {
    public string Name { get; set; }

    public bool LiveReloading { get; set; } = false;
    public LevelTools Tools { get; set; } = new LevelTools();

    public string Style { get; set; }
    public bool IsTemple { get; set; }
    public bool IsHell { get; set; }

    [JsonIgnore]
    public LevelObject Level { get; set; }
  }

  public class LevelTools
  {
    public int Traps { get; set; }
    public int Pickaxes { get; set; }
    public int Carrots { get; set; }
    public int Shovels { get; set; }
  }
}
