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
    // overwritten on initial load
    public bool ShouldReload { get; set; } = true;
  }
}
