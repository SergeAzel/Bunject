using BepInEx;
using Bunject.Internal;
using Bunject.Levels;
using Bunject.Monitoring;
using Bunject.Tiling;
using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Bunject
{
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

    internal static IEnumerable<ITileSource> TileSources { get => Instance.bunjectors.OfType<ITileSource>(); }

    internal static IEnumerable<IMonitor> Monitors { get => Instance.bunjectors.OfType<IMonitor>(); }

    public static void RegisterPlugin(IBunjectorPlugin bunjector)
    {
      Instance.bunjectors.Add(bunjector);
    }

    public static void RegisterBunburrow(IModBunburrow modBunburrow)
    {
      BunburrowManager.RegisterBurrow(modBunburrow);
    }

    public static void RegisterElevator(int bunburrowID, int depth)
    {
      BunburrowManager.RegisterElevator(bunburrowID, depth);
    }


    private List<IBunjectorPlugin> bunjectors;

    private BunjectAPI()
    {
      bunjectors = new List<IBunjectorPlugin>();
    }
  }
}
