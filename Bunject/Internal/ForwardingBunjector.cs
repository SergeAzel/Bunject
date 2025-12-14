using Bunburrows;
using Bunject.Levels;
using Bunject.Menu;
using Bunject.Monitoring;
using Bunject.Tiling;
using Characters.Bunny.Data;
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
  internal class ForwardingBunjector : IBunjectorPlugin, ITileSource, IMonitor, IMenuSource
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

    public LevelObject OnLevelLoad(LevelObject target, LevelIdentity identity)
    {
      var result = target;
      foreach (var monitors in BunjectAPI.Monitors)
      {
        result = monitors.OnLevelLoad(result, identity);
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

    public void OnBunnyCapture(BunnyIdentity bunnyIdentity, bool wasHomeCapture)
    {
      foreach (var bunjector in BunjectAPI.Monitors)
      {
        bunjector.OnBunnyCapture(bunnyIdentity, wasHomeCapture);
      }
    }

    public void OnMainMenu()
    {
      foreach (var bunjector in BunjectAPI.Monitors)
      {
        bunjector.OnMainMenu();
      }
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

    #region MenuSource - NOT IMPLEMENTED INTENTIONALLY
    // Implemented because IMenuSource is an IBunjectorPlugin, but we handle it elsewhere / differently
    public string MenuTitle => throw new NotImplementedException();
    public void DrawMenuOptions()
    {
      throw new NotImplementedException();
    }

    private GameObject PluginMenuObject { get; set; }

    internal void ShowOrHideMenu(bool showMenu)
    {
      if (PluginMenuObject == null)
      {
        PluginMenuObject = new GameObject();
        PluginMenuObject.AddComponent<MenuDisplay>();
      }

      PluginMenuObject.SetActive(showMenu);
    }
    #endregion
  }
}
