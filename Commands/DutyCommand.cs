using System;
using CommandSystem;
using Exiled.API.Features;

namespace EliteRP.Commands
{
    /// <summary>
    /// <b>.duty</b> — Toggles staff on/off-duty status (staff-only client command).
    /// </summary>
    [CommandHandler(typeof(ClientCommandHandler))]
    public sealed class DutyCommand : ICommand
    {
        public string   Command     => "duty";
        public string[] Aliases     => new[] { "staffduty", "onduty" };
        public string   Description => "Toggle your staff on/off-duty status.";

        public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            if (sender is not CommandSystem.PlayerCommandSender ps)
            {
                response = "Player-only command.";
                return false;
            }

            var player = Player.Get(ps.ReferenceHub);
            if (player is null) { response = "Player not found."; return false; }

            if (!Plugin.Instance.StaffDuty.IsStaff(player))
            {
                response = "<color=red>[Duty]</color> You do not have staff permissions.";
                return false;
            }

            if (Plugin.Instance.StaffDuty.IsOnDuty(player))
            {
                Plugin.Instance.StaffDuty.SetOffDuty(player);
                response = "[Duty] You are now OFF duty.";
            }
            else
            {
                Plugin.Instance.StaffDuty.SetOnDuty(player);
                response = "[Duty] You are now ON duty.";
            }

            return true;
        }
    }
}
