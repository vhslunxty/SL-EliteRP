using System;
using CommandSystem;
using Exiled.API.Features;

namespace EliteRP.Commands
{
    [CommandHandler(typeof(ClientCommandHandler))]
    public sealed class PrinterCommand : ICommand
    {
        public string Command => "printer";
        public string[] Aliases => new[] { "moneyprinter", "mp" };
        public string Description => "Money printer management. Usage: .printer place | .printer collect | .printer upgrade | .printer list";

        public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            if (sender is not CommandSystem.PlayerCommandSender ps)
            { response = "Player-only."; return false; }

            var player = Player.Get(ps.ReferenceHub);
            if (player is null) { response = "Not found."; return false; }

            if (arguments.Count == 0) { response = "Usage: .printer place | .printer collect [id] | .printer upgrade [id] | .printer list"; return false; }

            var sub = arguments.Array![arguments.Offset].ToLower();
            var printers = Plugin.Instance.Printers;

            switch (sub)
            {
                case "place":
                    var ok = printers.Place(player);
                    response = ok ? "Printer placed! ($500)" : "Cannot place printer.";
                    return ok;

                case "collect" when arguments.Count >= 2:
                    if (!int.TryParse(arguments.Array[arguments.Offset + 1], out var cid))
                    { response = "Invalid printer ID."; return false; }
                    printers.Collect(player, cid);
                    response = "Collection attempted.";
                    return true;

                case "collect":
                    var myPrinters = printers.GetPlayerPrinters(player);
                    if (myPrinters.Count == 0) { response = "You have no printers."; return false; }
                    var total = 0;
                    foreach (var p in myPrinters)
                        if (printers.Collect(player, p.Id)) total += p.StoredCash;
                    response = $"Collected from all printers. Total: ${total}";
                    return true;

                case "upgrade" when arguments.Count >= 2:
                    if (!int.TryParse(arguments.Array[arguments.Offset + 1], out var uid))
                    { response = "Invalid printer ID."; return false; }
                    var uok = printers.Upgrade(player, uid);
                    response = uok ? "Printer upgraded!" : "Cannot upgrade printer.";
                    return uok;

                case "list":
                    var list = printers.GetPlayerPrinters(player);
                    if (list.Count == 0) { response = "You own no printers."; return true; }
                    var sb = new System.Text.StringBuilder("Your printers:\n");
                    foreach (var p in list)
                    {
                        var status = p.IsExploded ? "<color=red>EXPLODED</color>" :
                                     p.IsOverheated ? "<color=orange>OVERHEATED</color>" :
                                     "<color=green>RUNNING</color>";
                        sb.AppendLine($"  #{p.Id} L{p.Level} — ${p.StoredCash} — {status}");
                    }
                    response = sb.ToString();
                    return true;

                default:
                    response = "Usage: .printer place | .printer collect [id] | .printer upgrade [id] | .printer list";
                    return false;
            }
        }
    }
}
