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
		nameof(VisualEffectsController.UpdateVoidVisualEffects))]
	// TODO: make a more sustainable implementation when custom Styles become a thing
	internal class UpdateVoidVisualEffectsPatch
	{
		private static bool Prefix(bool __runOriginal, VisualEffectsController __instance)
		{
			if (__runOriginal && GameManager.CurrentLevel.BaseData is ModLevelObject level)
			{
				if (level.VisualEffects.Contains(AssetsManager.BunburrowsListOfStyles.VoidB) && SettingsManager.HasVisualEffects)
				{
					Traverse.Create(__instance).Field<ScriptableRendererFeature>("glitchRendererFeature").Value.SetActive(true);
				}
				return false;
			}
			return true;
		}
	}

	[HarmonyPatch(typeof(VisualEffects.VisualEffectsController),
    nameof(VisualEffectsController.UpdateVisualEffects),
    new Type[] { })]
  // TODO: make a more sustainable implementation when custom Styles become a thing
  internal class UpdateVisualEffectsPatch
  {
    [HarmonyPriority(Priority.High)]
    private static bool Prefix(bool __runOriginal, VisualEffectsController __instance)
    {
      if (__runOriginal && GameManager.CurrentLevel.BaseData is ModLevelObject level)
      {
        var effects = level.VisualEffects;
        var t = Traverse.Create(__instance);
				var surface = effects.Contains(AssetsManager.SurfaceRightLevel.BunburrowStyle);
				var hay = effects.Contains(AssetsManager.BunburrowsListOfStyles[Bunburrow.Hay]);
        var aquatic = effects.Contains(AssetsManager.BunburrowsListOfStyles[Bunburrow.Aquatic]);
        var ghostly = effects.Contains(AssetsManager.BunburrowsListOfStyles[Bunburrow.Ghostly]);
        var hell = effects.Contains(AssetsManager.BunburrowsListOfStyles.Hell);
        var @void = effects.Contains(AssetsManager.BunburrowsListOfStyles.VoidB);
        var surfaceLeavesParticleSystem = t.Field<ParticleSystem>("surfaceLeavesParticleSystem").Value;
        var hayParticleSystem = t.Field<ParticleSystem>("hayParticleSystem").Value;
        var bubblesParticleSystem = t.Field<ParticleSystem>("bubblesParticleSystem").Value;
        var ghostsParticleSystem = t.Field<ParticleSystem>("ghostsParticleSystem").Value;
        var hellParticleSystem = t.Field<ParticleSystem>("hellParticleSystem").Value;
        if (SettingsManager.HasVisualEffects)
        {
          t.Field<GameObject>("hellTempleVisualEffect").Value.SetActive(false);
          t.Field<GameObject>("templeVisualEffect").Value.SetActive(false);
          t.Field<ScriptableRendererFeature>("glitchRendererFeature").Value.SetActive(@void);
          t.Field<GameObject>("hellVisualEffectObject").Value.SetActive(hell);
          t.Field<ScriptableRendererFeature>("heatDazeRendererFeature").Value.SetActive(hell);
          var f = t.Field<Coroutine>("hellVignetteCoroutine");
          if (hell)
          {
            if (f.Value == null)
            {
              var coroutine = t.Method("DoHellVignetteAnimation").GetValue<System.Collections.IEnumerator>();
              f.Value = __instance.StartCoroutine(coroutine);
            }
          }
          else
          {
            if (f.Value != null)
            {
              __instance.StopCoroutine(f.Value);
              f.Value = null;
            }
          }
          t.Field<GameObject>("hayVisualEffectObject").Value.SetActive(hay);
          if (hay)
            t.Field<Image>("hayVisualEffectImage").Value.material.SetFloat(t.Field<int>("RadiusHash").Value, Mathf.Lerp(1.5f, 0f, Mathf.InverseLerp(1f, 12f, level.Depth)));
          t.Field<GameObject>("aquaticVisualEffectObject").Value.SetActive(aquatic);
        }
				// redefine variables to reduce length of this thing
				surface &= SettingsManager.HasParticles;
				hay &= SettingsManager.HasParticles;
				aquatic &= SettingsManager.HasParticles;
				ghostly &= SettingsManager.HasParticles;
				hell &= SettingsManager.HasParticles;
				// TODO make this a style with associated visual effect
				if (surface)
				{
					if (surfaceLeavesParticleSystem.isPlaying)
					{
						surfaceLeavesParticleSystem.Stop();
					}
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
					{
						hayParticleSystem.Stop();
					}
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
					{
						bubblesParticleSystem.Stop();
					}
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
					{
						ghostsParticleSystem.Stop();
					}
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
					{
						hellParticleSystem.Stop();
					}
					hellParticleSystem.Play();
				}
				else if (hellParticleSystem.isPlaying)
				{
					hellParticleSystem.Stop();
					hellParticleSystem.Clear();
				}
				return false;
      }
      return __runOriginal;
    }
  }
}
