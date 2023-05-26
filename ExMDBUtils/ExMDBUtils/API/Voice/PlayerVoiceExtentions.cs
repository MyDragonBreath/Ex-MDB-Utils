using Exiled.API.Features;
using PlayerRoles.Voice;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VoiceChat;
using HarmonyLib;
using Mirror;
using VoiceChat.Networking;
using System.Runtime.CompilerServices;
using System.Reflection;
using System.Runtime.Remoting.Channels;
using PlayerRoles.Spectating;
using PlayerRoles.PlayableScps;
using PlayerStatsSystem;
using PlayerRoles.PlayableScps.Scp079;
using PlayerRoles.PlayableScps.Scp939;

namespace ExMDBUtils.API.Voice
{
    public static class PlayerVoiceExtentions
    {
        
        public static VoiceChatChannel GetVoiceChannel(this Player p)
        {
            ReferenceHub playerhub = ReferenceHub.GetHub(p.Id);
            if (playerhub.roleManager.CurrentRole is IVoiceRole voiceRole)
            {
                return voiceRole.VoiceModule.CurrentChannel;
            }

            return VoiceChatChannel.None;
        }

        public static class ValidationPatches
        {
            public static Harmony harmony;

            //private static CustomVoiceChannel ScpToMimic;

            public static List<CustomVoiceChannel> ValidChannels = new List<CustomVoiceChannel>();

            public static void init()
            {
                harmony = new Harmony("dev.dragonbreath.voiceAPI");

                //ScpToMimic = new CustomVoiceChannel(new int[] { }, VoiceChatChannel.ScpChat, new int[] { }, VoiceChatChannel.Intercom);
                //ValidChannels.Add(ScpToMimic);


                MethodInfo original = AccessTools.Method(typeof(VoiceTransceiver), nameof(VoiceTransceiver.ServerReceiveMessage));
                MethodInfo prefix = AccessTools.Method(typeof(ValidationPatches), nameof(ValidationPatches.prefix_ServerReceiveMessage));
                harmony.Patch(original, prefix: new HarmonyMethod(prefix));
            }

            public static void deinit()
            {
                harmony.UnpatchAll();
                harmony = null;
            }

            public static bool prefix_ServerReceiveMessage(NetworkConnection conn, VoiceMessage msg)
            {

                if (msg.SpeakerNull || msg.Speaker.netId != conn.identity.netId || !(msg.Speaker.roleManager.CurrentRole is IVoiceRole voiceRole) || !voiceRole.VoiceModule.CheckRateLimit() || VoiceChatMutes.IsMuted(msg.Speaker))
                {
                    return true;
                }
                VoiceChatChannel voiceChatChannel = voiceRole.VoiceModule.ValidateSend(msg.Channel);
                if (voiceChatChannel == VoiceChatChannel.None)
                {
                    return true;
                }
                
                foreach (var channel in ValidChannels)
                {
                    if (channel.SendingVoiceChannel == msg.Channel)
                    {
                        voiceChatChannel = channel.ReceivingVoiceChannel;
                        if (channel[0].Count == 0 || channel[0].Contains(msg.Speaker.PlayerId))
                        {
                            int[] whitelist = null;
                            if (channel[1].Count != 0) whitelist = channel[1].ToArray();
                            
                            foreach (ReferenceHub r in ReferenceHub.AllHubs) {
                                if (r.roleManager.CurrentRole is IVoiceRole vc)
                                {
                                    if (whitelist != null && !whitelist.Contains(r.PlayerId)) continue;

                                    var tempref = msg;
                                    tempref.Channel = vc.VoiceModule.ValidateReceive(msg.Speaker, voiceChatChannel); 
                                    
                                    if (channel.anon) tempref.Speaker = ReferenceHub.HostHub;
                                    
                                    if (voiceChatChannel == VoiceChatChannel.ScpChat) { tempref.Channel = VoiceChatChannel.ScpChat; }
                                    if (tempref.Channel == 0) continue;
                                    r.netIdentity.connectionToClient.Send(tempref);
                                }
                            }

                            return true;
                        }
                    }
                }

                return true;
            }




        }

    }

    public class CustomVoiceChannel
    {
        public VoiceChatChannel ReceivingVoiceChannel;
        public VoiceChatChannel SendingVoiceChannel;

        private List<int> SendingPlayers = new List<int>() { -1 };
        private List<int> ReceivingPlayers = new List<int>() { -1 } ;

        public bool anon = false;
        public CustomVoiceChannel(VoiceChatChannel SendingVoiceChannel, VoiceChatChannel ReceivingVoiceChannel, bool anonymous = false, VoiceChatChannel OverrideVoiceChannel = VoiceChatChannel.Proximity)
        {
            this.ReceivingVoiceChannel = ReceivingVoiceChannel;
            this.SendingVoiceChannel = SendingVoiceChannel;
            anon = anonymous;
        }

        public CustomVoiceChannel(int[] sendingPlayers, VoiceChatChannel SendingVoiceChannel, int[] receivingPlayers, VoiceChatChannel ReceivingVoiceChannel, bool anonymous = false, VoiceChatChannel OverrideVoiceChannel = VoiceChatChannel.Proximity)
        {
            this.ReceivingVoiceChannel = ReceivingVoiceChannel;
            this.SendingVoiceChannel = SendingVoiceChannel;
            anon = anonymous;

            this.SendingPlayers.Clear();
            this.ReceivingPlayers.Clear();
            this.SendingPlayers.AddRange(sendingPlayers);
            this.ReceivingPlayers.AddRange(receivingPlayers);
        }


        public List<int> this[int i]
        {
            get
            {
                switch (i)
                {
                    default: throw new Exception("Not Implemented");
                    case 0: return SendingPlayers;
                    case 1: return ReceivingPlayers;
                }
            }
            set
            {
                switch (i)
                {
                    default: throw new Exception("Not Implemented");
                    case 0: SendingPlayers = value; break;
                    case 1: ReceivingPlayers = value; break;
                }
            }
        }
        
    }
}
