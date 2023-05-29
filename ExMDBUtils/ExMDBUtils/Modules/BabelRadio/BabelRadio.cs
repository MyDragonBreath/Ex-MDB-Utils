using Exiled.CustomItems.API;
using ExMDBUtils.API.Voice;
using ExMDBUtils.Modules.BabelRadioModule;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExMDBUtils.Modules
{
    [Module("BabelRadio")]
    public class BabelRadio : BaseModule
    {
        private BabelRadioItem pv;
        public override void Enable(Plugin _plugin)
        {
            pv = new BabelRadioItem();
            pv.SpawnProperties = _plugin.Config.BabelRadio.SpawnProperties;
            pv.ScpToRadio = new CustomVoiceChannel(new int[] { }, VoiceChat.VoiceChatChannel.ScpChat, new int[] { -1 }, VoiceChat.VoiceChatChannel.Radio, Plugin.Singleton.Config.BabelRadio.IsAnonymous);
            pv.ScpToRadio = new CustomVoiceChannel(new int[] { -1 }, VoiceChat.VoiceChatChannel.Radio, new int[] { }, VoiceChat.VoiceChatChannel.ScpChat, Plugin.Singleton.Config.BabelRadio.IsAnonymous);
            pv.Register();
        }
    }
}
