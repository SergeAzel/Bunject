using HarmonyLib;
using Levels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bunject.Patches.LevelLoaderPatches
{
  [HarmonyPatch(typeof(LevelLoader), "LoadLevel")]
  internal class LoadLevelPatch
  {
    private static readonly string[] Separators = new string[4]
    {
      ",",
      "\r\n",
      "\r",
      "\n"
    };

    private static List<string> Postfix(List<string> __result, LevelObject levelObject)
    {
      //Regex can warn us for problems, but we need to bypass it for the sake of custom levels and custom burrows
      return levelObject.Content.Split(Separators, StringSplitOptions.RemoveEmptyEntries).ToList();
    }
  }
}
