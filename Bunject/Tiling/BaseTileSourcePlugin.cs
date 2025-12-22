using BepInEx;
using Levels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Bunject.Tiling
{
  public abstract class BaseTileSourcePlugin //: BaseUnityPlugin, ITileSource
  {
    public abstract void Awake();

    public virtual void OnAssetsLoaded()
    {
    }

    public virtual void OnProgressionLoaded(GeneralProgression progression)
    {
    }

    public virtual LevelsList LoadEmergencyLevelsList(LevelsList original)
    {
      return original;
    }

    public abstract bool SupportsTile(string tile);

    public abstract Tile LoadTile(LevelObject levelObject, string tile, Vector2Int position);
  }
}
