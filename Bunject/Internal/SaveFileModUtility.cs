using HarmonyLib;
using Saving;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using UnityEngine.SceneManagement;

namespace Bunject.Internal
{
  public static class SaveFileModUtility
  {
    private static Func<string> getRootSaveDataPathLambda = null;
    // Do not rename - matches name in SaveFileManipulationUtility from core
    public static string GetRootSaveDataPath()
    {
      Initialize();

      return getRootSaveDataPathLambda();
    }

    public static string GetPluginSaveDirectory(string pluginName)
    {
      return Path.Combine(GetRootSaveDataPath(), pluginName);
    }

    public static string GetPluginSaveFilePath(string pluginName, string saveName)
    {
      return Path.Combine(GetPluginSaveDirectory(pluginName), saveName + ".bunny");
    }

    public static string GetPluginSaveBackupFilePath(string pluginName, string saveName)
    {
      return Path.Combine(GetPluginSaveDirectory(pluginName), "backup", saveName + ".bunny");
    }

    public static string GetPluginSaveDeletedFilePath(string pluginName, string saveName)
    {
      return Path.Combine(GetPluginSaveDirectory(pluginName), "deleted", saveName + ".bunny");
    }

    public static bool PluginSaveExists(string pluginName, string saveName)
    {
      return File.Exists(GetPluginSaveFilePath(pluginName, saveName));
    }



    private static bool initialized = false;

    private static void Initialize()
    {
      if (!initialized)
      {
        MethodInfo method = typeof(SaveFileManipulationUtility).GetMethod(nameof(GetRootSaveDataPath), BindingFlags.Static | BindingFlags.NonPublic);
        getRootSaveDataPathLambda = Expression.Lambda<Func<string>>(Expression.Call(method, Expression.Constant(true))).Compile();
        initialized = true;
      }
    }
  }
}
