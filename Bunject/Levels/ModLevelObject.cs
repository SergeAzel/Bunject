using Bunburrows;
using Dialogue;
using HarmonyLib;
using Levels;
using Misc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Bunject.Levels
{
  // Designed to both differentiate between "core" level objects, and mod-created ones
  // Additionally, includes convenience fields for referencing depth and burrow.
  // Not including Dialogues yet, as we currently have no custom contol over these
  public class ModLevelObject : LevelObject
  {
    public ModLevelObject()
    {
      Traverse.Field("dialogues").SetValue(new List<DialogueObject>());
      Traverse.Field("contextualDialogues").SetValue(new List<ContextualDialogueInfo>());
      SideLevels = new DirectionsListOf<LevelObject>(null, null, null, null);
    }

    public string BunburrowName
    {
      get;
      set;
    }

    public int Depth
    {
      get;
      set;
    }

    public List<BunburrowStyle> VisualEffects
		{
      get;
      set;
		}

    private Traverse traverse;
    private Traverse Traverse
    {
      get
      {
        if (traverse == null)
        {
          traverse = HarmonyLib.Traverse.Create(this);
        }
        return traverse;
      }
    }

    public new string CustomNameKey
    {
      get { return base.CustomNameKey; }
      set { Traverse.Field<string>("customNameKey").Value = value; }
    }

    public new string Content
    {
      get { return base.Content; }
      set { Traverse.Field<string>("content").Value = value; }
    }

    public new int NumberOfTraps
    {
      get { return base.NumberOfTraps; }
      set { Traverse.Field<int>("numberOfTraps").Value = value; }
    }

    public new int NumberOfPickaxes
    {
      get { return base.NumberOfPickaxes; }
      set { Traverse.Field<int>("numberOfPickaxes").Value = value; }
    }

    public new int NumberOfShovels
    {
      get { return base.NumberOfShovels; }
      set { Traverse.Field<int>("numberOfShovels").Value = value; }
    }

    public new int NumberOfCarrots
    {
      get { return base.NumberOfCarrots; }
      set { Traverse.Field<int>("numberOfCarrots").Value = value; }
    }

    public new BunburrowStyle BunburrowStyle
    {
      get { return base.BunburrowStyle; }
      set { Traverse.Field<BunburrowStyle>("bunburrowStyle").Value = value; }
    }

    public new Sprite SpecificBackground
    {
      get { return base.SpecificBackground; }
      set { Traverse.Field<Sprite>("specificBackground").Value = value; }
    }

    public new bool IsTemple
    {
      get { return base.IsTemple; }
      set { Traverse.Field<bool>("isTemple").Value = value; }
    }

    public new bool IsHell
    {
      get { return base.IsHell; }
      set { Traverse.Field<bool>("isHell").Value = value; }
    }

    public new bool IsSurface
    {
      get { return base.IsSurface; }
      set { Traverse.Field<bool>("isSurface").Value = value;  }
    }

    public new DirectionsListOf<LevelObject> SideLevels
    {
      get { return base.SideLevels; }
      set { Traverse.Field<DirectionsListOf<LevelObject>>("sideLevels").Value = value; }
    }
  }
}
