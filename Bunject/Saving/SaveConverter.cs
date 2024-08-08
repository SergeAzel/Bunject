using Bunburrows;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text;
using System.Threading.Tasks;

namespace Bunject.Saving
{
  public class SaveConverter
  {
    private Dictionary<int, Bunburrow> bunburrowIDMap;
    public SaveConverter(Dictionary<int, Bunburrow> bunburrowIDMap)
    {
      this.bunburrowIDMap = bunburrowIDMap;
    }

    public bool BunburrowExists(int saveBunburrowID)
    {
      return bunburrowIDMap.ContainsKey(saveBunburrowID);
    }

    public Bunburrow ConvertBunburrow(int saveBunburrowID)
    {
      return bunburrowIDMap[saveBunburrowID];
    }
  }
}
