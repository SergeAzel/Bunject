using Computer;
using HarmonyLib;
using Misc;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;

namespace Bunject.Computer
{
  public class ComputerTabManager : MonoBehaviour
  {
    public static ComputerTabManager instance = null;
    internal static void Instantiate(OphelineComputerCanvasController computer)
    {
      instance = computer.gameObject.GetComponent<ComputerTabManager>();
      if (instance == null || instance.computer != computer)
      {
        instance = computer.gameObject.AddComponent<ComputerTabManager>();
      }
    }

    private OphelineComputerCanvasController computer;
    private Traverse traverse;
    private GameObject originalTab;
    private List<CustomComputerTab> controllers = new List<CustomComputerTab>();

    internal void Awake()
    {
      this.computer = gameObject.GetComponent<OphelineComputerCanvasController>();
      traverse = Traverse.Create(computer);

      // Get the root object containing tabs
      var tabsRoot = GameObject.Find("Header/Tabs");
      if (tabsRoot == null)
        throw new Exception("Cannot find Tab Root Object");

      //probably unnecessary to start at tabsRoot.  Oh well.
      originalTab = tabsRoot.GetComponentInChildren<ComputerTabController>().gameObject;

      BunjectAPI.Forward.GenerateTabs(this);
    }

    internal void OnComputerOpen()
    {
      var tabsToShow = controllers.Where(c => c.ShouldShow()).ToList();

      foreach (var tab in controllers) 
      {
        tab.gameObject.SetActive(tabsToShow.Contains(tab));
        tab.SetCustomTitle(tab.Title);
      }

      var renderingTabList = traverse.Field<List<ComputerTabController>>("availableTabs").Value;
      renderingTabList.AddRange(tabsToShow.Select(t => t.ToCore()).Where(c => !renderingTabList.Contains(c)));
    }

    internal void TabSelected(CustomComputerTab customTab)
    {
      traverse.Field<CanvasGroup>("mapCanvasGroup").Value.Deactivate();

      var specialCountersGameObject = traverse.Field<GameObject>("specialCounters").Value;
      var specialCountersTextComponent = traverse.Field<TextMeshProUGUI>("specialCountersTextComponent").Value;
      var contentText = traverse.Field<TextMeshProUGUI>("contentText").Value;

      customTab.SelectTab(specialCountersGameObject, specialCountersTextComponent, contentText);
    }

    public T CreateTab<T>() where T : CustomComputerTab
    {
      //Clone original tab
      var clone = Instantiate(originalTab);

      clone.transform.SetParent(originalTab.transform.parent);
      clone.transform.localScale = Vector3.one;

      var custom = clone.AddComponent<T>();
      custom.selectTab = () => SelectTab(custom);  // Set the "Button" action

      controllers.Add(custom);

      return custom;
    }

    private List<ComputerTabController> GetInternalAvailableTabs()
    {
      return traverse.Field<List<ComputerTabController>>("availableTabs").Value;
    }

    private void SelectTab(CustomComputerTab customTab)
    {
      var core = customTab.ToCore();
      var availableTabs = GetInternalAvailableTabs();

      var index = availableTabs.IndexOf(core);

      if (index >= 0)
      {
        traverse.Method("SwitchTab", index).GetValue();
      }
    }
  }
}
