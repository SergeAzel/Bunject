﻿using Bunject.Levels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bunject.Internal
{
  internal class BunburrowMetadata
  {
    public int ID { get; set; }
    public int ComparisonIndex { get; set; }
    public bool IsCustom { get; set; }

    public IModBunburrow ModBunburrow { get; set; }
  }
}