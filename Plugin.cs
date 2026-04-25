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
        public  F4MenuSystem         F4Menu     { get; private set; } = null!;
        public  DoorSystem           Doors      { get; private set; } = null!;
        public  MoneyPrinterSystem   Printers   { get; private set; } = null!;
        public  ShipmentSystem       Shipments  { get; private set; } = null!;
        public  SalarySystem         Salary     { get; private set; } = null!;
        public  LawSystem            Laws       { get; private set; } = null!;
        public  VoteSystem           Votes      { get; private set; } = null!;
        public  HudSystem            Hud        { get; private set; } = null!;

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
            F4Menu    = new F4MenuSystem();
            Doors     = new DoorSystem();
            Printers  = new MoneyPrinterSystem();
            Shipments = new ShipmentSystem();
            Salary    = new SalarySystem();
            Laws      = new LawSystem();
            Votes     = new VoteSystem();
            Hud       = new HudSystem();

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
            Doors.Save();
            Printers.Save();
            Printers.Dispose();
            Shipments.SaveDefinitions();
            Salary.Save();
            Laws.Save();
            Votes.Save();
            Hud.Dispose();

            Instance = null!;
            Log.Info($"{Name} has been disabled.");
            base.OnDisabled();
        }
    }
}
