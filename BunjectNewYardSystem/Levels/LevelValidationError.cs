using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bunject.NewYardSystem.Levels
{
  public class LevelValidationError
  {
    public LevelValidationError(string message, bool isWarning)
    {
      Message = message;
      IsWarning = isWarning;
    }

    public LevelValidationError(string message) : this(message, false)
    { }

    public string Message { get; private set; }

    public bool IsWarning { get; private set; }

    public override string ToString()
    {
      return Message;
    }
  }
}
