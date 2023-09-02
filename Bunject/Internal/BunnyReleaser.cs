using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bunject.Internal
{
  internal static class BunnyReleaser
  {
    public static void NotifyReleased() 
    { 
      var evnt = Traverse.Create(GameManager.BunniesReleaseAnimator).Field<MulticastDelegate>(nameof(BunniesReleaseAnimator.OnBunnyReleased)).Value;
      foreach (var target in evnt.GetInvocationList())
      {
        target.Method.Invoke(target.Target, new object[] { });
      }
    }
  }
}
