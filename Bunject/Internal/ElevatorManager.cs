using Bunburrows;
using Levels;
using Newtonsoft.Json;
using Saving.Architecture;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bunject.Internal
{
	internal class ElevatorManager
  {
    private static ElevatorManager instance = null;
    internal static bool IsInitialized => instance != null;
    internal static IReadOnlyDictionary<LevelIdentity, string> Elevators
    {
      get
      {
        if (!IsInitialized)
        {
          instance = new ElevatorManager();
        }
        return instance.elevatorCache;
      }
    }
    private static bool IsValidSave(string elevatorData)
    {
      return !string.IsNullOrWhiteSpace(elevatorData);
    }
    public static bool IsElevatorUnlock(LevelIdentity level, out string elevatorData)
		{
			return Elevators.TryGetValue(level, out elevatorData) && IsValidSave(elevatorData);
		}
		public static bool IsElevatorUnlock(string elevatorData, out LevelIdentity level)
    {
      if (IsValidSave(elevatorData) && Elevators.Values.Contains(elevatorData))
			{
        level = Elevators.Keys.First(l => Elevators[l] == elevatorData);
        return true;
			}
      level = new LevelIdentity();
      return false;
    }
    public static bool UnlockElevator(Bunburrow burrow, int depth, out string elevatorData)
    {
      return ElevatorUnlock(new LevelIdentity(burrow, depth), out elevatorData);
    }
    public static bool ElevatorUnlock(LevelIdentity level, out string elevatorData)
		{
			if (IsElevatorUnlock(level, out elevatorData))
        return false;
			
			if (!level.Bunburrow.IsCustomBunburrow() || !BunburrowManager.Bunburrows.Any(burrow => burrow.ID == (int)level.Bunburrow && burrow.Elevators.Contains(level.Depth)))
			{
				elevatorData = "";
        if (!instance.elevatorCache.ContainsKey(level))
          instance.elevatorCache.Add(level, elevatorData);
				return false;
			}

			elevatorData = JsonConvert.SerializeObject(level.ProduceSaveData());
			instance.elevatorCache.Add(level, elevatorData);
			return true;
		}
		public static System.Collections.IEnumerator ExtractElevatorProgression()
		{
      if (IsInitialized)
        instance.elevatorCache.Clear();
      foreach (var elevator in GameManager.GeneralProgression.UnlockedElevators)
      {
        if (!IsValidSave(elevator))
          continue;
        if (IsElevatorUnlock(elevator, out _))
          continue;
        LevelIdentity level;
        try
				{
          level = JsonConvert.DeserializeObject<LevelIdentitySaveData>(elevator).BuildLevelIdentity();
        }
				catch
				{
          continue;
				}
        instance.elevatorCache.Add(level, elevator);
      }
      yield break;
    }
		private ElevatorManager()
		{
      elevatorCache = new Dictionary<LevelIdentity, string>();
    }
    private readonly Dictionary<LevelIdentity, string> elevatorCache;
  }
}
