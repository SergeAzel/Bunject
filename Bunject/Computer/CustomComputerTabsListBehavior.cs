using Computer.Opheline;
using Computer.Opheline.Tabs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Bunject.Computer
{
  internal class CustomComputerTabsListBehavior : MonoBehaviour, ICustomPageGenerator
  {
    private OphelineComputerTabsList list;
    private int customIndexStart;
    internal void Initialize(OphelineComputerTabsList list)
    {
      this.list = list;
      this.CustomTabs = new List<BasicCustomComputerTabController>();

      BunjectAPI.Forward.GeneratePages(this);

      CreateComputerPage<BasicCustomComputerTabController>();

      customIndexStart = Enum.GetValues(typeof(OphelineComputerTab)).Cast<int>().Max() + 1;
    }

    public List<BasicCustomComputerTabController> CustomTabs { get; private set; }


    public void CreateComputerPage<TBehavior>() where TBehavior : BasicCustomComputerTabController
    {
      var newPage = new GameObject("CustomTab");
      newPage.SetActive(false);
      newPage.transform.parent = gameObject.transform;

      CustomTabs.Add(newPage.AddComponent<TBehavior>());
    }

    public OphelineComputerTab GetCustomControllerIndex(BasicCustomComputerTabController tab)
    {
      return (OphelineComputerTab)CustomTabs.IndexOf(tab) + customIndexStart;
    }

    public OphelineComputerTabController GetByIndex(OphelineComputerTab tab)
    {
      if (((int)tab) < customIndexStart)
      {
        return list[tab];
      }
      return CustomTabs[((int)tab) - (customIndexStart)];
    }

    public IEnumerable<OphelineComputerTabController> GetTabControllers()
    {
      var enumerator = list.GetEnumerator();
      while (enumerator.MoveNext())
      {
        yield return enumerator.Current;
      }

      foreach (var item in CustomTabs)
      {
        yield return item;
      }
    }
  }
}
