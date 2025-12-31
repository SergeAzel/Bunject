using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Bunject.Menu
{
  internal class MenuDisplay : MonoBehaviour
  {
    // TODO: Menu styling and make it auto-size based on internal content.  For now, this is acceptable :(
    private Rect windowRect = new Rect(20, 20, 0, 0);
    private List<IMenuSource> menuSources = null;
    private int activeMenu = 0;


    private void OnGUI()
    {
      menuSources = BunjectAPI.MenuOptions.ToList();
      if (menuSources.Count > 0)
      {
        activeMenu = activeMenu % menuSources.Count;

        GUILayout.Window(0, windowRect, RenderWindow, "Bunject", GUILayout.MinWidth(400), GUILayout.MinHeight(50));
      }
    }

    private void RenderWindow(int windowId)
    {
      GUILayout.BeginVertical();
      GUILayout.BeginHorizontal();
      if (menuSources.Count > 1)
      {
        if (GUILayout.Button("<"))
        {
          activeMenu = (activeMenu + menuSources.Count - 1) % menuSources.Count;
        }
        GUILayout.Label(menuSources[activeMenu % menuSources.Count].MenuTitle);

        if (GUILayout.Button(">"))
        {
          activeMenu = (activeMenu + 1) % menuSources.Count;
        }
      }
      GUILayout.EndHorizontal();

      menuSources[activeMenu].DrawMenuOptions();

      GUILayout.EndVertical();
    }
  }
}
