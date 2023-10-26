using HarmonyLib;
using Levels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using UnityEngine.UI;
using VisualEffects;

namespace Bunject.Levels
{
	public static class StyleEffectsManager
	{
		public static IModBunburrowStyleEffect None { get; } = new ModStyleEffect();
		public static IModBunburrowStyleEffect Surface { get; } = new ModStyleEffect(SetSurfaceParticleActive);
		public static IModBunburrowStyleEffect Aquatic { get; } = new ModStyleEffect(SetAquaticParticleActive, SetAquaticVisualActive);
		public static IModBunburrowStyleEffect Hay { get; } = new ModStyleEffect(SetHayParticleActive, SetHayVisualActive);
		public static IModBunburrowStyleEffect Ghostly { get; } = new ModStyleEffect(SetGhostlyParticleActive);
		public static IModBunburrowStyleEffect Void { get; } = new ModStyleEffect(null, SetVoidVisualActive);
		public static IModBunburrowStyleEffect Hell { get; } = new ModStyleEffect(SetHellParticleActive, SetHellVisualActive);
		private static void SetSurfaceParticleActive(bool on)
		{
			var surfaceLeavesParticleSystem = Traverse.Create(GameManager.VisualEffectsController).Field<ParticleSystem>("surfaceLeavesParticleSystem").Value;
			if (on)
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
		}
		private static void SetHayParticleActive(bool on)
		{
			var hayParticleSystem = Traverse.Create(GameManager.VisualEffectsController).Field<ParticleSystem>("hayParticleSystem").Value;
			if (on)
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
		}
		private static void SetHayVisualActive(bool on)
		{
			var t = Traverse.Create(GameManager.VisualEffectsController);
			t.Field<GameObject>("hayVisualEffectObject").Value.SetActive(on);
			if (on)
				t.Field<Image>("hayVisualEffectImage").Value.material
					.SetFloat(t.Field<int>("RadiusHash").Value, Mathf.Lerp(1.5f, 0f, Mathf.InverseLerp(1f, 12f,
					(GameManager.CurrentLevel.BaseData as ModLevelObject).Depth)));
		}
		private static void SetAquaticParticleActive(bool on)
		{
			var bubblesParticleSystem = Traverse.Create(GameManager.VisualEffectsController).Field<ParticleSystem>("bubblesParticleSystem").Value;
			if (on)
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
		}
		private static void SetAquaticVisualActive(bool on)
		{
			Traverse.Create(GameManager.VisualEffectsController).Field<GameObject>("aquaticVisualEffectObject").Value.SetActive(on);
		}
		private static void SetGhostlyParticleActive(bool on)
		{
			var ghostsParticleSystem = Traverse.Create(GameManager.VisualEffectsController).Field<ParticleSystem>("ghostsParticleSystem").Value;
			if (on)
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
		}
		private static void SetHellParticleActive(bool on)
		{
			var hellParticleSystem = Traverse.Create(GameManager.VisualEffectsController).Field<ParticleSystem>("hellParticleSystem").Value;
			if (on)
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
		}
		private static void SetHellVisualActive(bool on)
		{
			var t = Traverse.Create(GameManager.VisualEffectsController);
			t.Field<GameObject>("hellVisualEffectObject").Value.SetActive(on);
			t.Field<ScriptableRendererFeature>("heatDazeRendererFeature").Value.SetActive(on);
			var f = t.Field<Coroutine>("hellVignetteCoroutine");
			if (on)
			{
				if (f.Value == null)
				{
					var coroutine = t.Method("DoHellVignetteAnimation").GetValue<System.Collections.IEnumerator>();
					f.Value = GameManager.VisualEffectsController.StartCoroutine(coroutine);
				}
			}
			else
			{
				if (f.Value != null)
				{
					GameManager.VisualEffectsController.StopCoroutine(f.Value);
					f.Value = null;
				}
			}
		}
		private static void SetVoidVisualActive(bool on)
		{
			Traverse.Create(GameManager.VisualEffectsController).Field<ScriptableRendererFeature>("glitchRendererFeature").Value.SetActive(on);
		}
	}
	public class ModStyleEffect : IModBunburrowStyleEffect
	{
		private Action<bool> DefaultAction { get; } = on => { };
		private Action<bool> SetVisualActive { get; }
		private Action<bool> SetParticleActive { get; }
		public ModStyleEffect(Action<bool> setParticleActive = null, Action<bool> setVisualActive = null)
		{
			(SetParticleActive, SetVisualActive) = (setParticleActive ?? DefaultAction, setVisualActive ?? DefaultAction);
		}
		public void SetStyleParticleActive(bool on) => SetParticleActive(on);
		public void SetStyleVisualActive(bool on) => SetVisualActive(on);
	}
	public interface IModBunburrowStyleEffect
	{
		void SetStyleParticleActive(bool on);
		void SetStyleVisualActive(bool on);
	}
}
