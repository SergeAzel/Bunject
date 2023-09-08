﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bunject.Internal
{
  internal class ModBunburrow
  {
    public int ID { get; set; }
    public int ComparisonIndex { get; set; }
    public string Name { get; set; }
    public string Indicator { get; set; }
    public bool IsCustom { get; set; }
    public bool IsVoid { get; set; }
    // make this an HashSet considering only 1 elevator per floor?
    public List<int> Elevators { get; set; } = new List<int>();
  }
}
