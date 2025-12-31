using Computer;
using HarmonyLib;
using Misc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;

namespace Bunject.Computer
{
  public abstract class CustomComputerTab : MonoBehaviour
  {
    public static CustomComputerTab FindCustomInstance(ComputerTabController controller)
    {
      return controller.gameObject.GetComponent<CustomComputerTab>();
    }

    // Reference to original ComputerTabController
    private Traverse traverse;
    private ButtonController buttonController;
    internal Action selectTab;

    internal void SetCustomTitle(string title)
    {
      traverse.Field<TextMeshProUGUI>("tabTitle").Value.text = title;
      traverse.Field<TextMeshProUGUI>("tabTitle").Value.UpdateFontData();
    }

    public abstract string Title
    {
      get;
    }

    protected virtual void Awake()
    {
      var coreTabController = gameObject.GetComponent<ComputerTabController>();

      buttonController = gameObject.GetComponent<ButtonController>();
      buttonController.OnClick += OnClickWrapper;

      this.traverse = Traverse.Create(coreTabController);
    }

    private void OnClickWrapper()
    {
      selectTab?.Invoke();
    }

    protected virtual void OnDestroy()
    {
      if (buttonController != null)
      {
        buttonController.OnClick -= OnClickWrapper;
        buttonController = null;
      }
    }

    public virtual bool ShouldShow()
    {
      return true;
    }

    public abstract void SelectTab(GameObject specialCounters, TextMeshProUGUI specialCountersText, TextMeshProUGUI contentText);
  }
}
