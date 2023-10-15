using Bunburrows;
using Bunject.Internal;
using Bunject.Levels;
using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using UnityEngine.UI;
using VisualEffects;

namespace Bunject.Patches.VisualEffectsPatches
{
  [HarmonyPatch(typeof(VisualEffects.VisualEffectsController),
    nameof(VisualEffectsController.UpdateVisualEffects),
    new Type[] { })]
  // TODO: make a more sustainable implementation when custom Styles become a thing
  internal class UpdateVisualEffectsPatch
  {
    [HarmonyPriority(Priority.High)]
    private static bool Prefix(bool __runOriginal)
    {
      if (__runOriginal && GameManager.CurrentLevel.BaseData is ModLevelObject level)
      {
        var effects = level.VisualEffects;
        var t = Traverse.Create(GameManager.VisualEffectsController);
        var hay = effects.Contains(AssetsManager.BunburrowsListOfStyles[Bunburrow.Hay]);
        var aquatic = effects.Contains(AssetsManager.BunburrowsListOfStyles[Bunburrow.Aquatic]);
        var ghostly = effects.Contains(AssetsManager.BunburrowsListOfStyles[Bunburrow.Ghostly]);
        var hell = effects.Contains(AssetsManager.BunburrowsListOfStyles.Hell);
        var @void = effects.Contains(AssetsManager.BunburrowsListOfStyles.VoidB);
        if (SettingsManager.HasVisualEffects)
        {
          // this is specifically pillars room vfx
          t.Field<GameObject>("hellTempleVisualEffect").Value.SetActive(false);
          // this is specifically crossroads vfx
          t.Field<GameObject>("templeVisualEffect").Value.SetActive(false);

          t.Field<ScriptableRendererFeature>("glitchRendererFeature").Value.SetActive(@void);

          t.Field<GameObject>("hellVisualEffectObject").Value.SetActive(hell);
          t.Field<ScriptableRendererFeature>("heatDazeRendererFeature").Value.SetActive(hell);
          if (hell)
          {
            var coroutine = t.Method("DoHellVignetteAnimation").GetValue<System.Collections.IEnumerator>();
            t.Field<Coroutine>("hellVignetteCoroutine").Value = GameManager.VisualEffectsController.StartCoroutine(coroutine);
          }
          else
          {
            var coroutine = t.Field<Coroutine>("hellVignetteCoroutine");
            if (coroutine != null)
            {
              GameManager.VisualEffectsController.StopCoroutine(coroutine.Value);
              coroutine.Value = null;
            }
          }
          t.Field<GameObject>("aquaticVisualEffectObject").Value.SetActive(aquatic);
          t.Field<GameObject>("hayVisualEffectObject").Value.SetActive(hay);
          if (hay)
          {
            t.Field<Image>("hayVisualEffectImage").Value.material.SetFloat(t.Field<int>("RadiusHash").Value, Mathf.Lerp(1.5f, 0f, Mathf.InverseLerp(1f, 12f, level.Depth)));
          }
        }
        var surfaceLeavesParticleSystem = t.Field<ParticleSystem>("surfaceLeavesParticleSystem").Value;
        var hayParticleSystem = t.Field<ParticleSystem>("hayParticleSystem").Value;
        var bubblesParticleSystem = t.Field<ParticleSystem>("bubblesParticleSystem").Value;
        var ghostsParticleSystem = t.Field<ParticleSystem>("ghostsParticleSystem").Value;
        var hellParticleSystem = t.Field<ParticleSystem>("hellParticleSystem").Value;
        if (SettingsManager.HasParticles)
        {
          if (level.Depth == 0)
          {
            if (surfaceLeavesParticleSystem.isPlaying)
              surfaceLeavesParticleSystem.Stop();
            surfaceLeavesParticleSystem.Play();
          }
          else if (surfaceLeavesParticleSystem.isPlaying)
          {
            surfaceLeavesParticleSystem.Stop();
            surfaceLeavesParticleSystem.Clear();
          }

          if (hay)
          {
            if (hayParticleSystem.isPlaying)
              hayParticleSystem.Stop();
            hayParticleSystem.Play();
          }
          else if (hayParticleSystem.isPlaying)
          {
            hayParticleSystem.Stop();
            hayParticleSystem.Clear();
          }

          if (aquatic)
          {
            if (bubblesParticleSystem.isPlaying)
              bubblesParticleSystem.Stop();
            bubblesParticleSystem.Play();
          }
          else if (bubblesParticleSystem.isPlaying)
          {
            bubblesParticleSystem.Stop();
            bubblesParticleSystem.Clear();
          }

          if (ghostly)
          {
            if (ghostsParticleSystem.isPlaying)
              ghostsParticleSystem.Stop();
            ghostsParticleSystem.Play();
          }
          else if (ghostsParticleSystem.isPlaying)
          {
            ghostsParticleSystem.Stop();
            ghostsParticleSystem.Clear();
          }

          if (hell)
          {
            if (hellParticleSystem.isPlaying)
              hellParticleSystem.Stop();
            hellParticleSystem.Play();
          }
          else if (hellParticleSystem.isPlaying)
          {
            hellParticleSystem.Stop();
            hellParticleSystem.Clear();
          }
        }
        else
        {
          if (surfaceLeavesParticleSystem.isPlaying)
          {
            surfaceLeavesParticleSystem.Stop();
            surfaceLeavesParticleSystem.Clear();
          }
          if (hayParticleSystem.isPlaying)
          {
            hayParticleSystem.Stop();
            hayParticleSystem.Clear();
          }
          if (bubblesParticleSystem.isPlaying)
          {
            bubblesParticleSystem.Stop();
            bubblesParticleSystem.Clear();
          }
          if (ghostsParticleSystem.isPlaying)
          {
            ghostsParticleSystem.Stop();
            ghostsParticleSystem.Clear();
          }
          if (hellParticleSystem.isPlaying)
          {
            hellParticleSystem.Stop();
            hellParticleSystem.Clear();
          }
        }
        return false;
      }
      return __runOriginal;
    }
  }
}
