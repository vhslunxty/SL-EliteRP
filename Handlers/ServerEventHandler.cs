using Exiled.Events.EventArgs.Server;
using ServerEvents = Exiled.Events.Handlers.Server;

namespace EliteRP.Handlers
{
    /// <summary>
    /// Handles server-level Exiled events (round start/end, etc.).
    /// </summary>
    internal sealed class ServerEventHandler
    {
        public void Register()
        {
            ServerEvents.RoundStarted += OnRoundStarted;
            ServerEvents.RoundEnded   += OnRoundEnded;
        }

        public void Unregister()
        {
            ServerEvents.RoundStarted -= OnRoundStarted;
            ServerEvents.RoundEnded   -= OnRoundEnded;
        }

        // ── Handlers ──────────────────────────────────────────────────────────

        private static void OnRoundStarted()
        {
            Exiled.API.Features.Log.Debug("[EliteRP] Round started — restoring player states.");
            foreach (var player in Exiled.API.Features.Player.List)
            {
                Plugin.Instance.Jobs.RestoreJob(player);
                Plugin.Instance.Inventory.RestoreInventory(player);
            }
        }

        private static void OnRoundEnded(RoundEndedEventArgs ev)
        {
            Exiled.API.Features.Log.Debug("[EliteRP] Round ended — flushing data to disk.");
            Plugin.Instance.Economy.SaveAll();
            Plugin.Instance.Whitename.SaveAll();
            Plugin.Instance.Wanted.SaveAll();
            Plugin.Instance.Jobs.SaveAll();
            Plugin.Instance.Inventory.SaveAll();
        }
    }
}
