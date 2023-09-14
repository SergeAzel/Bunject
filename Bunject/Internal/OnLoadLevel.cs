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
      if (sourceList is ModLevelsList modList && (original is ModLevelObject || original == null))
        return BunjectAPI.Forward.LoadLevel(modList, depth, original as ModLevelObject);
      return original;
    }
  }
}
