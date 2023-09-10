using Bunburrows;
using Levels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tiling.Behaviour;
using UnityEngine;

namespace Bunject.Tiling
{
  public interface ITileSource
  {
    bool SupportsTile(string tile);

    Tile LoadTile(LevelObject levelObject, string tile, Vector2Int position);
  }
}
