using Exiled.API.Features;
using Exiled.Events.EventArgs.Player;
using Mirror;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExMDBUtils.Modules
{
    [Module("RealisticElevators")]
    public class RealisticElevators : BaseModule
    {
        Plugin p;
        public override void Enable(Plugin _plugin)
        {
            p = _plugin;
            _plugin.PlayerHandlers._OnInteractingElevator += OnInteractingElevator;
        }

        public void OnInteractingElevator(InteractingElevatorEventArgs ev)
        {
            switch (ev.Lift.Type)
            {
                default:
                    break;
                case Exiled.API.Enums.ElevatorType.GateA:
                case Exiled.API.Enums.ElevatorType.GateB:
                    ev.Elevator._animationTime = 2f + p.Config.RealisticElevators.SurfaceTimerExtention;
                    break;
                case Exiled.API.Enums.ElevatorType.LczB:
                case Exiled.API.Enums.ElevatorType.LczA:
                    ev.Elevator._animationTime = 2f + p.Config.RealisticElevators.LightTimerExtention;
                    break;
            }
        }

    }
}
