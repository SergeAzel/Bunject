using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bunject.Menu
{
  public interface IMenuSource : IBunjectorPlugin
  {
    string MenuTitle { get; }
    void DrawMenuOptions();
  }
}
