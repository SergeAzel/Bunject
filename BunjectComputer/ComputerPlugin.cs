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
	[BepInPlugin("startup.bunject.computer", "Bunject Computer", "0.1.0")]
	[BepInDependency("sergedev.bunject.newyardsystem", BepInDependency.DependencyFlags.SoftDependency)]
	public class ComputerPlugin : BaseUnityPlugin
	{
		public void Awake()
		{
			new Harmony("bunject.computer").PatchAll(Assembly.GetExecutingAssembly());
		}
	}
}
