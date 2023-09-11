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
      var modBurrow = BunburrowManager.Bunburrows.FirstOrDefault(mb => mb.Name == name);
      if (modBurrow != null && modBurrow.LevelSource != null)
      {
        return modBurrow.LevelSource.LoadLevelsList(name, original as ModLevelsList);
      }

      return original;
    }
  }
}
