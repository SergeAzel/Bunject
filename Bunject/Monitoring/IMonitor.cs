﻿using Levels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bunject.Monitoring
{
  // just kinda a garbage dump of a plugin - a place to put random callbacks that aren't really necessary or belong elsewhere.
  // Aka, event monitoring?
  public interface IMonitor : IBunjectorPlugin
  {
    LevelObject StartLevelTransition(LevelObject level, LevelIdentity identity);
  }
}
