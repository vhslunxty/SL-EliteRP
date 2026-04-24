using System;
using EliteRP.Handlers;
using EliteRP.Systems;
using Exiled.API.Features;

namespace EliteRP
{
    /// <summary>
    /// Entry-point for the EliteRP plugin.
    /// </summary>
    public sealed class Plugin : Plugin<Config>
    {
        // ── Singleton ─────────────────────────────────────────────────────────
        public static Plugin Instance { get; private set; } = null!;

        // ── Metadata ──────────────────────────────────────────────────────────
        public override string Name        => "EliteRP";
        public override string Author      => "EliteRP Team";
        public override string Prefix      => "elite_rp";
        public override Version Version    => new(1, 0, 0);
        public override Version RequiredExiledVersion => new(8, 0, 0);

        // ── Event handlers ────────────────────────────────────────────────────
        private PlayerEventHandler  _playerHandler  = null!;
        private ServerEventHandler  _serverHandler  = null!;

        // ── Systems ───────────────────────────────────────────────────────────
        public  JobSystem            Jobs       { get; private set; } = null!;
        public  EconomySystem        Economy    { get; private set; } = null!;
        public  WhitenameSystem      Whitename  { get; private set; } = null!;
        public  WantedSystem         Wanted     { get; private set; } = null!;
        public  StaffDutySystem      StaffDuty  { get; private set; } = null!;
        public  InventoryPersistence Inventory  { get; private set; } = null!;
        public  AfkSystem            Afk        { get; private set; } = null!;
        public  RpRulesSystem        RpRules    { get; private set; } = null!;

        // ─────────────────────────────────────────────────────────────────────

        public override void OnEnabled()
        {
            Instance = this;

            // Boot all sub-systems (they load their JSON data internally)
            Jobs      = new JobSystem();
            Economy   = new EconomySystem();
            Whitename = new WhitenameSystem();
            Wanted    = new WantedSystem();
            StaffDuty = new StaffDutySystem();
            Inventory = new InventoryPersistence();
            Afk       = new AfkSystem();
            RpRules   = new RpRulesSystem();

            // Register event handlers
            _playerHandler = new PlayerEventHandler();
            _serverHandler = new ServerEventHandler();

            _playerHandler.Register();
            _serverHandler.Register();

            Log.Info($"{Name} v{Version} by {Author} has been enabled.");
            base.OnEnabled();
        }

        public override void OnDisabled()
        {
            _playerHandler.Unregister();
            _serverHandler.Unregister();

            // Flush all data to disk
            Economy.SaveAll();
            Whitename.SaveAll();
            Wanted.SaveAll();
            Jobs.SaveAll();
            Inventory.SaveAll();

            Instance = null!;
            Log.Info($"{Name} has been disabled.");
            base.OnDisabled();
        }
    }
}
