using Bunject.Patches.OphelineComputerCanvasControllerPatches;
using Computer.Opheline;
using Computer.Opheline.Tabs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Bunject.Computer
{
  // Gets applied to OphelineComputerOptionsTabController (the Extras menu)
  // Relies upon CustomComputerTabsListBehavior to have our custom tabs pre-collected
  public class ComputerExtraButtonBehavior : MonoBehaviour
  {
    private void Awake()
    {
      var canvasController = GetComponentInParent<OphelineComputerCanvasController>();
      var parent = GetComponentInParent<CustomComputerTabsListBehavior>();
      var controller = GetComponent<OphelineComputerOptionsTabController>();

      Buttons = parent.CustomTabs.Select(ct => new CustomExtraButton(controller, ct.ButtonName, ct.ButtonText, () =>
      {
        SwitchTab.Invoke(canvasController, parent.GetCustomControllerIndex(ct), false);
      })).ToList();
    }

    public IEnumerable<CustomExtraButton> Buttons { get; private set; }
  }
}
