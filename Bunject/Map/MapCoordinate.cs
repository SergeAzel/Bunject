using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace Bunject.Map
{
  internal class MapCoordinate
  {
    public Bunburrows.Bunburrow Bunburrow { get; set; }
    public Vector2Int MapIndex { get; set; }
  }
}
