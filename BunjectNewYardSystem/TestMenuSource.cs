using Bunject.Menu;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bunject.NewYardSystem
{
  internal class TestMenuSource : IMenuSource
  {
    public string MenuTitle => "Test";

    public void Draw()
    {
      
    }

    public void OnAssetsLoaded()
    {
    }

    public void OnProgressionLoaded(GeneralProgression progression)
    {
    }
  }
}
