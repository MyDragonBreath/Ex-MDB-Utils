using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Exiled.API.Features;
using HarmonyLib;

using ExMDBUtils.EventHandlers;
using Server = Exiled.Events.Handlers.Server;
using Player = Exiled.Events.Handlers.Player;
using Map = Exiled.Events.Handlers.Map;
using ExMDBUtils.API.Voice;
using Exiled.CustomItems.API;

namespace ExMDBUtils
{
    public class Plugin : Plugin<Config>
    {
        public override string Name => "ExUtils";
        public override string Author => "MyDragonBreath";
        public override Version Version => new Version(1, 0, 0);

        public static Plugin Singleton;
        public ServerHandler ServerHandlers;
        public PlayerHandler PlayerHandlers;
        public ModuleManager ModuleManagers;

        
        public override void OnEnabled()
        {
            PlayerVoiceExtentions.ValidationPatches.init();

            

            Singleton = this;
            ServerHandlers = new(this);
            PlayerHandlers = new(this);
            ModuleManagers = new(this);

            Player.Escaping += PlayerHandlers.OnEscaping;
            Player.InteractingElevator += PlayerHandlers.OnInteractingElevator;
            Server.RoundStarted += ServerHandlers.OnRoundStarted;

            Log.Info("ExUtils Enabled");
            base.OnEnabled();
        }
        public override void OnDisabled()
        {
            PlayerVoiceExtentions.ValidationPatches.deinit();

            Player.Escaping -= PlayerHandlers.OnEscaping;
            Player.InteractingElevator -= PlayerHandlers.OnInteractingElevator;
            Server.RoundStarted -= ServerHandlers.OnRoundStarted;


            ServerHandlers = null;
            ModuleManagers = null;

            
            Log.Info("ExUtils Disabled");
            base.OnDisabled();

        }
    }
}
