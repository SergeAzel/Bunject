﻿using Bunburrows;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bunject.Internal
{
  internal class BunburrowManager
  {
    private static BunburrowManager instance = null;
    internal static bool IsInitialized => instance != null;
    internal static IReadOnlyList<ModBunburrow> Bunburrows
    {
      get
      {
        if (!IsInitialized)
        {
          instance = new BunburrowManager();
        }
        return instance.bunburrows;
      }
    }

    public static int CustomBunburrowThreshold => 50;

    internal static int RegisterBurrow(string name, string indicator, string style, bool isVoid = false)
    {
      if (Bunburrows.Any(bb => bb.Name == name))
        throw new ArgumentException($"Bunburrow name {name} is already in use!  Please use a unique name.");

      if (Bunburrows.Any(bb => bb.Indicator == indicator))
        throw new ArgumentException($"Bunburrow indicator {name} is already in use!  Please use a unique indicator.");

      var id = ++instance.maxID;

      var bunburrow = new ModBunburrow()
      {
        ID = id,
        Name = name, 
        ComparisonIndex = id,
        IsCustom = true,
        Indicator = indicator,
        // we can now hijack AssetsManager.BunburrowsListOfStyles for burrow coloring
        Style = style,
        IsVoid = isVoid
      };
      instance.bunburrows.Add(bunburrow);

      return id;
    }

    public static BunburrowStyle ResolveStyle(string style)
    {
      switch (style)
      {
        case "Aquatic":
        case "Sunken":
          return AssetsManager.BunburrowsListOfStyles[Bunburrow.Aquatic];
        case "Hay":
          return AssetsManager.BunburrowsListOfStyles[Bunburrow.Hay];
        case "Forgotten":
        case "Purple":
          return AssetsManager.BunburrowsListOfStyles[Bunburrow.Purple];
        case "Spooky":
        case "Ghostly":
          return AssetsManager.BunburrowsListOfStyles[Bunburrow.Ghostly];
        case "Void":
          return AssetsManager.BunburrowsListOfStyles.VoidB;
        case "Temple":
          return AssetsManager.BunburrowsListOfStyles.Temple;
        case "Hell":
          return AssetsManager.BunburrowsListOfStyles.Hell;
        case "HellTemple":
          return AssetsManager.BunburrowsListOfStyles.HellTemple;
        case "Pink":
        default:
          return AssetsManager.BunburrowsListOfStyles[Bunburrow.Pink];
      }
    }

    private BunburrowManager()
    {
      bunburrows = Enum.GetValues(typeof(global::Bunburrows.Bunburrow)).OfType<global::Bunburrows.Bunburrow>().Select(burrowEnum =>
        new ModBunburrow()
        {
          ID = (int)burrowEnum,
          Name = burrowEnum.ToBunburrowName(),
          ComparisonIndex = burrowEnum.ToComparisonIndex(),
          IsCustom = false,
          Indicator = burrowEnum.ToIndicator()
        }).ToList();

      maxID = bunburrows.Max(bb => bb.ID) + CustomBunburrowThreshold; 
      // pad out some space - ids are internally generated in our mods, but we're reusing them for a bunch of different internal identifiers
    }

    private List<ModBunburrow> bunburrows;
    private int maxID;
  }
}
