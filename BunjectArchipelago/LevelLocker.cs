using HarmonyLib;
using Levels;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Bunject.Archipelago
{
  public static class LevelLocker
  {
    public static LevelObject Lock(LevelObject source)
    {
      var result = UnityEngine.Object.Instantiate(source);

      var traverse = Traverse.Create(result);

      traverse.Field<int>("numberOfTraps").Value = 0;
      traverse.Field<int>("numberOfPickaxes").Value = 0;
      traverse.Field<int>("numberOfShovels").Value = 0;
      traverse.Field<int>("numberOfCarrots").Value = 0;

      return result;
    }

    public static string AppendLock(string levelTitle)
    {
      var idx = levelTitle.LastIndexOf(">");
      if (idx > 0)
      {
        return levelTitle.Substring(0, idx + 1) + " [LOCKED]";
      }

      return levelTitle.Split(' ')[0] + " [LOCKED]";
    }
  }
}
