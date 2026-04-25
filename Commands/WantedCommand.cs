using System;
using CommandSystem;
using Exiled.API.Features;

namespace EliteRP.Commands
{
    /// <summary>
    /// <b>RCON/RA: wanted &lt;add|clear&gt; &lt;player&gt; [stars]</b>
    /// Staff command to manage wanted levels from the Remote Admin console.
    /// </summary>
    [CommandHandler(typeof(RemoteAdminCommandHandler))]
    public sealed class WantedCommand : ICommand
    {
        public string   Command     => "wanted";
        public string[] Aliases     => new[] { "setwanted" };
        public string   Description => "Manage a player's wanted level. Usage: wanted add <name> [stars] | wanted clear <name>";

        public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            if (arguments.Count < 2)
            {
                response = "Usage: wanted <add|clear> <player name> [stars]";
                return false;
            }

            var sub    = arguments.Array![arguments.Offset].ToLower();
            var target = Player.Get(arguments.Array[arguments.Offset + 1]);

            if (target is null)
            {
                response = $"No player found: '{arguments.Array[arguments.Offset + 1]}'";
                return false;
            }

            switch (sub)
            {
                case "add":
                {
                    int stars = 1;
                    if (arguments.Count >= 3)
                        int.TryParse(arguments.Array[arguments.Offset + 2], out stars);

                    Plugin.Instance.Wanted.AddWanted(target, stars);
                    response = $"[Wanted] Added {stars}★ to {target.Nickname}. " +
                               $"Total: {Plugin.Instance.Wanted.GetLevel(target)}★";
                    return true;
                }

                case "clear":
                    Plugin.Instance.Wanted.ClearWanted(target);
                    response = $"[Wanted] Cleared wanted level for {target.Nickname}.";
                    return true;

                default:
                    response = "Unknown sub-command. Use: add | clear";
                    return false;
            }
        }
    }
}
