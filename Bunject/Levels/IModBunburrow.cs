﻿using Bunburrows;
using Levels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bunject.Levels
{
  public interface IModBunburrow
  {
    int ID { get; set; }
    string Name { get; }
    string Indicator { get; }
    bool IsVoid { get; }

    BunburrowStyle Style { get; }

    LevelsList GetLevels();

    LevelObject GetLevel(int depth);

    LevelObject GetSurfaceLevel();
  }
}
