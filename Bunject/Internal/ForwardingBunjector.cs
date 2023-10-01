using Bunburrows;
using Bunject.Levels;
using Bunject.Monitoring;
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
  internal class ForwardingBunjector : IBunjectorPlugin, ITileSource, IMonitor
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

    #region ILevelSource (not actually a thing anymore but still used) Implementation
    public ModLevelsList LoadLevelsList(string name, ModLevelsList original)
    {
      var modBurrow = BunburrowManager.Bunburrows.FirstOrDefault(mb => mb.ModBunburrow.Name == name);
      if (modBurrow != null && modBurrow.IsCustom)
      {
        return (ModLevelsList)modBurrow.ModBunburrow.GetLevels();
      }

      return original; 
    }

    public LevelObject LoadBurrowSurfaceLevel(string listName, LevelObject otherwise)
    {
      var modBurrow = BunburrowManager.Bunburrows.FirstOrDefault(mb => mb.ModBunburrow.Name == listName);
      return modBurrow?.ModBunburrow?.GetSurfaceLevel() ?? otherwise;
    }
    #endregion

    #region IMonitor implementation

    public LevelObject StartLevelTransition(LevelObject target, LevelIdentity identity)
    {
      var result = target;
      foreach (var monitors in BunjectAPI.Monitors)
      {
        result = monitors.StartLevelTransition(result, identity);
      }
      return result;
    }

    public LevelsList LoadEmergencyLevelsList(LevelsList original)
    {
      var result = original;
      foreach (var bunjector in BunjectAPI.Monitors)
      {
        result = bunjector.LoadEmergencyLevelsList(result);
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
