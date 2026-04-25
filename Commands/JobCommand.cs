using System;
using System.Text;
using CommandSystem;
using Exiled.API.Features;

namespace EliteRP.Commands
{
    /// <summary>
    /// <b>.job &lt;list|set|clear&gt;</b> — Browse and apply RP jobs.
    /// </summary>
    [CommandHandler(typeof(ClientCommandHandler))]
    public sealed class JobCommand : ICommand
    {
        public string   Command     => "job";
        public string[] Aliases     => new[] { "rjob", "setjob" };
        public string   Description => "Manage your RP job. Sub-commands: list, set <name>, clear";

        public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            if (sender is not CommandSystem.PlayerCommandSender ps)
            {
                response = "Player-only command.";
                return false;
            }

            var player = Player.Get(ps.ReferenceHub);
            if (player is null) { response = "Player not found."; return false; }

            if (!Plugin.Instance.Whitename.HasName(player))
            {
                response = "[EliteRP] Set your RP name first with .setname <First Last>.";
                return false;
            }

            if (arguments.Count == 0) { response = "Usage: .job <list|set <name>|clear>"; return false; }

            switch (arguments.Array![arguments.Offset].ToLower())
            {
                case "list":
                {
                    var sb = new StringBuilder();
                    sb.AppendLine("<color=#00cfff>[EliteRP]</color> Available jobs:");
                    foreach (var kv in Plugin.Instance.Jobs.Jobs)
                    {
                        sb.AppendLine(
                            $"  <color={kv.Value.BadgeColor}>{kv.Value.Name}</color>" +
                            $" — {kv.Value.Description} | Salary: ${kv.Value.Salary}/round" +
                            $" | Slots: {kv.Value.MaxSlots}");
                    }
                    response = sb.ToString();
                    return true;
                }

                case "set" when arguments.Count >= 2:
                {
                    var jobName = string.Join(" ", arguments.Array,
                                             arguments.Offset + 1, arguments.Count - 1);
                    var ok = Plugin.Instance.Jobs.AssignJob(player, jobName);
                    response = ok
                        ? $"[EliteRP] You are now working as {jobName}."
                        : $"[EliteRP] Unknown job '{jobName}'. Use .job list.";
                    return ok;
                }

                case "clear":
                    Plugin.Instance.Jobs.ClearJob(player);
                    response = "[EliteRP] Your RP job has been cleared.";
                    return true;

                default:
                    response = "Usage: .job <list|set <name>|clear>";
                    return false;
            }
        }
    }
}
