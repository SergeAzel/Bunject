using BepInEx;
using Bunject.Archipelago.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Bunject.Archipelago.Archipelago;
using Bunject.Menu;

namespace Bunject.Archipelago.UI
{
  internal class ArchipelagoMenu : IMenuSource
  {
    public string Uri { get; private set; } = null;
    public string SlotName { get; private set; } = null;
    public string Password { get; private set; } = null;

    public ArchipelagoMenu(Action onConnect)
    {
      if (onConnect == null)
        throw new ArgumentNullException(nameof(onConnect));

      OnConnect = onConnect;
    }


    private Action OnConnect;

    public string MenuTitle => "Archipelago";
    public void DrawMenuOptions()
    {
      Uri = MakeField("Host: ", Uri);
      SlotName = MakeField("Player Name: ", SlotName);
      Password = MakeField("Pasword: ", Password, true);

      if (!string.IsNullOrEmpty(Uri) && GUILayout.Button("Connect"))
      {
        OnConnect();
      }
    }

    private string MakeField(string label, string value, bool password = false)
    {
      GUILayout.BeginHorizontal();
      GUILayout.Label(label, GUILayout.ExpandWidth(true));
      if (password)
      {
        value = GUILayout.PasswordField(value ?? "", '*', GUILayout.Width(200));
      }
      else
      {
        value = GUILayout.TextField(value, GUILayout.Width(200));
      }
      GUILayout.EndHorizontal();

      return value;
    }

    public void OnAssetsLoaded() { }

    public void OnProgressionLoaded(GeneralProgression progression) { }
  }
}
