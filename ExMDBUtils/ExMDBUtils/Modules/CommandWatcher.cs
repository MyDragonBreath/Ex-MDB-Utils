using CommandSystem;
using Exiled.API.Features;
using Exiled.Permissions.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExMDBUtils.Modules
{
    [Module("CommandWatcher")]
    public class CommandWatcher : BaseModule
    {
        public static Player[] AlertList;
        public override void Enable(Plugin _plugin)
        {

            PluginAPI.Events.EventManager.Events[PluginAPI.Enums.ServerEventType.RemoteAdminCommandExecuted].RegisterInvoker(this.GetType(), null, this.GetType().GetMethod("RemoteAdminCommandExecuted"), true);
        }


        public static void RemoteAdminCommandExecuted(ICommandSender commandSender, string command, string[] arguments, bool result, string response)
        {
            Player.List.Where(x => x.CheckPermission(new PlayerPermissions[] { PlayerPermissions.ServerConsoleCommands })).ToList().ForEach(x =>
            {
                x.ShowHint($"Player {commandSender.LogName} used {command}");
                x.SendConsoleMessage($"Player {commandSender.LogName} used {command} with {String.Join(",",arguments)}. Got a {result} output", "pink");
            });
        }
    }
}
