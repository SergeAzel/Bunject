using BepInEx;
using Bunject.Internal;
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

    internal static IBunjector Forward { get; private set; } = new ForwardingBunjector();

    internal static IReadOnlyList<IBunjector> Bunjectors { get => Instance.bunjectors; }

    public static void Register(IBunjector bunjector)
    {
      Instance.bunjectors.Add(bunjector);
    }
    public static int[] RegisterBurrows((string name, string indicator, bool isVoid)[] burrows, Comparison<string> indicatorCompar)
		{
      return BunburrowManager.RegisterBurrows(burrows, indicatorCompar);
    }
    public static int RegisterBurrow(string name, string indicator, bool isVoid = false)
    {
      return BunburrowManager.RegisterBurrow(name, indicator, isVoid);
    }

		public static void RegisterElevator(string indicator, int depth)
    {
      ModElevatorController.Instance.RegisterElevator(indicator, depth);
    }

    private List<IBunjector> bunjectors;
    private BunjectAPI()
    {
      bunjectors = new List<IBunjector>();
    }
  }
}
