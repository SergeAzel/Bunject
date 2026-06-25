using Computer.Opheline.Tabs;
using HarmonyLib;
using Misc;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;
using TMPro;
using UnityEngine;

namespace Bunject.Computer
{
  public class CustomExtraButton
  {
    public CustomExtraButton(OphelineComputerOptionsTabController tabController, string buttonName, string buttonText, Action onClick)
    {
      Debug.Log($"New Custom Extra Button Construction begins: {buttonName} - '{buttonText}'");
      ChildObject = new GameObject(buttonName);

      // Steal the back button for templating purposes
      var template = Traverse.Create(tabController).Field<TextMeshProUGUI>("backTextComponent").Value.gameObject;
      var templateStyle = template.GetComponent<MenuButtonStyleController>();

      ChildObject.transform.parent = template.transform.parent;
      ChildObject.transform.SetAsLastSibling();
      template.transform.SetAsLastSibling();
      ChildObject.transform.localScale = template.transform.localScale;

      template.transform.SetAsLastSibling();

      Text = ChildObject.AddComponent<TextMeshProUGUI>();
      ButtonController = ChildObject.AddComponent<ButtonController>();

      Text.text = '>' + buttonText;
      Text.ForceMeshUpdate();
      Text.UpdateFontData();

      // Those two come first.  this second.
      Style = ChildObject.AddComponent<MenuButtonStyleController>();
      // Copy the colors
      CopyStyleColors(Style, templateStyle);
      Style.HandleDeselect();

      // Need to ensure that the size is correct
      var rect = ChildObject.GetComponent<RectTransform>();
      rect.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, Text.preferredWidth);
      rect.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, Text.preferredHeight);

      // Set up action on click!
      ButtonController.OnClick += onClick;
    }

    private static void CopyStyleColors(MenuButtonStyleController to, MenuButtonStyleController from)
    {
      const string hoveredColor = nameof(hoveredColor);
      const string selectedColor = nameof(selectedColor);
      const string disabledColor = nameof(disabledColor);

      var toTraverse = Traverse.Create(to);
      var fromTraverse = Traverse.Create(from);

      toTraverse.Field<Color>(hoveredColor).Value = fromTraverse.Field<Color>(hoveredColor).Value;
      toTraverse.Field<Color>(selectedColor).Value = fromTraverse.Field<Color>(selectedColor).Value;
      toTraverse.Field<Color>(disabledColor).Value = fromTraverse.Field<Color>(disabledColor).Value;
    }

    public GameObject ChildObject { get; private set; }

    public MenuButtonStyleController Style { get; private set; }

    public ButtonController ButtonController { get; private set; }

    public TextMeshProUGUI Text { get; set; }
  }
}
