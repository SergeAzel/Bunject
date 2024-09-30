using Bunject.Levels;
using Bunject.NewYardSystem.Model;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bunject.NewYardSystem.Levels.Archive
{
  public class ArchiveCustomWorld : CustomWorld
  {
    [JsonIgnore]
    public ZipArchive Archive { get; set; }

    public override BNYSModBunburrowBase GenerateBunburrow(BNYSPlugin pluginRef, string bunburrowName)
    {
      return new BNYSArchiveModBunburrow(pluginRef, this, Burrows.First(b => b.Name == bunburrowName));
    }
  }
}
