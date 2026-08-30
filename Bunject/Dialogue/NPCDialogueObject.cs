using System;
using System.Collections.Generic;
using Characters.Bunny.Data;
using UnityEngine;

namespace Bunject.Dialogue
{
    public class NPCDialogueObject : CustomDialogueObject
    {
        // Basic fields that allow us to set conditions for certain NPC dialogues
        public int RequiredCaptures { get; set; } = 0;
        public int RequiredBabies { get; set; } = 0;
        public int RequiredHomeCaptures { get; set; } = 0;

        // If set to true, the dialogue should only play once per save
        // If set to false, acts similarly to "Idle" dialogues
        public bool ShouldOnlyPlayOnce { get; set; } = false;

        // Each NPCDialogueObject should have a unique name in order for ShouldOnlyPlayOnce to work properly
        public string Name
        {
            get
            {
                return this.name;
            }
            set
            {
                this.name = value;
            }
        }

        public bool AreConditionsSatisfied(GeneralProgression progression)
        {
            return ((progression.HistoryCapturedBunnies.Count >= RequiredCaptures)
                 && (progression.HomeCapturedBunnies.Count >= RequiredHomeCaptures)
                 && (progression.ExistingCouples.Count >= RequiredBabies));
        }
    }
}