using Bunject.Levels;
using Levels;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Bunject.NewYardSystem.Model
{
  public class LevelMetadata
  {
    public string Name { get; set; }

    public bool LiveReloading { get; set; } = false;
    public LevelTools Tools { get; set; } = new LevelTools();

    public string Style { get; set; }
    public bool IsTemple { get; set; }
    public bool IsHell { get; set; }

    public string Content { get; set; }

    public bool IsWebLevel { get; set; }

    public int[] Teleport
    {
      get
      {
        if (TeleportX.HasValue && TeleportY.HasValue)
          return new int[] { TeleportX.Value, TeleportY.Value };
        return null; 
      }
      set
      {
        if (value != null && value.Length == 2)
        {
          TeleportX = value[0];
          TeleportY = value[1];
        }
        else
        {
          TeleportX = null;
          TeleportY = null;
        }
      }
    }

    public int? TeleportX { get; set; }
    public int? TeleportY { get; set; }

    public Vector2Int ToTeleportPosition()
    {
      if (TeleportX.HasValue && TeleportY.HasValue)
        return new Vector2Int(TeleportX.Value, TeleportY.Value);
      return new Vector2Int(-1, -1);
    }
  }

  public class LevelTools
  {
    public int Traps { get; set; }
    public int Pickaxes { get; set; }
    public int Carrots { get; set; }
    public int Shovels { get; set; }
  }
}
