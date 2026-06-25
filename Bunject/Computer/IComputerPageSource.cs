using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bunject.Computer
{
  public interface IComputerPageSource : IBunjectorPlugin
  {
    void GeneratePages(ICustomPageGenerator pageMaker);
  }
}
