using Archipelago.MultiClient.Net;
using Archipelago.MultiClient.Net.Enums;
using Archipelago.MultiClient.Net.Helpers;
using Archipelago.MultiClient.Net.Packets;
using Bunburrows;
using Bunject.Archipelago.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Bunject.Archipelago.Archipelago
{
  public class ArchipelagoClient : IDisposable
  {
    public const string GameName = "Paquerette Down The Bunburrows";
    public const string APVersion = "0.5.6";
    public const bool Authenticated = false;

    public string Seed { get; private set; }
    public int Slot { get; private set; }

    public ArchipelagoOptions Options { get; private set; }

    private ArchipelagoSession session;
    private bool disposedValue;

    private HashSet<string> MissingTools = MissingToolsGenerator.Generate();
    private HashSet<string> ToolsFound = new HashSet<string>();
    private Dictionary<string, int> AllItemsFound = new Dictionary<string, int>();

    public static ArchipelagoClient Connect(string hostName, string userName, string password)
    {
      var session = ArchipelagoSessionFactory.CreateSession(hostName);

      var client = new ArchipelagoClient(session);

      var loginResult = session.TryConnectAndLogin(GameName, userName, ItemsHandlingFlags.AllItems, password: password, requestSlotData: true);
      if (loginResult.Successful && loginResult is LoginSuccessful successful)
      {
        client.Options = ArchipelagoOptions.ParseSlotData(successful.SlotData);

        client.Seed = session.RoomState.Seed;
        client.Slot = successful.Slot;

        return client;
      }

      Console.WriteLine("Login Failed oops");

      return null;
    }

    private ArchipelagoClient(ArchipelagoSession session)
    {
      this.session = session;
      session.Items.ItemReceived += OnItemReceieved;
    }

    public bool HasToolsForLevel(string level)
    {
      if (MissingTools.Contains(level))
      {
        return ToolsFound.Contains(level);
      }
      return true;
    }

    public void NotifyBunnyCaptured(string bunny, bool wasHomeCapture)
    {
      var locationId = session.Locations.GetLocationIdFromName(GameName, bunny);

      if (locationId != -1)
      {
        session.Locations.CompleteLocationChecks(locationId);
      }
    }

    public void SendMessage(string message) { }

    private void OnItemReceieved(ReceivedItemsHelper items)
    {
      var itemReceived = items.PeekItem();
      if (itemReceived != null)
      {
        if (itemReceived.ItemGame == GameName)
        {
          if (MissingTools.Contains(itemReceived.ItemName))
          {
            ToolsFound.Add(itemReceived.ItemName);
          }

          if (AllItemsFound.ContainsKey(itemReceived.ItemName))
          {
            AllItemsFound[itemReceived.ItemName] += 1;
          }
          else
          {
            AllItemsFound.Add(itemReceived.ItemName, 1);
          }

          if (Options.victory_condition == VictoryCondition.GoldenFluffle)
          {
            if (itemReceived.ItemName == "Golden Fluffle")
            {
              if (AllItemsFound[itemReceived.ItemName] >= Options.golden_fluffles)
              {
                session.SetGoalAchieved();
              }
            }
          }
        }
      }
      items.DequeueItem();
    }

    #region IDisposable Implementation
    protected virtual void Dispose(bool disposing)
    {
      if (!disposedValue)
      {
        if (disposing) { }

        try
        {
          if (session.Socket.Connected)
          {
            // Note - intentionally not waiting for disconnect
            session.Socket.DisconnectAsync();
          }
        }
        catch { /* Well, we tried */ }

        disposedValue = true;
      }
    }

    // // TODO: override finalizer only if 'Dispose(bool disposing)' has code to free unmanaged resources
    // ~ArchipelagoClient()
    // {
    //     // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
    //     Dispose(disposing: false);
    // }

    public void Dispose()
    {
      // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
      Dispose(disposing: true);
      GC.SuppressFinalize(this);
    }
    #endregion
  }
}
