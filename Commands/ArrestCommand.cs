using System;
using CommandSystem;
using Exiled.API.Features;

namespace EliteRP.Commands
{
    /// <summary>
    /// <b>.arrest &lt;player&gt;</b> — Police-only client command.
    /// Arrests a nearby wanted player and rewards the officer.
    /// </summary>
    [CommandHandler(typeof(ClientCommandHandler))]
    public sealed class ArrestCommand : ICommand
    {
        public string   Command     => "arrest";
        public string[] Aliases     => new[] { "cuff", "detain" };
        public string   Description => "Arrest a wanted player (Police only). Usage: .arrest <player name>";

        private const float MaxArrestDistance = 4f;

        public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            if (sender is not CommandSystem.PlayerCommandSender ps)
            {
                response = "Player-only command.";
                return false;
            }

            var officer = Player.Get(ps.ReferenceHub);
            if (officer is null) { response = "Player not found."; return false; }

            var job = Plugin.Instance.Jobs.GetJob(officer);
            if (job?.Name != "Police")
            {
                response = "<color=red>[Arrest]</color> Only Police officers can make arrests.";
                return false;
            }

            if (arguments.Count == 0)
            {
                response = "Usage: .arrest <player name>";
                return false;
            }

            var targetName = string.Join(" ", arguments);
            var suspect    = Player.Get(targetName);

            if (suspect is null)
            {
                response = $"[Arrest] No player found: '{targetName}'";
                return false;
            }

            if (suspect == officer)
            {
                response = "[Arrest] You cannot arrest yourself.";
                return false;
            }

            var dist = UnityEngine.Vector3.Distance(officer.Position, suspect.Position);
            if (dist > MaxArrestDistance)
            {
                response = $"[Arrest] You are too far away ({dist:F1}m). Must be within {MaxArrestDistance}m.";
                return false;
            }

            Plugin.Instance.Wanted.Arrest(suspect, officer);
            response = $"[Arrest] You have arrested {suspect.Nickname}.";
            return true;
        }
    }
}
