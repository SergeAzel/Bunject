using HarmonyLib;
using Levels;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bunject.Archipelago.Utils
{
  internal class GameManagerTraverse
  {
    public static void StartLevelTransition(
      LevelObject levelObject,
      LevelTransitionType levelTransitionType,
      LevelIdentity levelIdentity,
      LevelIdentity? elevatorTargetLevelIdentity = null)
    {
      new Traverse(typeof(GameManager)).Method(nameof(StartLevelTransition), 
        new Type[] { typeof(LevelObject), typeof(LevelTransitionType), typeof(LevelIdentity), typeof(LevelIdentity?) },
        new object[] { levelObject, levelTransitionType, levelIdentity, elevatorTargetLevelIdentity }).GetValue();
    }

    public static LevelsList currentBunburrowLevelsList 
    { 
      get => throw new NotImplementedException();
      set => new Traverse(typeof(GameManager)).Field<LevelsList>(nameof(currentBunburrowLevelsList)).Value = value;
    }
  }
}
