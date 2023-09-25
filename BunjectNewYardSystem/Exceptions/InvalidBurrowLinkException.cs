using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bunject.NewYardSystem.Exceptions
{
  public class InvalidBurrowLinkException : Exception
  {
    public InvalidBurrowLinkException(string worldName, string burrowName, string linkDirection, string linkName)
      : base($"{worldName} - {burrowName} has an invalid {linkDirection} burrow link: '{linkName}'")
    { }
  }
}
