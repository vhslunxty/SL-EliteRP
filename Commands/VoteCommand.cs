using System;
using CommandSystem;
using Exiled.API.Features;

namespace EliteRP.Commands
{
    [CommandHandler(typeof(ClientCommandHandler))]
    public sealed class VoteCommand : ICommand
    {
        public string Command => "vote";
        public string[] Aliases => new[] { "votekick", "demote" };
        public string Description => "Vote system. Usage: .vote demote | .vote yes | .vote no";

        public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            if (sender is not CommandSystem.PlayerCommandSender ps)
            { response = "Player-only."; return false; }

            var player = Player.Get(ps.ReferenceHub);
            if (player is null) { response = "Not found."; return false; }

            if (arguments.Count == 0)
            { response = "Usage: .vote demote | .vote yes | .vote no"; return false; }

            var sub = arguments.Array![arguments.Offset].ToLower();
            var votes = Plugin.Instance.Votes;

            switch (sub)
            {
                case "demote":
                    var ok = votes.StartDemoteVote(player);
                    response = ok ? "Vote started!" : "Cannot start vote (check if there's already one or no director).";
                    return ok;

                case "yes":
                    var yok = votes.CastVote(player, true);
                    response = yok ? "Voted YES." : "No active vote.";
                    return yok;

                case "no":
                    var nok = votes.CastVote(player, false);
                    response = nok ? "Voted NO." : "No active vote.";
                    return nok;

                default:
                    response = "Usage: .vote demote | .vote yes | .vote no";
                    return false;
            }
        }
    }
}
