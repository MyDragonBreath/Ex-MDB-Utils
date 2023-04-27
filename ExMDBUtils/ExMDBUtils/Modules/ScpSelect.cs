using CommandSystem;
using Exiled.API.Features;
using Exiled.Permissions.Extensions;
using PlayerRoles;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ExMDBUtils.Modules
{
    [Module("ScpSelect")]
    public class ScpSelect : BaseModule
    {
        RoleTypeId[] valid_roles = { RoleTypeId.Scp049, RoleTypeId.Scp173, RoleTypeId.Scp096, RoleTypeId.Scp106 };

        public static List<RoleTypeId> roles = new List<RoleTypeId>();
        public override void Enable(Plugin _plugin)
        {
            _plugin.ServerHandlers._OnRoundStarted += OnRoundStarted;
            roles = new();
        }
        public void OnRoundStarted()
        {
            List<RoleTypeId> tmpRoles = new(roles);
            foreach (Player player in Player.List.Where(player => player.IsScp))
            {
                while (tmpRoles.Count > 0)
                {
                    player.Role.Set(tmpRoles.PullRandomItem());
                }
            }

            var sn = Player.List.Where(player => player.Role == RoleTypeId.Scp079).ToList();
            for (int i = 0; i< sn.Count()-1; i++) { sn[i].Role.Set(valid_roles.RandomItem()); }
        }




        [CommandHandler(typeof(RemoteAdminCommandHandler))]
        public class ScpSelect_Command : ParentCommand
        {
            public override string Command { get; } = "scpselect";

            public override string[] Aliases { get; } = new string[] { };

            public override string Description { get; } = "Interects with the ScpSelect Module";

            public override void LoadGeneratedCommands() { }

            protected override bool ExecuteParent(ArraySegment<string> arguments, ICommandSender sender, out string response)
            {
                if (!((CommandSender)sender).CheckPermission("exutils.scpselect"))
                {
                    response = "You do not have permission to use this command";
                    return false;
                }

                if (arguments.Count == 0)
                {
                    response = "Usage: scpselect (add/remove/list/all) [RoleType]";
                    return false;
                }

                switch (arguments.At(0))
                {
                    default:
                        response = "Usage: scpselect (add/remove/list/all) [RoleType]";
                        return false;
                    case "add":
                        if (arguments.Count != 2)
                        {
                            response = "Usage: scpselect add [RoleType]";
                            return false;
                        }
                        bool valid = Enum.TryParse<RoleTypeId>(arguments.At(1), out RoleTypeId res);
                        if (!valid)
                        {
                            response = "Couldn't find that role";
                            return false;
                        }

                        roles.Add(res);


                        response = "Added " + res.ToString() + " to the possible Scp\'s";
                        return true;
                    case "remove":
                        if (arguments.Count != 2)
                        {
                            response = "Usage: scpselect remove [RoleType]";
                            return false;
                        }
                        bool valid2 = Enum.TryParse<RoleTypeId>(arguments.At(1), out RoleTypeId res2);
                        if (!valid2)
                        {
                            response = "Couldn't find that role";
                            return false;
                        }

                        roles.Remove(res2);

                        response = "Removed " + res2.ToString() + " from the possible Scp\'s";
                        return true;
                    case "all":
                        response = String.Join(", ", Enum.GetNames(typeof(RoleTypeId)));
                        return true;
                    case "list":
                        response = String.Join(", ", roles);
                        if (response == "") response = "None";
                        return true;


                }
            }
        }
    }
}
