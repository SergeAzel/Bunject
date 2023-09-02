using HarmonyLib;
using Saving;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Bunject.Internal
{
  public static class SaveFileModUtility
  {
    private static Func<string> getRootSaveDataPathLambda = null;
    public static string GetRootSaveDataPath()
    {
      Initialize();

      return getRootSaveDataPathLambda();
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
