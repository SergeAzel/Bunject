using Bunject.Levels;
using Levels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bunject.Internal
{
  internal static class OnLoadLevel
  {
    internal static LevelObject LoadLevel(LevelObject original)
    {
      return LoadLevel(original, null, 0);
    }

    internal static LevelObject LoadLevel(LevelObject original, LevelsList sourceList, int depth)
    {
      var burrowName = sourceList?.name;
      if (!string.IsNullOrEmpty(burrowName) != null)
      {
        var modBurrow = BunburrowManager.Bunburrows.FirstOrDefault(mb => mb.Name == burrowName);
        if (modBurrow != null) 
        {
          return modBurrow.LevelSource.LoadLevel(burrowName, depth, original as ModLevelObject);
        }
      }

      return original;
    }
  }
}
