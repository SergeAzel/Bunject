using System;
using System.Collections.Generic;
using Dialogue;
using HarmonyLib;

namespace Bunject.Dialogue
{
    public class CustomDialogueLine : DialogueLine
    {
        public CustomDialogueLine()
        {
            Traverse.Create(this).Field("forceTurn").SetValue(false);
        }

        public string Headshot { get; set; }

        public new string Content
        {
            get
            {
                return base.Content;
            }

            set
            {
                Traverse.Create(this).Field("content").SetValue(value);
            }
        }

        public new HeadshotObject HeadshotObject
        {
            get
            {
                return base.HeadshotObject;
            }
            
            set
            {
                Traverse.Create(this).Field("headshotObject").SetValue(value);
            }
        }
    }
}