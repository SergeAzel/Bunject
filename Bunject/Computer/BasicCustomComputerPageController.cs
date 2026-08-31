using Computer.Opheline;
using Computer.Opheline.Tabs;
using HarmonyLib;
using Misc;
using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Bunject.Computer
{
  public class BasicCustomComputerPageController : OphelineComputerTabController
  {
    public virtual string ButtonName => "CustomPage";

    public virtual string ButtonText => "Page";

    //Optional text shown in the fixed header strip
    protected virtual string HeaderText => string.Empty;

    //Text shown in the body
    protected virtual string ContentText => string.Empty;

    // Controls if the button should appear in the extras menu
    public virtual bool ShouldShow() => true;

    private CanvasGroup canvasGroup;
    private ScrollRect scrollRect;

    protected TextMeshProUGUI HeaderTextComponent { get; private set; }
    protected TextMeshProUGUI ContentTextComponent { get; private set; }

    internal void Build(OphelineComputerTabController template)
    {
      canvasGroup = gameObject.AddComponent<CanvasGroup>();
      Traverse.Create(this).Field<CanvasGroup>("canvasGroup").Value = canvasGroup;

      var headerDonor = Traverse.Create(template).Field<TextMeshProUGUI>("countersText").Value;
      var contentDonor = Traverse.Create(template).Field<TextMeshProUGUI>("bunniesText").Value;

      BuildHeader(headerDonor);
      BuildScrollBody(contentDonor);

      canvasGroup.Deactivate();
    }

    private const float TopBorderPad = 4f;   // clear of the CRT border
    private const float HeaderHeight = 16f;  // just over one line at the computer font size
    private const float HeaderToBodyGap = 2f;

    private void BuildHeader(TextMeshProUGUI donor)
    {
      HeaderTextComponent = CloneText("Header", donor, transform);
      HeaderTextComponent.alignment = TextAlignmentOptions.Top;

      var rect = HeaderTextComponent.rectTransform;
      rect.anchorMin = new Vector2(0f, 1f);
      rect.anchorMax = new Vector2(1f, 1f);
      rect.pivot = new Vector2(0.5f, 1f);
      rect.offsetMin = new Vector2(4f, -(TopBorderPad + HeaderHeight)); // left pad; bottom of the strip
      rect.offsetMax = new Vector2(-4f, -TopBorderPad);                 // right pad; top pad clears the border
    }

    private void BuildScrollBody(TextMeshProUGUI donor)
    {
      var scrollGo = new GameObject("ScrollView", typeof(RectTransform));
      var scrollRt = (RectTransform)scrollGo.transform;
      scrollRt.SetParent(transform, false);
      scrollRt.anchorMin = Vector2.zero;
      scrollRt.anchorMax = Vector2.one;
      scrollRt.pivot = new Vector2(0.5f, 0.5f);
      scrollRt.offsetMin = new Vector2(6f, 6f);
      scrollRt.offsetMax = new Vector2(-6f, -(TopBorderPad + HeaderHeight + HeaderToBodyGap)); // just below the header strip
      scrollRt.localScale = Vector3.one;

      var viewportGo = new GameObject("Viewport", typeof(RectTransform), typeof(RectMask2D));
      var viewportRt = (RectTransform)viewportGo.transform;
      viewportRt.SetParent(scrollRt, false);
      viewportRt.anchorMin = Vector2.zero;
      viewportRt.anchorMax = Vector2.one;
      viewportRt.pivot = new Vector2(0f, 1f);
      viewportRt.offsetMin = Vector2.zero;
      viewportRt.offsetMax = Vector2.zero;
      viewportRt.localScale = Vector3.one;

      // Transparent raycast target so mouse wheel / drag reach the ScrollRect.
      var raycastTarget = viewportGo.AddComponent<Image>();
      raycastTarget.color = new Color(0f, 0f, 0f, 0f);
      raycastTarget.raycastTarget = true;

      ContentTextComponent = CloneText("Content", donor, viewportRt);
      ContentTextComponent.alignment = TextAlignmentOptions.TopLeft;

      var contentRt = ContentTextComponent.rectTransform;
      contentRt.anchorMin = new Vector2(0f, 1f);
      contentRt.anchorMax = new Vector2(1f, 1f);
      contentRt.pivot = new Vector2(0.5f, 1f);
      contentRt.sizeDelta = new Vector2(-4f, 0f); // 4px narrower than viewport; height from fitter
      contentRt.anchoredPosition = Vector2.zero;

      var fitter = ContentTextComponent.gameObject.AddComponent<ContentSizeFitter>();
      fitter.horizontalFit = ContentSizeFitter.FitMode.Unconstrained;
      fitter.verticalFit = ContentSizeFitter.FitMode.PreferredSize;

      scrollRect = scrollGo.AddComponent<ScrollRect>();
      scrollRect.viewport = viewportRt;
      scrollRect.content = contentRt;
      scrollRect.horizontal = false;
      scrollRect.vertical = true;
      scrollRect.movementType = ScrollRect.MovementType.Clamped;
      scrollRect.inertia = false;
      scrollRect.scrollSensitivity = 12f;
    }

    private TextMeshProUGUI CloneText(string name, TextMeshProUGUI donor, Transform parent)
    {
      TextMeshProUGUI text;

      if (donor != null)
      {
        text = UnityEngine.Object.Instantiate(donor, parent, false);
        text.name = name;

        foreach (var component in text.GetComponents<Component>())
        {
          if (component is ContentSizeFitter || component is LayoutElement || component is LayoutGroup)
          {
            UnityEngine.Object.Destroy(component);
          }
        }
      }
      else
      {
        // In case we cant get the donor
        text = new GameObject(name, typeof(RectTransform)).AddComponent<TextMeshProUGUI>();
        text.transform.SetParent(parent, false);
      }

      text.gameObject.SetActive(true);
      text.text = string.Empty;

      ApplyComputerFont(text);

      text.richText = true;
      text.textWrappingMode = TextWrappingModes.Normal;
      text.overflowMode = TextOverflowModes.Overflow;
      text.rectTransform.localScale = Vector3.one;

      return text;
    }

    protected void Refresh()
    {
      if (HeaderTextComponent != null)
      {
        var header = HeaderText ?? string.Empty;
        HeaderTextComponent.text = header;
        HeaderTextComponent.gameObject.SetActive(header.Length > 0);
        if (header.Length > 0)
        {
          ApplyComputerFont(HeaderTextComponent);
        }
      }

      if (ContentTextComponent != null)
      {
        ContentTextComponent.text = ContentText ?? string.Empty;
        ApplyComputerFont(ContentTextComponent);
      }
    }

    private static void ApplyComputerFont(TextMeshProUGUI text)
    {
      if (AssetsManager.OphelineComputerFontData != null)
      {
        text.UpdateToDesiredFontData(AssetsManager.OphelineComputerFontData);
      }
    }

    public override void HandleOpen()
    {
      Refresh();
      canvasGroup?.Activate();
      ScrollToTop();
    }

    private void ScrollToTop()
    {
      if (scrollRect == null || ContentTextComponent == null)
      {
        return;
      }

      ContentTextComponent.ForceMeshUpdate();
      Canvas.ForceUpdateCanvases();
      LayoutRebuilder.ForceRebuildLayoutImmediate(scrollRect.content);
      scrollRect.verticalNormalizedPosition = 1f;
    }

    public override void HandleClose()
    {
      canvasGroup?.Deactivate();
    }

    public override void HandleCancel()
    {
      RaiseInternalClose();
    }

    public override void HandleNavigation(Vector2 navigateValue, float timeSinceLastNavigateEvent)
    {
      if (scrollRect == null || Mathf.Abs(navigateValue.y) < 0.3f)
      {
        return;
      }

      var contentHeight = scrollRect.content.rect.height;
      if (contentHeight <= 0f)
      {
        return;
      }

      scrollRect.verticalNormalizedPosition = Mathf.Clamp01(
        scrollRect.verticalNormalizedPosition
        + navigateValue.y * 100f * Time.unscaledDeltaTime / contentHeight);
    }

    public override void HandleSubmit()
    {
      // A plain text page has nothing to submit.
    }
  }
}
