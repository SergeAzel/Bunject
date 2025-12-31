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
  public abstract class BasicCustomComputerTab : CustomComputerTab
  {
    public abstract string GetSpecialText();

    public abstract string GetContentText();

    public sealed override void SelectTab(GameObject specialCounters, TextMeshProUGUI specialCountersText, TextMeshProUGUI contentText)
    {
      var specialText = GetSpecialText();
      specialCounters.SetActive(!string.IsNullOrEmpty(specialText));
      specialCountersText.text = specialText;

      contentText.text = GetContentText();
      contentText.UpdateFontData(1, false);
    }
  }
}
