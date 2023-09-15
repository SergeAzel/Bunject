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
    public bool Enabled { get; set; } = true;
    public bool LiveReloading { get; set; } = false;
    public string Title { get; set; } = "Title";
    public List<Burrow> Burrows { get; set; }

    public List<SurfaceEntry> SurfaceEntries { get; set; }

    [JsonIgnore]
    public List<LevelObject> GeneratedSurfaceLevels { get; set; }
  }
}
