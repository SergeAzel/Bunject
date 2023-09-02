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
      if (original == null)
      {
        if (sourceList != null)
        {
          return BunjectAPI.Forward.LoadLevel(sourceList.name, depth, original);
        }
      }
      else
      {
        switch (original.name)
        {
          case "SurfaceRight":
            return BunjectAPI.Forward.LoadSpecialLevel(SpecialLevel.SurfaceBurrows, original);
        }

        if (sourceList != null)
        {
          return BunjectAPI.Forward.LoadLevel(sourceList.name, depth, original);
        }
      }
      return original;
    }
  }
}
