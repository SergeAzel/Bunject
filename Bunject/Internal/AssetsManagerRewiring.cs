using Bunject.Levels;
using Levels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace Bunject.Internal
{
  internal class AssetsManagerRewiring
  {
    internal static LevelsList LoadLevelsList(string name, LevelsList original)
    {
      if (original == null || original is ModLevelsList)
        return BunjectAPI.Forward.LoadLevelsList(name, original as ModLevelsList);
      return original;
    }
  }
}
