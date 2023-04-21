using Exiled.API.Features;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExMDBUtils.EventHandlers
{
    public class ServerHandler
    {
        private readonly Plugin _plugin;
        public ServerHandler(Plugin plugin) => _plugin = plugin;

        public delegate void CustomEventHandler();



        public event CustomEventHandler _OnRoundStarted; 
        public void OnRoundStarted() => _OnRoundStarted.Invoke();
    }
}
