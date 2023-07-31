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

                RoundSummary.EscapedScientists++;
            } 

            if (e.EscapeScenario == EscapeScenario.CuffedScientist)
            {
                st = EscapeScenarioType.ClassD;
                e.EscapeScenario = EscapeScenario.ClassD;
                RespawnTokensManager.GrantTokens(SpawnableTeamType.ChaosInsurgency, 6f);
                rt = RoleTypeId.ChaosConscript;

                RoundSummary.EscapedClassD++;
            }


            
            if (!EventManager.ExecuteEvent(new PlayerEscapeEvent( ReferenceHub.GetHub(e.Player.Id), rt)))
            {
                return;
            }

            ReferenceHub.GetHub(e.Player.Id).netIdentity.connectionToClient.Send(new EscapeMessage
            {
                ScenarioId = (byte)st,
                EscapeTime = (ushort)Mathf.CeilToInt(ReferenceHub.GetHub(e.Player.Id).roleManager.CurrentRole.ActiveTime)
            });

            ReferenceHub.GetHub(e.Player.Id).roleManager.ServerSetRole(rt, RoleChangeReason.Escaped);

            if (p.Config.Debug) Log.Info($"Escaped {e.EscapeScenario}");
        }
    }
}
