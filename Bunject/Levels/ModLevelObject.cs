using Bunburrows;
using Bunject.Dialogue;
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
  // There is also limited support for NPCs.
  public class ModLevelObject : LevelObject
  {
    public ModLevelObject()
    {
      this.Dialogues = new List<DialogueObject>();
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

    public List<NPCDialogueObject> OphelineDialogues
    {
      get;
      set;
    }

    public List<NPCDialogueObject> HerbeDialogues
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

    public List<DialogueObject> Dialogues
    {
      get { return Traverse.Field<List<DialogueObject>>("dialogues").Value; }
      set { Traverse.Field<List<DialogueObject>>("dialogues").Value = value; }
    }

    public new IReadOnlyList<Solution> Solutions
    {
      get { return base.Solutions; }
      set { Traverse.Field<List<Solution>>("solutions").Value = new List<Solution>(value); }
    }
    public new Vector2Int TeleportPosition
    {
      get { return base.TeleportPosition; }
      set { Traverse.Field<Vector2Int>("teleportPosition").Value = value; }
    }
  }
}
