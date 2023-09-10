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
  /* Actually, worthless to make a base class when all methods are basically required to be implemented? 
  public abstract class TileSourceBase : ITileSource
  {
    public abstract Tile LoadTile(LevelObject levelObject, string tile, Vector2Int position);

    public virtual void UpdateTileSprite(TileLevelData tile, BunburrowStyle style) { }

    public abstract bool SupportsTile(LevelObject levelObject, string tile) { }
  }*/
}
