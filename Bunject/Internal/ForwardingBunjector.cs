using Bunburrows;
using Bunject.Levels;
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
  internal class ForwardingBunjector : IBunjectorPlugin, ITileSource
  {
    #region IBunjectorPlugin Implementation
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
    #endregion

    #region ILevelSource Implementation
    public ModLevelObject LoadLevel(string listName, int index, ModLevelObject original)
    {
      var result = original;
      foreach (var levelSource in BunjectAPI.LevelSources)
      {
        result = levelSource.LoadLevel(listName, index, result);
      }
      return result;
    }

    public ModLevelsList LoadLevelsList(string name, ModLevelsList original)
    {
      var result = original;
      foreach (var levelSource in BunjectAPI.LevelSources)
      {
        result = levelSource.LoadLevelsList(name, result);
      }
      return result;
    }

    public LevelObject LoadBurrowSurfaceLevel(string listName, LevelObject otherwise)
    {
      var result = otherwise;
      foreach (var levelSource in BunjectAPI.LevelSources)
      {
        result = levelSource.LoadBurrowSurfaceLevel(listName, result);
      }
      return result;
    }

    /*
    public LevelObject StartLevelTransition(LevelObject target, LevelIdentity identity)
    {
      var result = target;
      foreach (var levelSource in BunjectAPI.LevelSources)
      {
        result = levelSource.StartLevelTransition(result, identity);
      }
      return result;
    }*/
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
