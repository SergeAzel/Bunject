using Bunject.NewYardSystem.Utility;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bunject.NewYardSystem.Model
{
  public class SurfaceEntry
  {
    public Dictionary<string, SurfaceCoordinate> Coordinates { get; set; }
    public SurfaceEntryGrid Grid { get; set; }
  }

  [JsonConverter(typeof(CoordinateJsonConverter))]
  public class SurfaceCoordinate
  {
    public SurfaceCoordinate() { }
    public SurfaceCoordinate(params int[] holeCoordinates)
    {
      Hole = holeCoordinates;
    }

    public int[] Hole { get; set; }
    public int[] Sign { get; set; }
    public bool NoSign { get; set; }
  }

  public class SurfaceEntryGrid
  {
    public string NW { get; set; }
    public string N { get; set; }
    public string NE { get; set; }
    public string W { get; set; }
    public string C { get; set; }
    public string E { get; set; }
    public string SW { get; set; }
    public string S { get; set; }
    public string SE { get; set; }
  }
}
