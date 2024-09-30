using Bunject.Levels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bunject.NewYardSystem.Levels
{
  public interface IBNYSModBunburrow
  {
    string WorldName { get; }  
    string LocalName { get; } 
    string WorldPrefix { get; }
    string LocalIndicator { get; }
  }
}
