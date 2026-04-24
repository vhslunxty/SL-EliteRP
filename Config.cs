using System.Collections.Generic;
using System.ComponentModel;
using Exiled.API.Interfaces;
using UnityEngine;

namespace EliteRP
{
    public sealed class Config : IConfig
    {
        [Description("Enable or disable the plugin.")]
        public bool IsEnabled { get; set; } = true;

        [Description("Print extra debug messages to the server console.")]
        public bool Debug { get; set; } = false;


        [Description("Seconds of inactivity before an AFK warning is broadcast.")]
        public float AfkWarningSeconds { get; set; } = 60f;

        [Description("Seconds of inactivity before the player is kicked.")]
        public float AfkKickSeconds { get; set; } = 90f;

        [Description("Message sent to a player just before they are AFK-kicked.")]
        public string AfkKickMessage { get; set; } =
            "<color=red>[EliteRP]</color> You have been removed from the server for inactivity (AFK).";


        [Description("Prevent players from killing in OOC-designated areas.")]
        public bool EnforceNoKillZones { get; set; } = true;

        [Description("Roles that are allowed to use OOC chat prefix (.) freely.")]
        public List<string> OocAllowedRoles { get; set; } = new() { "owner", "admin", "moderator" };

        [Description("Minimum seconds between OOC messages for regular players.")]
        public float OocCooldownSeconds { get; set; } = 10f;

        [Description("Starting wallet balance for new players (in RP credits).")]
        public int StartingBalance { get; set; } = 500;

        [Description("Maximum wallet balance a player can hold.")]
        public int MaxBalance { get; set; } = 1_000_000;

        [Description("The broadcast shown when a staff member goes on duty.")]
        public string StaffOnDutyMessage { get; set; } =
            "<color=yellow>[STAFF]</color> <b>{name}</b> is now <color=green>ON DUTY</color>.";

        [Description("The broadcast shown when a staff member goes off duty.")]
        public string StaffOffDutyMessage { get; set; } =
            "<color=yellow>[STAFF]</color> <b>{name}</b> is now <color=red>OFF DUTY</color>.";

        [Description("SCP:SL permission group names that count as staff.")]
        public List<string> StaffGroups { get; set; } = new() { "owner", "admin", "moderator", "helper" };

        [Description("Reward (in RP credits) for arresting a wanted player.")]
        public int WantedArrestReward { get; set; } = 250;

        [Description("Maximum wanted level (stars) before auto-broadcast.")]
        public int MaxWantedLevel { get; set; } = 5;

        [Description("Color used for the job-radio broadcast header.")]
        public string RadioBroadcastColor { get; set; } = "#00cfff";

        [Description("How many seconds a radio broadcast lingers on-screen.")]
        public ushort RadioBroadcastDuration { get; set; } = 5;

        [Description("Custom spawn positions keyed by RP job name.")]
        public Dictionary<string, SerializableVector3> JobSpawnPoints { get; set; } = new()
        {
            ["Police"]  = new SerializableVector3 {  X = 0f,  Y = 0f,  Z = 0f },
            ["Doctor"]  = new SerializableVector3 {  X = 10f, Y = 0f,  Z = 5f },
            ["Dealer"]  = new SerializableVector3 { X = -10f, Y = 0f, Z = -5f },
        };

        [Description("Minimum characters required for an RP name.")]
        public int MinWhitenameLength { get; set; } = 4;

        [Description("Maximum characters allowed for an RP name.")]
        public int MaxWhitenameLength { get; set; } = 32;

        [Description("Regex pattern RP names must match (empty = no restriction).")]
        public string WhitenamePattern { get; set; } = @"^[A-Za-z\s\-'\.]+$";
    }
    
    public sealed class SerializableVector3
    {
        public float X { get; set; }
        public float Y { get; set; }
        public float Z { get; set; }
        public Vector3 ToVector3() => new(X, Y, Z);
    }
}
