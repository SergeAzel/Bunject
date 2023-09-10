using BepInEx;
using Bunburrows;
using Levels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tiling.Behaviour;
using UnityEngine;

namespace Bunject
{
  //Defines default intended behavior for a bunjector that does nothing
  public abstract class BaseBunjector : BaseUnityPlugin, IBunjector
  {
    public abstract void Awake();

    public virtual void OnAssetsLoaded()
    {
    }

    public virtual void OnProgressionLoaded(GeneralProgression progression)
    {
    }

    public virtual LevelObject LoadLevel(string listName, int index, LevelObject original)
    {
      return original;
    }

    public virtual LevelsList LoadLevelsList(string name, LevelsList original)
    {
      return original;
    }

    public virtual LevelObject LoadSpecialLevel(SpecialLevel levelEnum, LevelObject original)
    {
      return original;
    }

    public virtual LevelObject RappelFromBurrow(string listName, LevelObject otherwise)
    {
      return otherwise;
    }

    public virtual LevelObject StartLevelTransition(LevelObject original, LevelIdentity identity)
    {
      return original;
    }
	}
}
