using Bunject.Archipelago.Archipelago;
using Bunject.Computer;
using Items;
using Misc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bunject.Archipelago
{
  internal class ProgressComputerTab : BasicCustomComputerTab
  {
    public override string Title => "Arch";

    private ArchipelagoClient client => ArchipelagoPlugin.Instance.ArchipelagoClient;

    public override bool ShouldShow()
    {
      return client != null;
    }

    public override string GetContentText()
    {
      if (client != null)
      {
        if (client.ToolsFound.Any())
        {
          var sortedTools = client.ToolsFound.Where(s => s.Contains('-'))
            .OrderBy(s => Convert.ToInt32(s.Split('-')[1]))
            .GroupBy(s => s[0]).ToDictionary(g => g.Key, g => g.Select(s => s).ToArray());

          var west = sortedTools.ContainsKey('W') ? sortedTools['W'] : [];
          var north = sortedTools.ContainsKey('N') ? sortedTools['N'] : [];
          var center = sortedTools.ContainsKey('C') ? sortedTools['C'] : [];
          var south = sortedTools.ContainsKey('S') ? sortedTools['S'] : [];
          var east = sortedTools.ContainsKey('E') ? sortedTools['E'] : [];

          var max = new[] { west.Length, north.Length, center.Length, east.Length, south.Length }.Max();

          var builder = new StringBuilder();
          builder.Append("Tools Found:" + Environment.NewLine);
          for (var i = 0; i < max; i++)
          {
            builder.Append(PadFive(west, i));
            builder.Append(PadFive(north, i));
            builder.Append(PadFive(center, i));
            builder.Append(PadFive(south, i));
            builder.Append(PadFive(east, i));
            builder.Append(Environment.NewLine);
          }
          return builder.ToString();
        }
        return "No Tools Found!";
      }
      return null;
    }

    private string PadFive(string[] strings, int index)
    {
      if (strings.Length > index)
      {
        return strings[index].PadRight(5, ' ');
      }
      return "     ";
    }

    public override string GetSpecialText()
    {
      if (client != null)
      {
        var bunny = client.Options.home_captures ? FullBunnyIcon() : HalfBunnyIcon();

        switch (client.Options.victory_condition)
        {
          case Client.VictoryCondition.Credits:
            return bunny + " Reach E-12!";
          case Client.VictoryCondition.GoldenBunny:
            return bunny + " Find the " + StartGold() + "Golden Bunny" + EndColor() + "!";
          case Client.VictoryCondition.GoldenFluffle:
            if (client.Options.golden_fluffles < 10)
              return bunny + " Golden Fluffles: " + StartGold() + client.GoldenFluffleCount + EndColor() + " of " + StartGold() + client.Options.golden_fluffles + EndColor();
            else  // Not enough room for 2-digit counts with the bun icon
              return "Golden Fluffles: " + StartGold() + client.GoldenFluffleCount + EndColor() + " of " + StartGold() + client.Options.golden_fluffles + EndColor();
          case Client.VictoryCondition.FullClear:
            return bunny + " Collect All Bunnies!";
        }
      }
      return null;
    }

    private static string StartGold()
    {
      return "<color=#D3AF37>";
    }

    private static string EndColor()
    {
      return "</color>";
    }

    private static string HalfBunnyIcon()
    {
      return StringHelpers.SurroundStringWithBunnySizeTag(StringHelpers.ReplaceVariables("<sprite name=\"$notHomeCapturedBunnyWhite\">", AssetsManager.BunburrowsListOfStyles.GetBunburrowStyleFromID(-1)), inComputer: true);
    }

    private static string FullBunnyIcon()
    {
      return StringHelpers.SurroundStringWithBunnySizeTag(StringHelpers.ReplaceVariables("<sprite name=\"$homeCapturedBunnyWhite\">", AssetsManager.BunburrowsListOfStyles.GetBunburrowStyleFromID(-1)), inComputer: true);
    }
  }
}
