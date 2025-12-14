using Characters.Bunny.Data;
using Levels;
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
    LevelObject OnLevelLoad(LevelObject level, LevelIdentity identity);

    LevelsList LoadEmergencyLevelsList(LevelsList original);

    void OnBunnyCapture(BunnyIdentity bunnyIdentity, bool wasHomeCapture);

    void OnMainMenu();
  }
}
