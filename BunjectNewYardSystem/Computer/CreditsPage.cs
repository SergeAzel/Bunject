using Bunject.Computer;
using Bunject.NewYardSystem.Model;
using System;

namespace Bunject.NewYardSystem.Computer
{
  internal class CreditsPage : BasicCustomComputerPageController
  {
    // Set by BNYSPlugin.GeneratePages after the page is created.
    public CustomWorld World { get; set; }

    public override string ButtonName => "BNYSCredits";
    public override string ButtonText => "Level Credits";

    public override bool ShouldShow() => World != null;

    protected override string HeaderText => StartGold() + World?.Title + EndColor();
    protected override string ContentText => GetContentText();

    private string content = null;

    private string GetContentText()
    {
      if (World != null && content == null)
      {
        content = "By" + "  " + StartGold() + GetAuthor() + EndColor() + Environment.NewLine + Environment.NewLine
          + GetDescription(World);
      }

      return content;
    }

    private string GetAuthor()
    {
      if (string.IsNullOrEmpty(World?.Author))
        return "Bunknown Author";
      return World?.Author;
    }


    private static string StartGold()
    {
      return "<color=#D3AF37>";
    }

    private static string EndColor()
    {
      return "</color>";
    }
    
    private static string GetDescription(CustomWorld world)
    {
      return string.IsNullOrWhiteSpace(world.Description)
        ? "A custom world lovingly crafted for your bunjoyment."
        : world.Description;
    }
  }
}
