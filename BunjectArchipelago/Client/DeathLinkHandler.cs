using Archipelago.MultiClient.Net.BounceFeatures.DeathLink;
using BepInEx;
using Bunject.Archipelago.Client;
using Bunject.Archipelago.UI;
using System;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.UIElements.UxmlAttributeDescription;

namespace Bunject.Archipelago;

public class DeathLinkHandler
{
  private ArchipelagoOptions options;
  private string slotName;
  private readonly DeathLinkService service;
  private TrapHandler trapHandler = null;

  public DeathLinkHandler(DeathLinkService deathLinkService, string name, ArchipelagoOptions options, TrapHandler trapHandler)
  {
    service = deathLinkService;
    slotName = name;
    this.options = options;
    this.trapHandler = trapHandler;

    ArchipelagoConsole.LogMessage("Deathlink starting");
    service.OnDeathLinkReceived += DeathLinkReceived;
    service.EnableDeathLink();
  }

  private void DeathLinkReceived(DeathLink deathLink)
  {
    // Start with the basics.   make it configurable.
    trapHandler.TrapRecieved(options.death_link_behavior);

    ArchipelagoPlugin.BepinLogger.LogMessage(deathLink.Cause.IsNullOrWhiteSpace()
        ? $"Received Death Link from: {deathLink.Source}"
        : deathLink.Cause);
  }
}