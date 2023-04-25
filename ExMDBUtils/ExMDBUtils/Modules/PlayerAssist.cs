using CommandSystem;
using Exiled.API.Features;
using Exiled.Permissions.Extensions;
using PlayerRoles;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ExMDBUtils.Modules
{
    [Module("PlayerAssist")]
    public class PlayerAssist : BaseModule
    {
        public static List<Player> players;
        public override void Enable(Plugin _plugin)
        {
            players = new List<Player>();
            _plugin.ServerHandlers._OnRoundStarted += OnRoundStarted;
        }

        public void OnRoundStarted()
        {
            players.ForEach(x =>
            {
                Player switchto = Player.List.ToList().First(x => x.Role == RoleTypeId.ClassD);
                var myrole = x.Role.Type;

                switchto.Role.Set(myrole, Exiled.API.Enums.SpawnReason.RoundStart, RoleSpawnFlags.All);
                x.Role.Set(RoleTypeId.ClassD);
            });

            players.Clear();
        }


        [CommandHandler(typeof(RemoteAdminCommandHandler))]
        public class PlayerAssist_Command : ParentCommand
        {
            public override string Command { get; } = "playerassist";

            public override string[] Aliases { get; } = new string[] { "pa" };

            public override string Description { get; } = "Interects with the Player Assist Module";

            public override void LoadGeneratedCommands() { }

            protected override bool ExecuteParent(ArraySegment<string> arguments, ICommandSender sender, out string response)
            {

                if (!((CommandSender)sender).CheckPermission("exutils.playerassist"))
                {
                    response = "You do not have permission to use this command";
                    return false;
                }
                
                if (arguments.Count == 0)
                {
                    response = String.Join(",", players);
                    return true;
                }

                try
                {
                    Player p = Player.List.First(x => x.Nickname == arguments.At(0));

                    if (players.Contains(p)) players.Remove(p);
                    else players.Add(p);

                    response = $"{arguments.At(0)} " + (players.Contains(p) ? "added" : "removed");
                    return true;

                } catch (Exception ex)
                {
                    response = $"Couldnt find {arguments.At(0)}";
                    return false;
                }
            }
        }
    }
}
