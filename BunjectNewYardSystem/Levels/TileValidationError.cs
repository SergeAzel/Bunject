using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bunject.NewYardSystem.Levels
{
  public class TileValidationError : LevelValidationError
  {
    public TileValidationError(string message, int row, int column) : base($"Tile on Row {row}, Column {column}: {message}") 
    { }
  }
}
