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

    public static int RegisterBurrow(string name, string indicator, bool isVoid = false)
    {
      return BunburrowManager.RegisterBurrow(name, indicator, isVoid);
    }

    // [Done]
    // having 3 methods might be a little excessive
    // (considering the ID one should be more than sufficient cause RegisterBurrow returns ID)
    // consider removing Indicator variant but *maybe* not name and refactor both methods to "RegisterElevator"
    public static void RegisterElevator(int id, int depth)
    {
      BunburrowManager.RegisterElevator(id, depth);
    }
    public static void RegisterElevator(string name, int depth)
    {
      BunburrowManager.RegisterElevator(name, depth);
    }

    private List<IBunjector> bunjectors;
    private BunjectAPI()
    {
      bunjectors = new List<IBunjector>();
    }
  }
}
