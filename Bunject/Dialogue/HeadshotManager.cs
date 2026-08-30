using System;
using System.Collections.Generic;
using Dialogue;

namespace Bunject.Dialogue
{
  public static class HeadshotManager
  {
    public static Dictionary<string, HeadshotObject> AllHeadshots = new Dictionary<string, HeadshotObject>();
    private static bool areHeadshotsLoaded = false;

    // Yes it is ugly, but functional
    // Also seemingly the only way to access a few specific headshots
    public static void GetAllHeadshots()
    {
      if (!areHeadshotsLoaded)
      {
        GetHeadshot("PaqueretteIdle", "Moko1", 1);
        GetHeadshot("PaqueretteSmile", "Moko3", 2);
        GetHeadshot("PaqueretteMischievous", "Moko3", 2); // Internal name for PaqueretteSmile
        GetHeadshot("PaqueretteSalute", "Moko3", 2); // Internal name for PaqueretteSmile
        GetHeadshot("PaqueretteHappy", "Moko1", 0);
        GetHeadshot("PaqueretteCute", "Moko1", 0); // Internal name for PaqueretteHappy
        GetHeadshot("PaqueretteKawaii", "Moko1", 0); // Internal name for PaqueretteHappy
        GetHeadshot("PaqueretteSad", "Moko3", 1);
        GetHeadshot("PaqueretteScared", "AccidentallyFellTutorial", 0);
        GetHeadshot("PaqueretteSurprised", "FirstPaqueretteTrespass", 0);
        GetHeadshot("PaqueretteThinking", "AccidentallyFellTutorial", 1);

        GetHeadshot("OphelineIdle", "OphelineIntro", 1);
        GetHeadshot("OphelineFlushed", "OphelineUnknownSecret", 5);
        GetHeadshot("OphelineSigh", "OphelineIdle", 2);
        GetHeadshot("OphelineSmirk", "OphelineIntro2", 4);
        GetHeadshot("OphelineSwirly", "OphelineIntro2", 3);
        GetHeadshot("OphelineHappy", "OphelinePillarsIdle", 3);

        GetHeadshot("Bunbot", "OphelineRobotDoneRecapturing0", 0);
        GetHeadshot("Paquerettoid", "OphelineRobotDoneRecapturing0", 0); // Internal name for Bunbot

        GetHeadshot("HerbeIdle", "HerbeIntro", 6);
        GetHeadshot("HerbeWhistling", "HerbeIntro", 1);
        GetHeadshot("HerbeSurprised", "HerbeIntro", 5);
        GetHeadshot("HerbeWorried", "HerbeIntro2", 3);
        GetHeadshot("HerbeSad", "HerbePowerUnlock", 3);
        GetHeadshot("HerbeSadRedSticker", "HerbeBackOnSurface", 13);
        GetHeadshot("HerbeSmoked", "HerbeFoundInTemple", 2);
        GetHeadshot("HerbeSmokedRedSticker", "HerbeBackOnSurface", 4);

        areHeadshotsLoaded = true;
      }
    }

    public static bool TryResolve(string name, out HeadshotObject headshot)
    {
      if (!string.IsNullOrEmpty(name) && AllHeadshots.TryGetValue(name, out headshot))
        return true;

      AllHeadshots.TryGetValue("PaqueretteIdle", out headshot);
      return false;
    }

    private static void GetHeadshot(string name, string dialogueName, int lineNumber)
    {
      try
      {
        AllHeadshots.Add(name, AssetsManager.Dialogues[dialogueName].DialogueLines[lineNumber].HeadshotObject);
      }
      catch (Exception e)
      {
        UnityEngine.Debug.LogWarning($"Failed to load headshot {name} from {dialogueName}[{lineNumber}]: {e.Message}");
      }
    }
  }
}