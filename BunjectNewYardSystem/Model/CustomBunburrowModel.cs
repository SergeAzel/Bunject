using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bunject.NewYardSystem.Model
{
  internal class CustomBunburrowModel
  {
    public string Name { get; set; }

    public string Indicator { get; set; }

    public bool IsVoid { get; set; }

    [JsonIgnore]
    public int ID { get; set; }
  }
}
