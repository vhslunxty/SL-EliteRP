using System;
using CommandSystem;
using Exiled.API.Features;

namespace EliteRP.Commands
{
    /// <summary>
    /// <b>.setname &lt;First Last&gt;</b> — Sets the player's RP identity name.
    /// </summary>
    [CommandHandler(typeof(ClientCommandHandler))]
    public sealed class SetNameCommand : ICommand
    {
        public string Command    => "setname";
        public string[] Aliases  => new[] { "rpname", "identity" };
        public string Description => "Set your RP identity name. Usage: .setname John Doe";

        public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            if (sender is not CommandSystem.PlayerCommandSender playerSender)
            {
                response = "This command can only be used by a player.";
                return false;
            }

            var player = Player.Get(playerSender.ReferenceHub);
            if (player is null)
            {
                response = "Player not found.";
                return false;
            }

            if (arguments.Count == 0)
            {
                response = "Usage: .setname <First Last>";
                return false;
            }

            var name = string.Join(" ", arguments);
            var error = Plugin.Instance.Whitename.SetName(player, name);

            if (error is not null)
            {
                response = $"[EliteRP] {error}";
                return false;
            }

            response = $"[EliteRP] Your RP name has been set to: {name}";
            return true;
        }
    }
}
