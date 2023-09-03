using Bunburrows;
using Levels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bunject.Internal
{
	// Note: this class can probably be merged
	// Note: it's probably a good idea to have Bunject itself register the elevators
	[Obsolete("This has been refactored into ModBurrows themselves")]
	internal class ModElevatorController
	{
		private const string SEPARATOR = "-";
		// Note: must not contain above string twice, else modify methods that reference the above
		private const string ELEVATOR_SAVE_KEY_FORMAT = "{0}" + SEPARATOR + "{1}";

		private readonly List<string> _customElevators = new List<string>();
		private static ModElevatorController _instance;
		
		public static ModElevatorController Instance
		{
			get
			{
				if (_instance == null)
					_instance = new ModElevatorController();
				return _instance;
			}
		}
		public void RegisterElevator(string indicator, int depth)
		{
			_customElevators.Add(GetElevatorFromLevelIdentity(indicator, depth));
		}

		// This PROBABLY should be using Yard functionalities since it's not a separate dll anymore
		internal bool TryGetLevel(string elevator, out LevelIdentity level)
		{
			level = new LevelIdentity();
			if (!elevator.Contains(SEPARATOR))
				return false;
			var indentity = elevator.Split(new string[] {SEPARATOR}, StringSplitOptions.None);
			if (!int.TryParse(indentity[0], out var burrow))
				return false;
			if (!int.TryParse(indentity[1], out var depth))
				return false;
			if (!_customElevators.Contains(GetElevatorFromLevelIdentity(burrow, depth)))
				return false;
			level = new LevelIdentity((Bunburrow)burrow, depth);
			return true;
		}
		internal bool TryGetElevator(LevelIdentity level, out string elevator)
		{
			if (BunburrowManager.Bunburrows.Any(x => x.ID == (int)level.Bunburrow && x.Elevators.Contains(level.Depth)))
			{
				elevator = GetElevatorSaveKey(level.Bunburrow, level.Depth);
				return true;
			}
			else
			{
				elevator = "";
				return false;
			}
		}
		internal string GetElevatorFromLevelIdentity(LevelIdentity identity)
		{
			return GetElevatorFromLevelIdentity(identity.Bunburrow, identity.Depth);
		}
		internal string GetElevatorFromLevelIdentity(int burrow, int depth)
		{
			return GetElevatorFromLevelIdentity((Bunburrow)burrow, depth);
		}
		internal string GetElevatorFromLevelIdentity(Bunburrow burrow, int depth)
		{
			return GetElevatorFromLevelIdentity(burrow.ToIndicator(), depth);
		}
		internal string GetElevatorFromLevelIdentity(string indicator, int depth)
		{
			const string SHORT_LEVEL_INDICATOR_FORMAT = "{0}-{1}";
			return string.Format(SHORT_LEVEL_INDICATOR_FORMAT, indicator, depth);
		}
		internal string GetElevatorSaveKey(Bunburrow burrow, int depth)
		{
			return GetElevatorSaveKey((int)burrow, depth);
		}
		internal string GetElevatorSaveKey(int burrow, int depth)
		{
			return string.Format(ELEVATOR_SAVE_KEY_FORMAT, burrow, depth);
		}
	}
}
