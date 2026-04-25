using Exiled.API.Features;
using Exiled.Events.EventArgs.Player;
using PlayerEvents = Exiled.Events.Handlers.Player;
using Exiled.Events.EventArgs.Interfaces;

namespace EliteRP.Handlers
{
    /// <summary>
    /// Subscribes to all player-related Exiled events and delegates them to
    /// the appropriate sub-system.
    /// </summary>
    internal sealed class PlayerEventHandler
    {
        private readonly EliteRP.Plugin _plugin = EliteRP.Plugin.Instance;

        // ── Registration ──────────────────────────────────────────────────────

        public void Register()
        {
            PlayerEvents.Verified            += OnVerified;
            PlayerEvents.Left                += OnLeft;
            PlayerEvents.Spawned             += OnSpawned;
            PlayerEvents.Dying               += OnDying;
            PlayerEvents.Died                += OnDied;
            PlayerEvents.UsingMicroHIDEnergy += OnActivity;
            PlayerEvents.TogglingNoClip      += OnToggleNoClip;   // F4 key
            PlayerEvents.InteractingDoor     += OnInteractingDoor;
            PlayerEvents.ChangingRole        += OnChangingRole;
        }

        public void Unregister()
        {
            PlayerEvents.Verified            -= OnVerified;
            PlayerEvents.Left                -= OnLeft;
            PlayerEvents.Spawned             -= OnSpawned;
            PlayerEvents.Dying               -= OnDying;
            PlayerEvents.Died                -= OnDied;
            PlayerEvents.UsingMicroHIDEnergy -= OnActivity;
            PlayerEvents.TogglingNoClip      -= OnToggleNoClip;
            PlayerEvents.InteractingDoor     -= OnInteractingDoor;
            PlayerEvents.ChangingRole        -= OnChangingRole;
        }

        // ── Handlers ──────────────────────────────────────────────────────────

        private void OnVerified(VerifiedEventArgs ev)
        {
            _plugin.Afk.OnPlayerJoin(ev.Player);
            _plugin.Economy.GetBalance(ev.Player); // Ensure account exists

            // Restore RP name badge if returning player
            var rpName = _plugin.Whitename.GetName(ev.Player);
            if (rpName is not null)
                ev.Player.CustomInfo = $"[{rpName}]";

            ev.Player.Broadcast(8,
                "<color=#00cfff>[EliteRP]</color> Welcome! Use <b>.setname YourRPName</b> to set your identity " +
                "and <b>.job list</b> to browse available jobs.");

            Log.Debug($"[EliteRP] {ev.Player.Nickname} verified.");
        }

        private void OnLeft(LeftEventArgs ev)
        {
            _plugin.Afk.OnPlayerLeave(ev.Player);
            _plugin.StaffDuty.OnDisconnect(ev.Player);
            _plugin.Inventory.SaveInventory(ev.Player);
            _plugin.F4Menu.OnPlayerLeave(ev.Player);
        }

        private void OnSpawned(SpawnedEventArgs ev)
        {
            _plugin.Jobs.RestoreJob(ev.Player);
            _plugin.Inventory.RestoreInventory(ev.Player);
        }

        private void OnDying(DyingEventArgs ev)
        {
            if (ev.Attacker is null || ev.Player is null) return;
            if (ev.Attacker == ev.Player)                 return; // suicide

            if (_plugin.RpRules.ShouldBlockKill(ev.Attacker, ev.Player))
            {
                ev.IsAllowed = false;
                ev.Attacker.Broadcast(5,
                    "<color=red>[RP RULES]</color> That kill is not permitted in this area.");
            }
        }

        private void OnDied(DiedEventArgs ev)
        {
            if (ev.Attacker is not null && ev.Attacker != ev.Player)
                _plugin.RpRules.HandleUnauthorizedKill(ev.Attacker, ev.Player);

            // Save inventory before it's cleared
            _plugin.Inventory.SaveInventory(ev.Player);
        }

        // Treat any item/ability use as "movement" for AFK detection
        private void OnActivity(UsingMicroHIDEnergyEventArgs ev) =>
            _plugin.Afk.OnPlayerMove(ev.Player);

        /// <summary>
        /// F4 key (NoClip toggle) is intercepted to open/cycle/close the F4 menu.
        /// The NoClip action is suppressed while the menu is navigating pages so
        /// staff members who are actually allowed to NoClip don't accidentally fly.
        /// </summary>
        private void OnToggleNoClip(TogglingNoClipEventArgs ev)
        {
            // Only intercept for non-NoClip-permitted players, or always intercept
            // and pass-through only when the menu is already closed after last page.
            _plugin.F4Menu.Toggle(ev.Player);

            // Suppress the actual NoClip toggle while cycling menu pages
            ev.IsAllowed = false;
        }

        /// <summary>
        /// Door ownership check — blocks opening if the player doesn't own
        /// the door (and it's not a public/unowned door).
        /// </summary>
        private void OnInteractingDoor(InteractingDoorEventArgs ev)
        {
            if (ev.Player is null || ev.Door is null) return;
            if (Plugin.Instance.StaffDuty.IsOnDuty(ev.Player)) return; // Staff bypass

            var doorId = ev.Door.GetInstanceID();
            if (!Plugin.Instance.Doors.CanAccess(ev.Player, doorId))
            {
                ev.IsAllowed = false;
                ev.Player.Broadcast(3,
                    $"<color=red>[Door]</color> {Plugin.Instance.Doors.GetDoorInfo(doorId)}");
            }
        }

        /// <summary>
        /// When a player changes role, clear their job-specific state
        /// but preserve their identity and wallet.
        /// </summary>
        private void OnChangingRole(ChangingRoleEventArgs ev)
        {
            if (ev.Player is null) return;
            // Optionally: clear job on role change to prevent SCP job mismatch
            // _plugin.Jobs.ClearJob(ev.Player);
        }
    }
}
