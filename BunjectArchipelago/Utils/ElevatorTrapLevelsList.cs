using Bunject.Levels;
using Levels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Bunject.Archipelago.Utils
{
  public class ElevatorTrapLevelsList : ModLevelsList
  {
    public ElevatorTrapLevelsList()
    {
      NumberOfRegularBunnies = 0;
      NumberOfTempleBunnies = 0;
      NumberOfHellBunnies = 0;

      TempleStartDepth = 2;
      HellStartDepth = 21;

      dummyTemple = ScriptableObject.CreateInstance<ModLevelObject>();
      dummyTemple.IsTemple = true;
      dummyTemple.BunburrowStyle = AssetsManager.BunburrowsListOfStyles.Temple;

      dummyHell = ScriptableObject.CreateInstance<ModLevelObject>();
      dummyHell.IsHell = true;
      dummyHell.BunburrowStyle = AssetsManager.BunburrowsListOfStyles.Hell;
    }

    private ModLevelObject dummyTemple;
    private ModLevelObject dummyHell;
    public override LevelObject LoadLevel(int depth, LoadingContext loadingContext)
    {
      if (depth < HellStartDepth)
        return dummyTemple;
      else
        return dummyHell;
    }
  }
}
