using System;
using CommandSystem;
using Exiled.API.Features;

namespace EliteRP.Commands
{
    [CommandHandler(typeof(ClientCommandHandler))]
    public sealed class ShipmentCommand : ICommand
    {
        public string Command => "shipment";
        public string[] Aliases => new[] { "ship", "buyshipment" };
        public string Description => "Buy weapon/item shipments. Usage: .shipment list | .shipment buy <key>";

        public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            if (sender is not CommandSystem.PlayerCommandSender ps)
            { response = "Player-only."; return false; }

            var player = Player.Get(ps.ReferenceHub);
            if (player is null) { response = "Not found."; return false; }

            if (arguments.Count == 0) { response = "Usage: .shipment list | .shipment buy <key>"; return false; }

            var sub = arguments.Array![arguments.Offset].ToLower();
            var shipments = Plugin.Instance.Shipments;

            switch (sub)
            {
                case "list":
                    var sb = new System.Text.StringBuilder("<color=#00cfff>[Shipments]</color>\n");
                    foreach (var kv in shipments.Shipments)
                    {
                        var s = kv.Value;
                        sb.AppendLine($"  <color=yellow>{s.Key}</color> — {s.Name} — ${s.Price} — x{s.Amount} ({s.ItemType})");
                    }
                    response = sb.ToString();
                    return true;

                case "buy" when arguments.Count >= 2:
                    var key = arguments.Array[arguments.Offset + 1];
                    var ok = shipments.Buy(player, key);
                    response = ok ? $"Bought shipment: {key}" : "Cannot buy shipment.";
                    return ok;

                default:
                    response = "Usage: .shipment list | .shipment buy <key>";
                    return false;
            }
        }
    }
}
