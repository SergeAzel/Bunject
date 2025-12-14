using Bunburrows;
using Levels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Tiling.Behaviour;
using UnityEngine;

namespace Bunject
{
  public interface IBunjectorPlugin
  {
    void OnAssetsLoaded();

    void OnProgressionLoaded(GeneralProgression progression);
  }
}
