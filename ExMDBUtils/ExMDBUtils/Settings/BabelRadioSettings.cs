using Exiled.API.Features.Spawn;
using Exiled.API.Structs;
using System.Collections.Generic;
using System.ComponentModel;
using VoiceChat;

namespace ExMDBUtils.Settings
{
    public class BabelRadioSettings
    {
        public bool IsEnabled { get; set; } = true;

        [Description("Setting to true hides player names, but will cause all but one person speaking at a time to go glitchy.")]
        public bool IsAnonymous { get; set; } = false;

        [Description("Allows the radio user to communicate with the the SCP\'s")]
        public bool CanSpeak { get; set; } = false;

        public SpawnProperties SpawnProperties { get; set; } = new SpawnProperties()
        {
            Limit = 1,
            DynamicSpawnPoints = new List<DynamicSpawnPoint>()
            {
                new DynamicSpawnPoint()
                {
                    Chance = 100,
                    Location = Exiled.API.Enums.SpawnLocationType.InsideLocker
                }
            }
        };


        public float RadioIdleUsage { get; set; } = 160f;
        public int RadioMaxRange { get; set; } = 99;
        public int TalkingUsage { get; set; } = 20;

        public VoiceChatChannel radioToVc { get; set; } = VoiceChatChannel.RoundSummary;
    }
}
