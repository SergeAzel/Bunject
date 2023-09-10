using Bunburrows;
using Bunject.Tiling;
using HarmonyLib;
using Levels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tiling.Behaviour;
using UnityEngine;

namespace Bunject.Internal
{
  internal class ForwardingBunjector : IBunjector, ITileSource
  {
    #region IBunjector Implementation
    public void OnAssetsLoaded()
    {
      foreach (var bunjector in BunjectAPI.Bunjectors)
      {
        bunjector.OnAssetsLoaded();
      }
    }

    public void OnProgressionLoaded(GeneralProgression progression)
    {
      foreach (var bunjector in BunjectAPI.Bunjectors)
      {
        bunjector.OnProgressionLoaded(progression);
      }
    }

    public LevelObject LoadLevel(string listName, int index, LevelObject original)
    {
      var result = original;
      foreach (var bunjector in BunjectAPI.Bunjectors)
      {
        result = bunjector.LoadLevel(listName, index, result);
      }
      return result;
    }

    public LevelsList LoadLevelsList(string name, LevelsList original)
    {
      var result = original;
      foreach (var bunjector in BunjectAPI.Bunjectors)
      {
        result = bunjector.LoadLevelsList(name, result);
      }
      return result;
    }

    public LevelObject LoadSpecialLevel(SpecialLevel levelEnum, LevelObject original)
    {
      var result = original;
      foreach (var bunjector in BunjectAPI.Bunjectors)
      {
        result = bunjector.LoadSpecialLevel(levelEnum, result);
      }
      return result;
    }

    public LevelObject RappelFromBurrow(string listName, LevelObject otherwise)
    {
      var result = otherwise;
      foreach (var bunjector in BunjectAPI.Bunjectors)
      {
        result = bunjector.RappelFromBurrow(listName, result);
      }
      return result;
    }

    public LevelObject StartLevelTransition(LevelObject target, LevelIdentity identity)
    {
      var result = target;
      foreach (var bunjector in BunjectAPI.Bunjectors)
      {
        result = bunjector.StartLevelTransition(result, identity);
      }
      return result;
    }
    #endregion

    #region ITileSource Implementation
    public bool SupportsTile(string tile)
    {
      return BunjectAPI.TileSources.Any(ts => ts.SupportsTile(tile));
    }

    public Tile LoadTile(LevelObject levelObject, string tile, Vector2Int position)
    {
      return BunjectAPI.TileSources.FirstOrDefault(ts => ts.SupportsTile(tile))?.LoadTile(levelObject, tile, position);
    }
    #endregion
  }
}
