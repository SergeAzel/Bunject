using Bunject.Levels;
using Bunject.NewYardSystem.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bunject.NewYardSystem.Levels.Local
{
  public class LocalCustomWorld : CustomWorld
  {
    public override BNYSModBunburrowBase GenerateBunburrow(BNYSPlugin pluginRef, string bunburrowName)
    {
      return new BNYSLocalModBunburrow(pluginRef, this, Burrows.First(b => b.Name == bunburrowName)); 
    }
  }
}
