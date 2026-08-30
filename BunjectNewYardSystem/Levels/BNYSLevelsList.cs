using Bunject.Dialogue;
using Bunject.Levels;
using Bunject.NewYardSystem.Model;
using Dialogue;
using Levels;
using Misc;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Bunject.NewYardSystem.Levels
{
  public class BNYSLevelsList : ModLevelsList
  {
    public BNYSPlugin Bnys { get; set; }
    public BNYSModBunburrowBase ModBunburrow { get; set; }
    public bool PermitReloading { get; set; } = true;
    public bool DelayReloading { get; set; } = false;
    
    public new BNYSLevelObject this[int depth]
    {
      get { return base[depth] as BNYSLevelObject; }
      set { base[depth] = value; }
    }

    public override LevelObject LoadLevel(int depth, LoadingContext loadingContext)
    {
      if (depth > 0 && depth <= MaximumDepth)
      {
        var level = this[depth];
        if (level == null)
        {
          level = ScriptableObject.CreateInstance<BNYSLevelObject>();
          // Store off the ever-important burrow names and such
          level.BunburrowName = ModBunburrow.BurrowModel.Name;
          level.Depth = depth;

          ReloadLevel(depth, level);

          this[depth] = level;
        }
        else if (PermitReloading && level.ShouldReload && loadingContext == LoadingContext.LevelTransition)
        {
          ReloadLevel(depth, level);
        }

        return level;
      }
      else
      {
        Bnys.Logger.LogWarning($"{ModBunburrow.Name}: Depth requested, {depth}, exceeds burrow depth of {MaximumDepth}");
        return null;
      }
    }

    // Repopulates the levelObject from file contents
    private void ReloadLevel(int depth, BNYSLevelObject levelObject)
    {
      var metadata = ModBunburrow.LoadLevel(depth);

      levelObject.ShouldReload = PermitReloading && (ModBunburrow.World.LiveReloading || metadata.LiveReloading);
      levelObject.LastReloadTime = DateTime.Now;
      levelObject.DelayReload = DelayReloading;

      PopulateLevel(levelObject, metadata, depth);
    }

    private LevelMetadata CreateDefaultLevelMetadata()
    {
      return new LevelMetadata()
      {
        Name = "Failed Level Load",
        LiveReloading = true,
        IsHell = false,
        IsTemple = false,
        Tools = new LevelTools()
      };
    }

    private void PopulateLevel(BNYSLevelObject levelObject, LevelMetadata levelConfig, int depth)
    {
      levelObject.name = $"Level {ModBunburrow.Name} - {levelConfig.Name}";

      levelObject.CustomNameKey = levelConfig.Name;
      levelObject.BunburrowStyle = BNYSPlugin.ResolveStyle(levelConfig.Style);

      if (levelConfig.Tools is LevelTools tools)
      {
        levelObject.NumberOfTraps = tools.Traps;
        levelObject.NumberOfPickaxes = tools.Pickaxes;
        levelObject.NumberOfCarrots = tools.Carrots;
        levelObject.NumberOfShovels = tools.Shovels;
      }
      levelObject.IsTemple = levelConfig.IsTemple;
      levelObject.IsHell = levelConfig.IsHell;

      levelObject.Content = levelConfig.Content;

      if (levelConfig.Hints is List<LevelHint> hints)
        levelObject.Solutions = hints.Select(hint =>
        {
          var solution = new Solution();
          var t = HarmonyLib.Traverse.Create(solution);
          t.Field<Vector2Int>("position").Value = new Vector2Int(hint.PositionX, hint.PositionY);
          t.Field<Misc.Direction>("orientation").Value = hint.Orientation;
          return solution;
        }).ToList();

      levelObject.TeleportPosition = levelConfig.ToTeleportPosition();
      
      levelObject.Dialogues = new List<DialogueObject>();
      levelObject.OphelineDialogues = new List<NPCDialogueObject>();
      levelObject.HerbeDialogues = new List<NPCDialogueObject>();
      
      if (levelConfig.Dialogues != null)
      {
        levelObject.Dialogues = levelConfig.Dialogues.ConvertAll(dialogue => (DialogueObject)dialogue.ConvertToCustomDialogueObject());
      }
      
      if (levelConfig.OphelineDialogues != null)
      {
        levelObject.OphelineDialogues = levelConfig.OphelineDialogues.ConvertAll(dialogue => dialogue.ConvertToNPCDialogueObject());
      }

      if (levelConfig.HerbeDialogues != null)
      {
        levelObject.HerbeDialogues = levelConfig.HerbeDialogues.ConvertAll(dialogue => dialogue.ConvertToNPCDialogueObject());
      }
    }
  }
}