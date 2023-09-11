using BepInEx;
using Levels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bunject.Levels
{
  public abstract class BaseLevelSourcePlugin : BaseUnityPlugin, ILevelSource
  {
    public abstract void Awake();

    public virtual void OnAssetsLoaded()
    {
    }

    public virtual void OnProgressionLoaded(GeneralProgression progression)
    {
    }

    public virtual ModLevelObject LoadLevel(ModLevelsList levelsList, int index, ModLevelObject original)
    {
      return original;
    }

    public virtual ModLevelsList LoadLevelsList(string bunburrowName, ModLevelsList original)
    {
      return original;
    }

    public virtual LevelObject LoadBurrowSurfaceLevel(string bunburrowName, LevelObject otherwise)
    {
      return otherwise;
    }

    public virtual LevelObject StartLevelTransition(LevelObject original, LevelIdentity identity)
    {
      return original;
    }
  }
}
