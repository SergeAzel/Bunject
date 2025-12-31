using Bunject.Levels;
using Bunject.NewYardSystem.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bunject.NewYardSystem.Levels.Web
{
  public class WebCustomWorld : CustomWorld
  {
    public override BNYSModBunburrowBase GenerateBunburrow(BNYSPlugin pluginRef, string bunburrowName)
    {
      return new BNYSWebModBunburrow(pluginRef, this, Burrows.First(b => b.Name == bunburrowName)); 
    }
  }
}
