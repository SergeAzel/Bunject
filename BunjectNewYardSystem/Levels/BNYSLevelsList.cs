using Bunject.Levels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bunject.NewYardSystem.Levels
{
  public class BNYSLevelsList : ModLevelsList
  {
    public new BNYSLevelObject this[int depth]
    {
      get { return base[depth] as BNYSLevelObject; }
      set { base[depth] = value; }
    }
  }
}
