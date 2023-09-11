using Levels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bunject.Levels
{
  public interface ILevelSource : IBunjectorPlugin
  {
    ModLevelsList LoadLevelsList(string bunburrowName, ModLevelsList original);

    ModLevelObject LoadLevel(ModLevelsList sourceList, int depth, ModLevelObject original);

    LevelObject LoadBurrowSurfaceLevel(string bunburrowName, LevelObject otherwise);
  }
}
