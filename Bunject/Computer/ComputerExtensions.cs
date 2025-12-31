using Computer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bunject.Computer
{
  public static class ComputerExtensions
  {
    public static CustomComputerTab ToCustom(this ComputerTabController controller)
    {
      return controller.gameObject.GetComponent<CustomComputerTab>();
    }

    public static ComputerTabController ToCore(this CustomComputerTab custom)
    {
      return custom.gameObject.GetComponent<ComputerTabController>();
    }
  }
}
