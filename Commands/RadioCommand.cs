using System;
using CommandSystem;
using Exiled.API.Features;

namespace EliteRP.Commands
{
    /// <summary>
    /// <b>.radio &lt;message&gt;</b> — Broadcasts a message to all players in the same RP job.
    /// </summary>
    [CommandHandler(typeof(ClientCommandHandler))]
    public sealed class RadioCommand : ICommand
    {
        public string   Command     => "radio";
        public string[] Aliases     => new[] { "rc", "jobradio" };
        public string   Description => "Send a radio message to your job faction. Usage: .radio <message>";

        public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            if (sender is not CommandSystem.PlayerCommandSender ps)
            {
                response = "Player-only command.";
                return false;
            }

            var player = Player.Get(ps.ReferenceHub);
            if (player is null) { response = "Player not found."; return false; }

            if (arguments.Count == 0)
            {
                response = "Usage: .radio <message>";
                return false;
            }

            var message = string.Join(" ", arguments);
            var ok = EliteRP.Systems.RadioSystem.Broadcast(player, message);
            response = ok
                ? "[Radio] Message sent to your faction."
                : "[Radio] You don't have an RP job assigned.";
            return ok;
        }
    }
}
