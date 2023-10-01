using Bunburrows;
using Bunject.Levels;
using Bunject.NewYardSystem.Model;
using Levels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Bunject.NewYardSystem.Levels
{
  public class BNYSLostBunburrow : IModBunburrow
  {
    public BNYSLostBunburrow(CustomBunburrowModel cachedBurrow) 
    {
      Name = (string.IsNullOrEmpty(cachedBurrow.World) ? string.Empty : cachedBurrow.World + "::") + cachedBurrow.Name;
      Indicator = (string.IsNullOrEmpty(cachedBurrow.Prefix) ? string.Empty : cachedBurrow.Prefix + "-") + cachedBurrow.Indicator;
    }

    public int ID { get; set; }

    // To be uncommented when world caching is refined
    public string Name { get; set; }

    public string Indicator { get; set; }

    public bool IsVoid { get; set; } = false;

    public BunburrowStyle Style { get; set; }

    public bool HasEntrance => false;
    public bool HasSign => false;

    public Vector2Int? OverrideSignCoordinate()
    {
      return null;
    }

    public LevelObject GetLevel(int depth)
    {
      return null;
    }

    public LevelsList GetLevels()
    {
      return null;
    }

    public LevelObject GetSurfaceLevel()
    {
      return null;
    }

  }
}
