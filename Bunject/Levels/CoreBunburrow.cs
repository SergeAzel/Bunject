using Levels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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

    private LevelsList levels;

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
  }
}
