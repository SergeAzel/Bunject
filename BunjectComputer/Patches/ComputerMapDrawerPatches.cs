using Bunject.Computer.Internal;
using Computer;
using HarmonyLib;
using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bunject.Computer.Patches.ComputerMapDrawerPatches
{
  
  [HarmonyPatch(typeof(ComputerMapDrawer), "HandleOpen")]
  internal class HandleOpenPatch
  {
    private static void Prefix() => ModComputerMapDrawer.Instance.HandleOpen();
  }
  [HarmonyPatch(typeof(ComputerMapDrawer), "HandleDownFloorInput")]
  internal class HandleDownFloorInputPatch
  {
    private static bool Prefix(bool __runOriginal) => __runOriginal && ModComputerMapDrawer.Instance.HandleDownFloorInput();
  }
  [HarmonyPatch(typeof(ComputerMapDrawer), "CenterOnStart")]
  internal class CenterOnStartPatch
  {
    private static void Postfix() => ModComputerMapDrawer.Instance.CenterOnStart();
  }
  [HarmonyPatch(typeof(ComputerMapDrawer), "UpdateSelector")]
  internal class UpdateSelectorPatch
  {
    private static void Postfix() => ModComputerMapDrawer.Instance.UpdateSelector();
  }
  [HarmonyPatch(typeof(ComputerMapDrawer), "DrawMapAtDepth")]
  internal class DrawMapAtDepthPatch
  {
    public static bool Prefix(bool __runOriginal, int depth) => __runOriginal && ModComputerMapDrawer.Instance.DrawMapAtDepth(depth);
  }
}
