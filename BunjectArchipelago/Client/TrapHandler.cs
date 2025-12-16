using Archipelago.MultiClient.Net.BounceFeatures.DeathLink;
using Audio;
using Bunject.Archipelago.Archipelago;
using Bunject.Archipelago.UI;
using Bunject.Archipelago.Utils;
using Bunject.Levels;
using HarmonyLib;
using Levels;
using Pause;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.PlayerLoop;

namespace Bunject.Archipelago.Client
{


  public class TrapHandler
  {
    private ArchipelagoClient client;
    private readonly Queue<Trap> trapQueue = new();
    private GameObject trapGameObject;
    private IModBunburrow modBunburrow;
    private ModLevelsList elevatorLevelsList;

    public TrapHandler(ArchipelagoClient client)
    {
      this.client = client;

      modBunburrow = ElevatorTrapBunburrow.Instance;
    }

    public void TrapRecieved(Trap trap)
    {
      trapQueue.Enqueue(trap);

      ArchipelagoConsole.LogMessage("Trap Recieved!");

      if (trapGameObject == null)
      {
        trapGameObject = new GameObject();
        var behavior = trapGameObject.AddComponent<TrapBehavior>();
        behavior.TrapHandler = this;
      }
    }

    private bool IsSafeToTrap()
    {
      return (GeneralInputManager.CanGoBackToSurface)
        && (!GameManager.PauseController.IsPaused);
    }

    public void ActivateTrap()
    {
      if (trapQueue.Count > 0 && IsSafeToTrap())
      {
        var nextTrap = Derandomize(trapQueue.Dequeue());

        switch (nextTrap)
        {
          case Trap.SurfaceTeleport:
            ArchipelagoConsole.LogMessage("Surface Teleport Trap Activated!");
            GameManager.HandleBackToSurfaceFromPause();
            break;
          case Trap.Elevator:
            ArchipelagoConsole.LogMessage("Elevator Trap Activated!");
            ElevatorTrapInternals();
            break;
          default:
            ArchipelagoConsole.LogMessage($"Unknown Trap Enum ({nextTrap})!");
            break;
        }
      }
    }

    private void ElevatorTrapInternals()
    {
      try
      {
        GameManager.SoundsManager.PlayElevatorDingSound();
        MusicManager.StopMusic();
        GameManager.GeneralProgression.HandleElevatorUnlock();
        GameManager.GeneralProgression.HandleLevelExit();
        GameManager.LevelStates.CurrentLevelState.HandlePaqueretteExit(GameManager.PaqueretteController.CurrentTile.Position, GameManager.PaqueretteController.FacedDirection, false);
        GameManager.GeneralProgression.SaveBunniesTransfers();
        GameManagerTraverse.currentBunburrowLevelsList = ElevatorTrapBunburrow.Instance.GetLevels();

        // Prevent skipping :)
        new Traverse(GameManager.GeneralProgression).Property(nameof(GeneralProgression.TookTheElevator)).SetValue(false);
        GameManagerTraverse.StartLevelTransition(AssetsManager.ElevatorLevel, LevelTransitionType.Elevator, MakeFakeIdentity());
      }
      catch (Exception ex)
      {
        ArchipelagoConsole.LogMessage("Elevator Trap: Exception!");
        ArchipelagoPlugin.BepinLogger.LogError(ex);
      }
    }

    private LevelIdentity MakeFakeIdentity()
    {
      return new LevelIdentity((Bunburrows.Bunburrow)ElevatorTrapBunburrow.Instance.ID, client.GetElevatorTrapDepth());
    }

    private static System.Random rng = new System.Random();
    private Trap Derandomize(Trap trap)
    {
      if (trap == Trap.Random)
        return (Trap) 1 + rng.Next() % 2;
      return trap;
    }
  }

  public class TrapBehavior : MonoBehaviour
  {
    public TrapHandler TrapHandler = null;

    public void Awake()
    {
      ArchipelagoConsole.LogMessage("Trap Behavior Activate!");
    }

    public void Update()
    {
      TrapHandler?.ActivateTrap();
    }
  }
}
