using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bunject.Levels
{
  // Enums representing the context in which we're needing to load or access a level object.
  // Maybe add an enum for Map rendering usage or whatever else later.
  // As long as it's not something significant, "Metadata" should be the value used by default.
  public enum LoadingContext
  {
    Metadata,
    LevelTransition
  }

  // Intended to be used with prefix-postfix pairs that set this appropriately in the relevant functions
  // But for now only hacked in on prefix to LoadLevel to force reloading.
  internal static class CurrentLoadingContext
  {
    public static LoadingContext Value { get; set; } = LoadingContext.Metadata;
  }
}
