using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bunject.Archipelago
{
  public static class MissingToolsGenerator
  {
    public static HashSet<string> Generate(bool fullGame)
    {
      return (fullGame ? Full : Credits);
    }

    static HashSet<string> Credits = new HashSet<string>()
    {
      "N-2",
      "N-3",
      "N-4",
      "N-5",
      "N-6",
      "N-7",
      "N-8",
      "N-9",
      "N-10",
      "N-11",

      "C-1",
      "C-2",
      "C-4",
      "C-5",
      "C-6",
      "C-7",
      "C-8",
      "C-9",
      "C-10",
      "C-11",
      "C-12",

      "S-1",
      "S-2",
      "S-3",
      "S-4",
      "S-7",
      "S-8",
      "S-9",
      "S-10",
      "S-11",
      "S-12",

      "E-1",
      "E-2",
      "E-3",
      "E-4",
      "E-5",
      "E-6",
      "E-7",
      "E-8",
      "E-9",
      "E-10",
      "E-11",

      "C-13",
      "C-14",
      "C-15",
      "C-16",
      "C-17",
      "C-18",
      "C-19",

      "W-14",
      "W-15",
      "W-16",
      "W-17",
      "W-18",

      "N-12",
      "N-13",
      "N-14",
      "N-15",
      "N-16",
      "N-17",
      "N-18",

      "S-13",
      "S-14",
      "S-15",
      "S-16",
      "S-17",
      "S-18",
      "S-19"
    };

    static HashSet<string> Full = new HashSet<string>(Credits)
    {
      "S-20",

      "E-16",
      "E-17",
      "E-18",
      "E-19",
      "E-20",
      "E-21",
      "E-22",

      "C-20",
      "C-21",
      "C-22",
      "C-24",

      "W-19",
      "W-20",
      "W-21",

      "N-19",
      "N-21",
      "N-23",

      "S-21",
      "S-22",
      "S-23",
      "S-24",

      "W-23",
      "W-24",
      "W-25",
      "W-26",

      "N-25",
      "N-26",

      "C-25",
      "C-26",

      "S-25",
      "S-26",

      "E-23",
      "E-24",
      "E-25",
      "E-26",
      "E-27"
    };
  }
}
