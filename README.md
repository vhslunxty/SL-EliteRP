# EliteRP — SCP:SL RolePlay Plugin (Exiled 8.x)

A full-featured, modular RolePlay plugin built on the **Exiled 8.x** framework.  
All player data is stored as **JSON flat-files** inside `~/.config/EXILED/Configs/EliteRP/`.
For EliteRP Community and FA Code

---

## Features

| System | Description |
|---|---|
| **Job System** | Assign RP jobs (Police, Doctor, Criminal, Dealer, Citizen, …). Custom job definitions via `job_definitions.json`. Salary per round, slot limit, badge colour. |
| **Whitename / Identity** | Players must set a valid RP name before using any RP command. Names are unique, length-validated, and regex-checked. |
| **Economy / Wallet** | Per-player credit wallet. Deposits, withdrawals, player-to-player transfers. Persistent across restarts. |
| **Wanted / Police System** | 1–5 star wanted levels. Police officers earn credit rewards for arresting wanted players (proximity check). Staff can manually set/clear wanted levels via RA. |
| **Staff Duty Mode** | Staff members toggle on/off duty. On-duty badge is shown server-wide via broadcast. Off-duty staff follow normal RP rules. |
| **Inventory Persistence** | Player items are saved on death/disconnect and restored on next spawn — so RP items survive rounds. |
| **AFK Kick** | Players idle for `AfkWarningSeconds` receive a warning; at `AfkKickSeconds` they are kicked with an RP-flavoured message. |
| **RP Rules Enforcer** | OOC cooldown (rate-limits `.` prefix chat). Kill-rule enforcement with automatic wanted-level escalation on RDM. |
| **Radio / Walkie** | `.radio <msg>` broadcasts to all players in the same RP job. Colour and duration configurable. |
| **Custom Spawn Points** | Per-job spawn co-ordinates set in `config.yml`. Players teleport to their job's spawn on assignment. |

---

## Installation

1. **Build** the project:
   ```
   dotnet build -c Release
   ```
   Output: `bin/Release/net6.0/EliteRP.dll`

2. **Copy** `EliteRP.dll` into your server's Exiled plugins folder:
   ```
   ~/.config/EXILED/Plugins/EliteRP.dll
   ```

3. **Set** the `EXILED_REFERENCES` environment variable to your server's managed DLL folder so the project can resolve its references, e.g.:
   ```
   export EXILED_REFERENCES=/path/to/SCPSL_Data/Managed
   ```

4. **Start** the server. On first run, default configs and JSON data files are created automatically in:
   ```
   ~/.config/EXILED/Configs/EliteRP/
   ```

---

## Configuration (`config.yml`)

Key config values (auto-generated under the `elite_rp` prefix):

```yaml
elite_rp:
  is_enabled: true
  debug: false
  afk_warning_seconds: 60
  afk_kick_seconds: 90
  enforce_no_kill_zones: true
  ooc_cooldown_seconds: 10
  starting_balance: 500
  max_balance: 1000000
  wanted_arrest_reward: 250
  max_wanted_level: 5
  radio_broadcast_color: "#00cfff"
  radio_broadcast_duration: 5
  staff_groups:
    - owner
    - admin
    - moderator
    - helper
  job_spawn_points:
    Police:  { x: 0,   y: 0, z: 0  }
    Doctor:  { x: 10,  y: 0, z: 5  }
    Dealer:  { x: -10, y: 0, z: -5 }
```

---

## Player Commands (chat console — prefix `.`)

| Command | Description |
|---|---|
| `.setname First Last` | Set your RP identity name |
| `.job list` | List all available jobs |
| `.job set <name>` | Join an RP job |
| `.job clear` | Leave your current job |
| `.wallet` | View your credit balance |
| `.wallet pay <player> <amount>` | Send credits to another player |
| `.radio <message>` | Broadcast to your job faction |
| `.arrest <player>` | Arrest a nearby wanted player *(Police only)* |
| `.duty` | Toggle on/off duty *(Staff only)* |

## Staff / RA Commands

| Command | Description |
|---|---|
| `wanted add <player> [stars]` | Add wanted stars to a player |
| `wanted clear <player>` | Clear a player's wanted level |

---

## Project Structure

```
EliteRP/
├── EliteRP.csproj
├── Plugin.cs                   ← Entry-point, wires all systems
├── Config.cs                   ← All YAML-configurable values
├── Systems/
│   ├── JobSystem.cs            ← Job definitions + assignment
│   ├── EconomySystem.cs        ← Wallet & transfers
│   ├── WhitenameSystem.cs      ← RP identity names
│   ├── WantedSystem.cs         ← Wanted levels + arrests
│   ├── StaffDutySystem.cs      ← On/off duty toggling
│   ├── InventoryPersistence.cs ← Save/restore player items
│   ├── AfkSystem.cs            ← AFK detection + kick
│   ├── RpRulesSystem.cs        ← OOC cooldown + kill rules
│   └── RadioSystem.cs          ← Job-faction radio broadcasts
├── Handlers/
│   ├── PlayerEventHandler.cs   ← All player Exiled events
│   └── ServerEventHandler.cs   ← Round start/end flush
└── Commands/
    ├── SetNameCommand.cs
    ├── JobCommand.cs
    ├── WalletCommand.cs
    ├── WantedCommand.cs
    ├── ArrestCommand.cs
    ├── RadioCommand.cs
    └── DutyCommand.cs
```
