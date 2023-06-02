using Exiled.API.Enums;
using Exiled.API.Features;
using PlayerRoles;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExMDBUtils.Modules
{
    //[Module("AntiTrap")]
    public class AntiTrap : BaseModule
    {
        public override void Enable(Plugin _plugin)
        {
            Exiled.Events.Handlers.Player.InteractingDoor += Player_InteractingDoor;
            
        }

        static Dictionary<RoomType, DateTime> RoomTimers = new Dictionary<RoomType,DateTime>();


        DoorType[] WatchDoors = new DoorType[] { DoorType.Scp330, DoorType.Scp330Chamber, DoorType.GateA, DoorType.GateB, DoorType.HczArmory, DoorType.LczArmory, DoorType.Intercom};
        RoomType[] roomTypes = new RoomType[] { RoomType.Lcz330, RoomType.Lcz330, RoomType.EzGateA, RoomType.EzGateB, RoomType.HczArmory, RoomType.LczArmory, RoomType.EzIntercom };
        private void Player_InteractingDoor(Exiled.Events.EventArgs.Player.InteractingDoorEventArgs ev)
        {
            if (WatchDoors.Contains(ev.Door.Type))
            {
                RoomType rt = roomTypes[WatchDoors.IndexOf(ev.Door.Type)];
                Room room = Room.Get(rt);
                IEnumerable<Player> players = room.Players;

                List<Player> validScps = players.Where(x => x.IsScp && x.Role != RoleTypeId.Scp079 && x.Role != RoleTypeId.Scp106).ToList();
                if (validScps.Count > 0) 
                {
                    if (RoomTimers.ContainsKey(rt)) { RoomTimers[rt] = DateTime.Now; }
                    else { RoomTimers.Add(rt, DateTime.Now); }
                };
            }
        }
    }
}
