using Exiled.API.Enums;
using Exiled.API.Features;
using Exiled.Events.EventArgs.Player;
using PlayerRoles;
using PluginAPI.Enums;
using PluginAPI.Events;
using Respawning;
using System.Reflection;
using UnityEngine;
using static Escape;

namespace ExMDBUtils.Modules
{

    [Module("DetainUse")]
    public class DetainUse : BaseModule
    {
        Plugin p;
        public override void Enable(Plugin _plugin)
        {
            p = _plugin;
            _plugin.PlayerHandlers._OnEscaping += OnEscaping;
        }

        public void OnEscaping(EscapingEventArgs e)
        {
            if (p.Config.Debug) Log.Info($"Escaping {e.EscapeScenario}");
            if (e.EscapeScenario != EscapeScenario.CuffedClassD && e.EscapeScenario != EscapeScenario.CuffedScientist) return;
            e.IsAllowed = false;

            EscapeScenarioType st = EscapeScenarioType.ClassD;
            RoleTypeId rt = RoleTypeId.ClassD;
            if (e.EscapeScenario == EscapeScenario.CuffedClassD)
            {
                st = EscapeScenarioType.Scientist;
                e.EscapeScenario = EscapeScenario.Scientist;
                RespawnTokensManager.GrantTokens(SpawnableTeamType.NineTailedFox, 5f);
                rt = RoleTypeId.NtfSpecialist;

                var prop = typeof(RoundSummary).GetProperty("EscapedScientists");
                if (p.Config.Debug) Log.Info($"Original S {(int)prop.GetValue(null, null)}");
                prop.SetValue(null, (int)prop.GetValue(null, null) + 1);
                if (p.Config.Debug) Log.Info($"New S {(int)prop.GetValue(null, null)}");
                //RoundSummary.EscapedScientists++;
            } 

            if (e.EscapeScenario == EscapeScenario.CuffedScientist)
            {
                st = EscapeScenarioType.ClassD;
                e.EscapeScenario = EscapeScenario.ClassD;
                RespawnTokensManager.GrantTokens(SpawnableTeamType.ChaosInsurgency, 6f);
                rt = RoleTypeId.ChaosConscript;

                var prop = typeof(RoundSummary).GetProperty("EscapedClassD");
                if (p.Config.Debug) Log.Info($"Original CD {(int)prop.GetValue(null, null)}");
                prop.SetValue(null, (int)prop.GetValue(null, null) + 1);
                if (p.Config.Debug) Log.Info($"New CD {(int)prop.GetValue(null, null)}");
                //RoundSummary.EscapedClassD++;
            }


            
            if (EventManager.ExecuteEvent(ServerEventType.PlayerEscape, ReferenceHub.HostHub, e.EscapeScenario))
            {
                ReferenceHub.GetHub(e.Player.GameObject).connectionToClient.Send(new EscapeMessage
                {
                    ScenarioId = (byte)st,
                    EscapeTime = (ushort)Mathf.CeilToInt(ReferenceHub.GetHub(e.Player.GameObject).roleManager.CurrentRole.ActiveTime)
                });
                //Escape.OnServerPlayerEscape(ReferenceHub.HostHub); NO ACHIEVEMENTS FOR YOU HAHAHAHAHAHAHAHA
                ReferenceHub.GetHub(e.Player.GameObject).roleManager.ServerSetRole(rt, RoleChangeReason.Escaped);
            }

            if (p.Config.Debug) Log.Info($"Escaped {e.EscapeScenario}");
        }
    }
}
