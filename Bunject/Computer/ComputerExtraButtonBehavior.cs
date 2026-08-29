using Computer.Opheline;
using Computer.Opheline.Tabs;
using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Bunject.Computer
{
  // Gets applied to OphelineComputerOptionsTabController (the Extras menu)
  // Relies upon CustomComputerPagesListBehavior to have our custom pages pre-collected
  public class ComputerExtraButtonBehavior : MonoBehaviour
  {
    private void Awake()
    {
      var canvasController = GetComponentInParent<OphelineComputerCanvasController>();
      var parent = GetComponentInParent<CustomComputerPagesListBehavior>();
      var controller = GetComponent<OphelineComputerOptionsTabController>();

      Buttons = parent.CustomPages.Select(page => new CustomExtraButton(controller, page, () =>
      {
        var index = parent.GetCustomControllerIndex(page);

        Traverse.Create(canvasController).Method("SwitchTab", index, false).GetValue();
      })).ToList();
    }

    public IReadOnlyList<CustomExtraButton> Buttons { get; private set; }
  }
}
