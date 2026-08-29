using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Bunject.Utility
{
  // Keeping these in case I need them again later
  internal class DebugUtil
  {
    public static void Trace(string message)
    {
      Debug.Log($"[Bunject.Computer] {message}");
    }

    public static void DumpHierarchy(GameObject obj)
    {
      try
      {
        WholeThing(obj);
      }
      catch (Exception e)
      {
        Debug.LogException(e);
      }
    }

    private static void WholeThing(GameObject obj)
    {
      Debug.Log($"Game Object: {obj.name}  << {obj.GetType().Name} >>");

      foreach (var component in obj.GetComponents<Component>())
      {
        Debug.Log($" - {component.GetType().Name}");

        var tr = Traverse.Create(component);

        foreach (var field in tr.Fields())
        {
          Debug.Log($" - : {field} - {tr.Field(field)}");
        }

        foreach (var prop in tr.Properties())
        {
          Debug.Log($" - : {prop} - {tr.Property(prop)}");
        }
      }


      Debug.Log($"======================= {obj.name} CHILDREN =======================");

      foreach (var child in obj.transform.OfType<Transform>().Select(tr => tr.gameObject))
      {
        WholeThing(child);
      }

      Debug.Log($"!!!!!!!!!!!!!!!!!!!!!!! {obj.name} CHILDREN END !!!!!!!!!!!!!!!!!!!!!!!");
    }
  }
}
