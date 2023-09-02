using HarmonyLib;
using Saving.Architecture;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace SaveFileCleanup
{
  [HarmonyPatch(typeof(ProgressSaveData), MethodType.Constructor, new Type[] {
    typeof(double), 
    typeof(ProgressSummary),
    typeof(bool),
    typeof(bool),
    typeof(bool),
    typeof(bool),
    typeof(bool), 
    typeof(List<string>),
    typeof(List<string>),
    typeof(List<LevelIdentitySaveData>),
    typeof(List<LevelIdentitySaveData>),
    typeof(List<BunnyIdentitySaveData>),
    typeof(List<BunnyIdentitySaveData>),
    typeof(List<BunnyIdentitySaveData>),
    typeof(List<BunnyIdentitySaveData>),
    typeof(List<PairSaveData<BunnyIdentitySaveData>>),
    typeof(List<PairSaveData<BunnyIdentitySaveData>>),
    typeof(int),
    typeof(int)})]
  internal class ProgressSaveDataPatches
  {
    /*private static ConstructorInfo TargetMethod()
    {
      return typeof(ProgressSaveData).GetConstructor(typeof(ProgressSaveDataPatches).GetMethod(nameof(Prefix)).GetParameters().Select(p => p.ParameterType).ToArray());
    }*/

    private static void Prefix(double timePlayed,
      ProgressSummary progressSummary,
      bool unlockedPinkBunburrow,
      bool unlockedHayBunburrow,
      bool unlockedAquaticBunburrow,
      bool unlockedGhostlyBunburrow,
      bool unlockedDungeonBunburrow,
      List<string> playedOphelineDialogues,
      List<string> playedOneShotDialogues,
      List<LevelIdentitySaveData> levelsCompletedOnce,
      List<LevelIdentitySaveData> levelsVisited,
      List<BunnyIdentitySaveData> seenBunnies,
      List<BunnyIdentitySaveData> capturedBunnies,
      List<BunnyIdentitySaveData> historyCapturedBunnies,
      List<BunnyIdentitySaveData> homeCapturedBunnies,
      List<PairSaveData<BunnyIdentitySaveData>> existingCouples,
      List<PairSaveData<BunnyIdentitySaveData>> existingGoldenCouples,
      int tallestPileEver,
      int tallestGoldenPileEver)
    {
      // cleanup seen bunnies

      CleanupLevelList(levelsCompletedOnce);
      CleanupLevelList(levelsVisited);
      CleanupBunnyList(seenBunnies);
      CleanupBunnyList(capturedBunnies);
      CleanupBunnyList(historyCapturedBunnies);
      CleanupBunnyList(homeCapturedBunnies);
      CleanupBunnyPairList(existingCouples);
      CleanupBunnyPairList(existingGoldenCouples);
    }

    private static void CleanupLevelList(List<LevelIdentitySaveData> levels)
    {
      foreach (var level in levels.ToList()) 
      {
        if (!Enum.IsDefined(typeof(Bunburrows.Bunburrow), level.Bunburrow))
          levels.Remove(level);
      }
    }

    private static void CleanupBunnyList(List<BunnyIdentitySaveData> bunnies)
    {
      foreach (var bunny in bunnies.ToList())
      {
        if (!Enum.IsDefined(typeof(Bunburrows.Bunburrow), bunny.Bunburrow))
          bunnies.Remove(bunny);
      }
    }

    private static void CleanupBunnyPairList(List<PairSaveData<BunnyIdentitySaveData>> pairs)
    {

      foreach (var pair in pairs.ToList())
      {
        if (!Enum.IsDefined(typeof(Bunburrows.Bunburrow), pair.Left.Bunburrow)
          || !Enum.IsDefined(typeof(Bunburrows.Bunburrow), pair.Right.Bunburrow))
          pairs.Remove(pair);
      }
    }
  }
}
