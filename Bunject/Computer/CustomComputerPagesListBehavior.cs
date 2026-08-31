using Computer.Opheline;
using Computer.Opheline.Tabs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Bunject.Computer
{
  internal class CustomComputerPagesListBehavior : MonoBehaviour, ICustomPageGenerator
  {
    private OphelineComputerTabsList list;
    private int customIndexStart;

    private OphelineComputerTabController pageTemplate;

    internal void Initialize(OphelineComputerTabsList list)
    {
      this.list = list;
      this.CustomPages = new List<BasicCustomComputerPageController>();

      this.pageTemplate = list[OphelineComputerTab.Captures];

      customIndexStart = Enum.GetValues(typeof(OphelineComputerTab)).Cast<int>().Max() + 1;

      BunjectAPI.Forward.GeneratePages(this);
    }

    public List<BasicCustomComputerPageController> CustomPages { get; private set; }


    public TPage CreateComputerPage<TPage>() where TPage : BasicCustomComputerPageController
    {
      var newPage = new GameObject("CustomPage", typeof(RectTransform));

      newPage.transform.SetParent(pageTemplate.transform.parent, false);

      // Stretch to fill the parent container
      var rect = (RectTransform)newPage.transform;
      rect.anchorMin = Vector2.zero;
      rect.anchorMax = Vector2.one;
      rect.pivot = new Vector2(0.5f, 0.5f);
      rect.offsetMin = Vector2.zero;
      rect.offsetMax = Vector2.zero;
      rect.localScale = Vector3.one;
      rect.localPosition = Vector3.zero;

      var behaviour = newPage.AddComponent<TPage>();
      try
      {
        behaviour.Build(pageTemplate);
      }
      catch
      {
        Destroy(newPage);
        throw;
      }

      CustomPages.Add(behaviour);
      return behaviour;
    }

    public OphelineComputerTab GetCustomControllerIndex(BasicCustomComputerPageController page)
    {
      return (OphelineComputerTab)CustomPages.IndexOf(page) + customIndexStart;
    }

    public OphelineComputerTabController GetByIndex(OphelineComputerTab tab)
    {
      if (((int)tab) < customIndexStart)
      {
        return list[tab];
      }
      return CustomPages[((int)tab) - (customIndexStart)];
    }

    public IEnumerable<OphelineComputerTabController> GetPageControllers()
    {
      var enumerator = list.GetEnumerator();
      while (enumerator.MoveNext())
      {
        yield return enumerator.Current;
      }

      foreach (var item in CustomPages)
      {
        yield return item;
      }
    }
  }
}
