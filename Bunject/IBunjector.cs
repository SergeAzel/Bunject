using Levels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Bunject
{
  public interface IBunjector
  {
    void OnAssetsLoaded();

    void OnProgressionLoaded(GeneralProgression progression);

    LevelsList LoadLevelsList(string name, LevelsList original);

    LevelObject LoadLevel(string listName, int depth, LevelObject original);

    LevelObject LoadSpecialLevel(SpecialLevel levelEnum, LevelObject original);

    LevelObject RappelFromBurrow(string listName, LevelObject otherwise);

    LevelObject StartLevelTransition(LevelObject original, LevelIdentity identity);
  }
}
