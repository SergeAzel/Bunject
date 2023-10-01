using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bunject.NewYardSystem.Model
{
  public class CustomBunburrowModel
  {
    public string World { get; set; }
    public string Name { get; set; }

    public string Prefix { get; set; }
    public string Indicator { get; set; }

    [JsonIgnore]
    public int ID { get; set; }
  }
}
