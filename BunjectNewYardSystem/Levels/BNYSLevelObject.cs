using Bunject.Levels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bunject.NewYardSystem.Levels
{
  public class BNYSLevelObject : ModLevelObject
  {
    // overwritten on initial load - keep default to true please
    private bool shouldReload = true;
    public bool ShouldReload
    {
      get { return IsWebLoad ? (shouldReload && (LastReloadTime.Minute + 5 < DateTime.Now.Minute || LastReloadTime.Hour != DateTime.Now.Hour)) : shouldReload; }
      set { shouldReload = value; }
    }

    public DateTime LastReloadTime { get; set; }

    public bool IsWebLoad { get; set; } = false;
  }
}
