using Bunburrows;
using Levels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Bunject.Levels
{
  public interface IModBunburrow : IComparable<IModBunburrow>
  {
    int ID { get; set; }
    string Name { get; }
    string Indicator { get; }
    bool IsVoid { get; }
    BunburrowStyle Style { get; }

    bool HasEntrance { get; }

    bool HasSign { get; }

    Vector2Int? OverrideSignCoordinate();

    LevelsList GetLevels();

    LevelObject GetSurfaceLevel();
  }
}
