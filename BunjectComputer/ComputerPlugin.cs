using BepInEx;
using Bunject;
using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Bunject.Computer
{
	[BepInPlugin("startup.bunject.computer", "Bunject Computer", "1.2.2")]
	[BepInDependency("sergedev.bunject.newyardsystem", BepInDependency.DependencyFlags.HardDependency)]
	public class ComputerPlugin : BaseUnityPlugin, IBunjectorPlugin
	{
    public void OnAssetsLoaded()
    {
      // Nothing needed
    }

    public void OnProgressionLoaded(GeneralProgression progression)
    {
      progression.HandleMapUnlock();
    }

    private void Awake()
		{
			new Harmony("bunject.computer").PatchAll(Assembly.GetExecutingAssembly());

      BunjectAPI.RegisterPlugin(this);
		}
	}
}
