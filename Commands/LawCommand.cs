using System;
using CommandSystem;
using Exiled.API.Features;

namespace EliteRP.Commands
{
    [CommandHandler(typeof(ClientCommandHandler))]
    public sealed class LawCommand : ICommand
    {
        public string Command => "law";
        public string[] Aliases => new[] { "laws", "site_director" };
        public string Description => "View or manage laws. Usage: .laws | .law add <text> (Director only) | .law remove <id> | .law reset";

        public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            if (sender is not CommandSystem.PlayerCommandSender ps)
            { response = "Player-only."; return false; }

            var player = Player.Get(ps.ReferenceHub);
            if (player is null) { response = "Not found."; return false; }

            if (arguments.Count == 0)
            {
                Plugin.Instance.Laws.BroadcastLaws(player);
                response = "Laws displayed above.";
                return true;
            }

            var sub = arguments.Array![arguments.Offset].ToLower();
            var laws = Plugin.Instance.Laws;

            switch (sub)
            {
                case "add" when arguments.Count >= 2:
                    var text = string.Join(" ", arguments.Array, arguments.Offset + 1, arguments.Count - 1);
                    var ok = laws.AddLaw(player, text);
                    response = ok ? "Law added." : "You are not the Site Director.";
                    return ok;

                case "remove" when arguments.Count >= 2:
                    if (!int.TryParse(arguments.Array[arguments.Offset + 1], out var id))
                    { response = "Invalid law ID."; return false; }
                    var rok = laws.RemoveLaw(player, id);
                    response = rok ? "Law removed." : "You are not the Site Director or law doesn't exist.";
                    return rok;

                case "reset":
                    laws.ResetLaws(player);
                    response = "Laws reset.";
                    return true;

                case "become_director":
                    laws.SetDirector(player);
                    response = "You are now the Site Director.";
                    return true;

                default:
                    laws.BroadcastLaws(player);
                    response = "Laws displayed.";
                    return true;
            }
        }
    }
}
