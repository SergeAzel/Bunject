using BepInEx;
using Bunject.Internal;
using Bunject.Levels;
using Bunject.Tiling;
using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Bunject
{
  // This class is a mess - todo cleanup
  public class BunjectAPI
  {
    static BunjectAPI()
    {
      Instance = new BunjectAPI();

      var harmony = new Harmony("bunject.API");
      harmony.PatchAll(System.Reflection.Assembly.GetAssembly(typeof(BunjectAPI)));
    }

    public static string SaveFolder { get; set; } = null;

    internal static BunjectAPI Instance { get; private set; }

    internal static ForwardingBunjector Forward { get; private set; } = new ForwardingBunjector();

    internal static IReadOnlyList<IBunjectorPlugin> Bunjectors { get => Instance.bunjectors; }

    internal static IEnumerable<ILevelSource> LevelSources { get => Instance.bunjectors.OfType<ILevelSource>(); }

    internal static IEnumerable<ITileSource> TileSources { get => Instance.bunjectors.OfType<ITileSource>(); }

    public static void RegisterPlugin(IBunjectorPlugin bunjector)
    {
      Instance.bunjectors.Add(bunjector);
    }

    public static int RegisterBurrow(ILevelSource levelSource, string name, string indicator, bool isVoid = false)
    {
      return BunburrowManager.RegisterBurrow(levelSource, name, indicator, isVoid);
    }

		public static void RegisterElevator(string indicator, int depth)
    {
      ModElevatorController.Instance.RegisterElevator(indicator, depth);
    }


    private List<IBunjectorPlugin> bunjectors;

    private BunjectAPI()
    {
      bunjectors = new List<IBunjectorPlugin>();
    }
  }
}
