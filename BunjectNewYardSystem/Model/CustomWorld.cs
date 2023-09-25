using Levels;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bunject.NewYardSystem.Model
{
  public class CustomWorld
  {
    public string ProxyURL { get; set; }
    public bool Enabled { get; set; } = true;
    public bool LiveReloading { get; set; } = false;
    public string Title { get; set; }
    public string Prefix { get; set; }

    public List<Burrow> Burrows { get; set; }

    public List<SurfaceEntry> SurfaceEntries { get; set; }

    [JsonIgnore]
    public Uri ProxyUri { get; set; }

    [JsonIgnore]
    public List<LevelObject> GeneratedSurfaceLevels { get; set; }
  }
}
