using Bunburrows;
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
  internal class ForwardingBunjector : IBunjector
  {
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

    public bool ValidateBaseTile(LevelObject levelObject, string tile)
    {
      return BunjectAPI.Bunjectors.All(x => x.ValidateBaseTile(levelObject, tile));
    }

    public bool ValidateModTile(LevelObject levelObject, string tile)
    {
      return BunjectAPI.Bunjectors.Any(x => x.ValidateModTile(levelObject, tile));
    }

    public TileLevelData LoadTile(LevelObject levelObject, string tile, Vector2Int position, 
      out bool isBunnyTile, out bool isStartTile, out bool isHoleTile, out bool hasStartTrap, out bool hasStartCarrot)
		{
			isBunnyTile = false;
			isStartTile = false;
			isHoleTile = false;
			hasStartTrap = false;
			hasStartCarrot = false;
			TileLevelData result = null;
      foreach (var bunjector in BunjectAPI.Bunjectors)
      {
        result = bunjector.LoadTile(levelObject, tile, position, out isBunnyTile, out isStartTile, out isHoleTile, out hasStartTrap, out hasStartCarrot);
        if (result != null)
          break;
      }
      return result;
		}

		public void UpdateTileSprite(TileLevelData tile, BunburrowStyle style)
    {
      foreach (var bunjector in BunjectAPI.Bunjectors)
      {
        bunjector.UpdateTileSprite(tile, style);
      }
    }
  }
}
