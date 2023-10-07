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
  internal class CoreBunburrow : IModBunburrow
  {
    public CoreBunburrow(string name, string indicator, bool isVoid)
    {
      Name = name;
      Indicator = indicator;
      IsVoid = isVoid;
    }

    public string Name { get; }
    public string Indicator { get; }
    public bool IsVoid { get; }
    public int ID { get; set; }

    // Never actually called
    public BunburrowStyle Style { get; }

    private LevelsList levels;

    public bool HasEntrance => true;
    public bool HasSign => true;

    public Vector2Int? OverrideSignCoordinate()
    {
      return null;
    }

    public LevelsList GetLevels()
    {
      if (levels == null)
        levels = AssetsManager.LevelsLists[Name];
      return levels;
    }

    public LevelObject GetLevel(int depth)
    {
      return GetLevels()[depth];
    }

    public LevelObject GetSurfaceLevel()
    {
      return AssetsManager.SurfaceRightLevel;
    }

		public int CompareTo(IModBunburrow other)
		{
			return other is CoreBunburrow ? (ID - other.ID) : 0;
		}
	}
}
