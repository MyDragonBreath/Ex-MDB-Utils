using Exiled.Events.EventArgs.Player;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Exiled.Events.Events;

namespace ExMDBUtils.EventHandlers
{
    public class PlayerHandler
    {
        private readonly Plugin _plugin;
        public PlayerHandler(Plugin plugin) => _plugin = plugin;

        public delegate void CustomEventHandler();



        public event CustomEventHandler<EscapingEventArgs> _OnEscaping;
        public void OnEscaping(EscapingEventArgs e) => _OnEscaping.Invoke(e);
    }
}
