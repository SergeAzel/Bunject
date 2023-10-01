using HarmonyLib;
using Levels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tiling.Behaviour;

namespace Bunject.Internal
{
  [HarmonyPatch]
  internal static class LevelBuilderExtensions
  {
    [HarmonyReversePatch]
    [HarmonyPatch(typeof(LevelBuilder), nameof(GetTileInListByCoordinates))]
    public static TileLevelData GetTileInListByCoordinates(List<TileLevelData> tiles, int row, int col)
    {
      throw new NotImplementedException("Implementation overwritten by harmony reverse patch");
    }
  }
}
