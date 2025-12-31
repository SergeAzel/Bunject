using Bunburrows;
using Bunject.Levels;
using Levels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Bunject.Archipelago.Utils
{
  internal class ElevatorTrapBunburrow : IModBunburrow
  {
    private static ElevatorTrapBunburrow instance = new ElevatorTrapBunburrow();
    public static ElevatorTrapBunburrow Instance => instance;

    private ElevatorTrapBunburrow() { }

    public int ID { get; set; }

    public string Name => "Elevator Trap Bunburrow";

    public string Indicator => "TRAP";

    public bool IsVoid => false;

    public BunburrowStyle Style => AssetsManager.BunburrowsListOfStyles[Bunburrow.Pink];

    public bool HasEntrance => false;

    public bool HasSign => false;


    LevelsList levels = null;
    public LevelsList GetLevels()
    {
      if (levels == null)
      {
        levels = ScriptableObject.CreateInstance<ElevatorTrapLevelsList>();
      }
      return levels;
    }

    public LevelObject GetSurfaceLevel()
    {
      return AssetsManager.SurfaceRightLevel;
    }

    public Vector2Int? OverrideSignCoordinate()
    {
      return null;
    }
  }
}
