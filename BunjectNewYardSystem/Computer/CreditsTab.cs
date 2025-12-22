using Bunject.Computer;
using Bunject.NewYardSystem.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.UIElements;

namespace Bunject.NewYardSystem.Computer
{
  internal class CreditsTab : BasicCustomComputerTab
  {
    // Set on creation in BNYSPlugin.cs
    public CustomWorld World { get; set; }

    public override string Title => "CRED";

    private string content = null;

    public override string GetContentText()
    {
      if (World != null)
      {
        if (content == null)
        {
          content = Center("Author") + Environment.NewLine + StartGold() + Center(GetAuthor()) + EndColor() + Environment.NewLine + Environment.NewLine + World.Description;
        }
      }

      return content;
    }

    public override string GetSpecialText()
    {
      return World?.Title;
    }

    private string GetAuthor()
    {
      if (string.IsNullOrEmpty(World?.Author))
          return "Unattributed";
      return World?.Author;
    }

    private string Center(string text)
    {
      var totalSpace = 24 - (text?.Length ?? 0);

      if (totalSpace <= 0)
        return text;

      return new string(' ', totalSpace / 2) + text + new string(' ', (totalSpace + 1) / 2);
    }

    private static string StartGold()
    {
      return "<color=#D3AF37>";
    }

    private static string EndColor()
    {
      return "</color>";
    }

  }
}
