using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Exiled.API.Features;


using ExMDBUtils.EventHandlers;
using Server = Exiled.Events.Handlers.Server;
using Player = Exiled.Events.Handlers.Player;

namespace ExMDBUtils
{
    public class Plugin : Plugin<Config>
    {
        public override string Name => "ExUtils";
        public override string Author => "MyDragonBreath";
        public override Version Version => new Version(1, 0, 0);

        public static Plugin Singleton;
        public ServerHandler ServerHandlers;
        public ModuleManager ModuleManagers;
        public override void OnEnabled()
        {
            Singleton = this;
            ServerHandlers = new(this);
            ModuleManagers = new(this);

            Server.RoundStarted += ServerHandlers.OnRoundStarted;
            
            Log.Info("ExUtils Enabled");
            base.OnEnabled();
        }
        public override void OnDisabled()
        {

            Server.RoundStarted -= ServerHandlers.OnRoundStarted;
            ServerHandlers = null;
            ModuleManagers = null;

            Log.Info("ExUtils Disabled");
            base.OnDisabled();

        }
    }
}
