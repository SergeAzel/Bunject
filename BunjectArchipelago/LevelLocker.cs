using HarmonyLib;
using Levels;
using System;
using System.Collections.Generic;
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

      var newName = source.CustomNameKey + " Locked";

      var traverse = Traverse.Create(result);
      traverse.Field<string>("customNameKey").Value = newName;

      traverse.Field<int>("numberOfTraps").Value = 0;
      traverse.Field<int>("numberOfPickaxes").Value = 0;
      traverse.Field<int>("numberOfShovels").Value = 0;
      traverse.Field<int>("numberOfCarrots").Value = 0;

      return result;
    }
  }
}
