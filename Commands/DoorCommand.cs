using System;
using CommandSystem;
using Exiled.API.Features;

namespace EliteRP.Commands
{
    [CommandHandler(typeof(ClientCommandHandler))]
    public sealed class DoorCommand : ICommand
    {
        public string Command => "door";
        public string[] Aliases => new[] { "buydoor", "selldoor" };
        public string Description => "Door ownership commands. Usage: .door buy | .door sell | .door info";

        public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            if (sender is not CommandSystem.PlayerCommandSender ps)
            { response = "Player-only."; return false; }

            var player = Player.Get(ps.ReferenceHub);
            if (player is null) { response = "Not found."; return false; }

            if (arguments.Count == 0) { response = "Usage: .door buy | .door sell | .door info"; return false; }

            var door = player.CurrentRoom?.Doors.FirstOrDefault(d => Vector3.Distance(d.Position, player.Position) < 3f);
            if (door is null) { response = "No door nearby (<3m)."; return false; }

            var doorId = door.GetInstanceID();
            switch (arguments.Array![arguments.Offset].ToLower())
            {
                case "buy":
                    var ok = Plugin.Instance.Doors.Buy(player, door);
                    response = ok ? "Door purchased!" : "Could not purchase door.";
                    return ok;
                case "sell":
                    Plugin.Instance.Doors.Sell(player, doorId);
                    response = "Door sold.";
                    return true;
                case "info":
                    response = Plugin.Instance.Doors.GetDoorInfo(doorId);
                    return true;
                default:
                    response = "Usage: .door buy | .door sell | .door info";
                    return false;
            }
        }
    }
}
