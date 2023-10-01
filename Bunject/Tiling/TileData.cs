using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tiling.Behaviour;

namespace Bunject.Tiling
{
  public sealed class Tile
  {
    public Tile(TileLevelData instance)
    {
      this.Instance = instance;
    }

    public TileLevelData Instance { get; set; }
    public bool IsBunnyTile { get; set; } = false;
    public bool IsStartTile { get; set; } = false;
    public bool IsHoleTile { get; set; } = false; 
    public bool HasStartTrap { get; set; } = false;
    public bool HasStartCarrot { get; set; } = false;
  }
}
