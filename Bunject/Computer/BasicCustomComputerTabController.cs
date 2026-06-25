using Bunject.Patches.OphelineComputerCanvasControllerPatches;
using Bunject.Utility;
using Computer.Opheline;
using Computer.Opheline.Tabs;
using HarmonyLib;
using Misc;
using System;
using System.Collections.Generic;
using System.Text;
using TMPro;
using Unity.VectorGraphics;
using UnityEngine;
using UnityEngine.UI;

namespace Bunject.Computer
{
  internal class BasicCustomComputerTabController : OphelineComputerTabController
  {
    public virtual string ButtonName => "ExtraPage";
    public virtual string ButtonText => "Extra Page";
    protected virtual string HeaderText => "Upper Text";
    protected virtual string ContentText => "AAAAAAAAAAAAAA\nBBBBBBBBB\nCCCCCCCCC\nDDDDDD";

    private Traverse<CanvasGroup> traverseCanvasGroup;
    protected CanvasGroup CanvasGroup
    {
      get => traverseCanvasGroup.Value;
      set => traverseCanvasGroup.Value = value;
    }

    public BasicCustomComputerTabController()
    {
      traverseCanvasGroup = Traverse.Create(this).Field<CanvasGroup>("canvasGroup");
    }


    public void Awake()
    {
      var rect = gameObject.AddComponent<RectTransform>();
      var vlayout = gameObject.AddComponent<VerticalLayoutGroup>();
      CanvasGroup = gameObject.AddComponent<CanvasGroup>();

      var tempText = gameObject.AddComponent<TextMeshProUGUI>();
      tempText.text = ContentText;
      tempText.UpdateFontAsset();
      tempText.UpdateFontData();


      // Create children objects
      var mainContainer = new GameObject(gameObject.name + "-MainContainer");
      mainContainer.transform.parent = gameObject.transform;
      var canRenderer = mainContainer.AddComponent<CanvasRenderer>();
      var image = mainContainer.AddComponent<Image>();
      image.sprite = Resources.Load<Sprite>("subMenuOutline2");
      image.type = Image.Type.Tiled;
      image.pixelsPerUnitMultiplier = 1;
      image.fillAmount = 1;
      image.fillClockwise = true;

      var layoutElement = mainContainer.AddComponent<LayoutElement>();

      // mainContainer children 
      var mainContainerContent = new GameObject(gameObject.name + "-MainContainer-Content");
      mainContainerContent.transform.parent = mainContainer.transform;
      var contentRect = mainContainerContent.AddComponent<RectTransform>();
      mainContainerContent.AddComponent<VerticalLayoutGroup>();

      // scrollview
      var mainScroll = new GameObject(gameObject.name + "-MainContainer-Content-Scroll");
      mainScroll.transform.parent = mainContainerContent.transform;
      mainScroll.AddComponent<RectTransform>();
      mainScroll.AddComponent<CanvasRenderer>();
      var scrollRect = mainScroll.AddComponent<ScrollRect>();

      mainScroll.AddComponent<LayoutElement>();

      // viewport
      var viewport = new GameObject(gameObject.name + "-MainContainer-Content-Scroll-Viewport");
      var viewportRect = viewport.AddComponent<RectTransform>();
      viewport.AddComponent<CanvasRenderer>();
      viewport.AddComponent<Image>();
      viewport.AddComponent<Mask>();


      // viewportContent
      var viewportContent = new GameObject(gameObject.name + "-MainContainer-Content-Scroll-Viewport-Content");
      viewportContent.transform.parent = viewport.transform;
      viewportContent.AddComponent<RectTransform>();
      var contentSize = viewportContent.AddComponent<ContentSizeFitter>();
      contentSize.horizontalFit = ContentSizeFitter.FitMode.Unconstrained;
      contentSize.verticalFit = ContentSizeFitter.FitMode.PreferredSize;
      viewportContent.AddComponent<VerticalLayoutGroup>();

      // Finally getting there!!
      var contentHeader = new GameObject(gameObject.name + "-MainContainer-Content-Scroll-Viewport-Content-Header");
      contentHeader.transform.parent = viewportContent.transform;
      contentHeader.AddComponent<RectTransform>();
      contentHeader.AddComponent<CanvasRenderer>();
      var headerText = contentHeader.AddComponent<TextMeshProUGUI>();

      headerText.text = HeaderText;

      var contentText = new GameObject(gameObject.name + "-MainContainer-Content-Scroll-Viewport-Content-Text");
      contentText.transform.parent = viewportContent.transform;
      contentText.AddComponent<RectTransform>();
      contentText.AddComponent<CanvasRenderer>();
      var contentTextText = contentText.AddComponent<TextMeshProUGUI>();
      contentTextText.text = HeaderText;

      scrollRect.viewport = viewportRect;

      DebugUtil.WholeThing(gameObject);
    }


    public override void HandleCancel()
    {
      RaiseInternalClose();
    }

    public override void HandleNavigation(Vector2 navigateValue, float timeSinceLastNavigateEvent)
    {
    }

    public override void HandleSubmit()
    {
      var canvasController = GetComponentInParent<OphelineComputerCanvasController>();
      var tabList = GetComponentInParent<CustomComputerTabsListBehavior>();

      SwitchTab.Invoke(canvasController, tabList.GetCustomControllerIndex(this), false);
    }

    public override void HandleOpen()
    {
      DebugUtil.WholeThing(gameObject);

      CanvasGroup?.Activate();
    }

    public override void HandleClose()
    {
      CanvasGroup?.Deactivate();
    }
  }
}
