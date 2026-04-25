using System;
using CommandSystem;
using Exiled.API.Features;

namespace EliteRP.Commands
{
    /// <summary>
    /// <b>.wallet [pay &lt;player&gt; &lt;amount&gt;]</b> — Check balance or pay another player.
    /// </summary>
    [CommandHandler(typeof(ClientCommandHandler))]
    public sealed class WalletCommand : ICommand
    {
        public string   Command     => "wallet";
        public string[] Aliases     => new[] { "balance", "money" };
        public string   Description => "Check your wallet or pay another player. Usage: .wallet | .wallet pay <name> <amount>";

        public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            if (sender is not CommandSystem.PlayerCommandSender ps)
            {
                response = "Player-only command.";
                return false;
            }

            var player = Player.Get(ps.ReferenceHub);
            if (player is null) { response = "Player not found."; return false; }

            // .wallet  → show balance
            if (arguments.Count == 0)
            {
                var bal = Plugin.Instance.Economy.GetBalance(player);
                response = $"<color=#00cfff>[Wallet]</color> Balance: <b>${bal}</b>";
                return true;
            }

            // .wallet pay <name> <amount>
            if (arguments.Array![arguments.Offset].Equals("pay", StringComparison.OrdinalIgnoreCase))
            {
                if (arguments.Count < 3)
                {
                    response = "Usage: .wallet pay <player name> <amount>";
                    return false;
                }

                var targetName = arguments.Array[arguments.Offset + 1];
                var target     = Player.Get(targetName);
                if (target is null)
                {
                    response = $"[Wallet] No player found named '{targetName}'.";
                    return false;
                }

                if (!int.TryParse(arguments.Array[arguments.Offset + 2], out var amount) || amount <= 0)
                {
                    response = "[Wallet] Amount must be a positive integer.";
                    return false;
                }

                if (!Plugin.Instance.Economy.Transfer(player, target, amount))
                {
                    response = $"[Wallet] Insufficient funds. Your balance: ${Plugin.Instance.Economy.GetBalance(player)}";
                    return false;
                }

                target.Broadcast(5,
                    $"<color=green>[Wallet]</color> <b>{player.Nickname}</b> sent you <b>${amount}</b>.");
                response = $"[Wallet] Sent ${amount} to {target.Nickname}. New balance: ${Plugin.Instance.Economy.GetBalance(player)}";
                return true;
            }

            response = "Usage: .wallet | .wallet pay <name> <amount>";
            return false;
        }
    }
}
