﻿using HarmonyLib;
using Levels;
using Misc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bunject.Levels
{
  public class ModLevelsList : LevelsList 
  {
    public ModLevelsList() 
    { 
      TraverseList.Value = new List<LevelObject>();
      AdjacentBunburrows = new DirectionsListOf<LevelsList>(null, null, null, null);
    }

    private Traverse traverse;
    protected Traverse Traverse
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

    public new int NumberOfRegularBunnies
    {
      get { return base.NumberOfRegularBunnies; }
      set { Traverse.Field<int>("numberOfRegularBunnies").Value = value; }
    }

    public new int NumberOfTempleBunnies
    {
      get { return base.NumberOfTempleBunnies; }
      set { Traverse.Field<int>("numberOfTempleBunnies").Value = value; }
    }

    public new int NumberOfHellBunnies
    {
      get { return base.NumberOfHellBunnies; }
      set { Traverse.Field<int>("numberOfHellBunnies").Value = value; }
    }

    public new DirectionsListOf<LevelsList> AdjacentBunburrows
    {
      get { return base.AdjacentBunburrows; }
      set { Traverse.Field<DirectionsListOf<LevelsList>>("adjacentBunburrows").Value = value; }
    }

    public new int TempleStartDepth
    {
      get { return base.TempleStartDepth; }
      set { Traverse.Field<int>("templeStartDepth").Value = value; }
    }

    public new int HellStartDepth
    {
      get { return base.HellStartDepth; }
      set { Traverse.Field<int>("hellStartDepth").Value = value; }
    }

    private Traverse<List<LevelObject>> traverseList = null;
    private Traverse<List<LevelObject>> TraverseList
    {
      get
      {
        if (traverseList == null)
        {
          traverseList = Traverse.Field<List<LevelObject>>("list");
        }
        return traverseList;
      }
    }

    protected List<LevelObject> List
    {
      get
      {
        return traverseList.Value;
      }
    }

    // Abuse of list and list sizing, but this makes the derived class feel more... reasonable to work with
    // Not that I recommend changing depths at runtime, but...
    public int MaximumDepth
    {
      get { return TraverseList.Value.Count; }
      set 
      {
        var internalList = TraverseList.Value;
        if (value < internalList.Count)
        {
          internalList.RemoveRange(value, internalList.Count - value);
        }
        else
        {
          internalList.AddRange(new LevelObject[value - internalList.Count]);
        }
      }
    }

    // The indexer is currently patched into for injection - avoid that by making our own
    public new ModLevelObject this[int depth]
    {
      get { return TraverseList.Value[depth - 1] as ModLevelObject; }
      set { TraverseList.Value[depth - 1] = value; }
    }

    // To permit us doing strange things with level lists...
    public virtual LevelObject LoadLevel(int depth, LoadingContext loadingContext)
    {
      return this[depth];
    }
  }
}
