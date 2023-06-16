using Exiled.API.Enums;
using Exiled.API.Features;
using Exiled.API.Features.Attributes;
using Exiled.API.Features.Items;
using Exiled.API.Features.Pickups;
using Exiled.API.Features.Spawn;
using Exiled.API.Structs;
using Exiled.CustomItems.API.EventArgs;
using Exiled.CustomItems.API.Features;
using Exiled.Events.EventArgs.Player;
using Exiled.Loader;
using ExMDBUtils.API.Voice;
using InventorySystem.Items;
using InventorySystem.Items.Pickups;
using MapGeneration.Distributors;
using Mirror;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace ExMDBUtils.Modules.BabelRadioModule
{
    [CustomItem(ItemType.Radio)]
    public class BabelRadioItem : CustomItem
    {
        public override uint Id { get; set; } = 60;
        public override string Name { get; set; } = "<color=orange>Babel Radio</color>";

        public override string Description { get; set; } = "Listen To <color=red>Scp\'s</color>";
        public override float Weight { get; set; } = 0.75f;
        public override SpawnProperties SpawnProperties { get; set; } = new()
        {
            Limit = 1,
            DynamicSpawnPoints = new List<DynamicSpawnPoint>()
            {
                new DynamicSpawnPoint()
                {
                    Chance = 100,
                    Location = Exiled.API.Enums.SpawnLocationType.InsideLocker
                }
            }
        };
        public override ItemType Type { get; set; } = ItemType.Radio;


        public override uint Spawn(IEnumerable<SpawnPoint> spawnPoints, uint limit)
        {
            uint spawned = 0;

            var query = spawnPoints.Where(x => ((x is DynamicSpawnPoint r) && r.Location == SpawnLocationType.InsideLocker));
            var toSpawnNormally = spawnPoints.Where(x => !query.Contains(x));

            foreach (var point in query)
            {
                if (Plugin.Singleton.Config.Debug) Log.Info($"Attempting spawn BabelRadio {spawned}");
                if (Loader.Random.NextDouble() * 100 >= point.Chance || (limit > 0 && spawned >= limit))
                    continue;
                spawned++;

                var lockers = Map.Lockers.Where(x => x is not null && x.Loot is not null && x.Chambers is not null && x is PedestalScpLocker).OrderBy(x => new Guid());
                Vector3 position = lockers.ElementAt(0).Chambers[0]._spawnpoint.transform.position;

                try {
                GameObject.FindObjectsOfType<ItemPickupBase>().Where(x => Vector3.Distance(x.transform.position, lockers.ElementAt(0).Chambers[0]._spawnpoint.position) < 2).ToList()
                    .ForEach(x => { NetworkServer.Destroy(x.gameObject); });
                } catch (Exception e) { Log.Info($"Item failed to remove at {position}"); }

                

                if (Plugin.Singleton.Config.Debug) Log.Info($"Spawning the BabelRadio in an ScpPedestal at {position}");

                Spawn(position, null);
            }


            if (limit <= spawned) return spawned;
            return base.Spawn(toSpawnNormally, limit - spawned);
        }


        public override Pickup Spawn(Vector3 position, Player? owner = null)
        {
            Pickup pickup = Pickup.CreateAndSpawn(Type, position, default);
            pickup.Weight = Weight;
            TrackedSerials.Add(pickup.Serial);
            return pickup;
        }

        public CustomVoiceChannel ScpToRadio;
        public CustomVoiceChannel RadioToScp;

        protected override void OnDropping(DroppingItemEventArgs ev)
        {
            StoppedUsing(ev.Player);
            base.OnDropping(ev);
        }

        protected override void OnOwnerHandcuffing(OwnerHandcuffingEventArgs ev)
        {
            StoppedUsing(ev.Player);
            base.OnOwnerHandcuffing(ev);
        }

        protected override void OnOwnerEscaping(OwnerEscapingEventArgs ev)
        {
            StoppedUsing(ev.Player);
            base.OnOwnerEscaping(ev);
        }

        protected override void SubscribeEvents()
        {
            Exiled.Events.Handlers.Player.ChangingRadioPreset += Player_ChangingRadioPreset;
            Exiled.Events.Handlers.Player.UsingRadioBattery += Player_UsingRadioBattery;
            Exiled.Events.Handlers.Player.UsedItem += Player_UsedItem;
            Exiled.Events.Handlers.Player.MakingNoise += Player_MakingNoise;
            Exiled.Events.Handlers.Player.ChangingRole += Player_ChangingRole;
            Exiled.Events.Handlers.Player.Died += Player_Died;
            base.SubscribeEvents();
        }

        private void Player_Died(DiedEventArgs ev)
        {
            StoppedUsing(ev.Player);
        }

        private void Player_ChangingRole(ChangingRoleEventArgs ev)
        {
            StoppedUsing(ev.Player);
        }

        private void Player_MakingNoise(MakingNoiseEventArgs ev)
        {
            UsingItem(ev.Player.CurrentItem, ev.Player);
        }

        private void Player_UsedItem(UsedItemEventArgs ev)
        {
            UsingItem(ev.Item, ev.Player);
        }

        private void Player_UsingRadioBattery(UsingRadioBatteryEventArgs ev)
        {
            UsingItem(ev.Radio, ev.Player);
        }

        private void Player_ChangingRadioPreset(ChangingRadioPresetEventArgs ev)
        {
            UsingItem(ev.Player.CurrentItem, ev.Player);
        }



        private void UsingItem(Item? item, Player p)
        {
            if (item != null && item is Radio r && Check(item) && r.IsEnabled && r.BatteryLevel > 0)
            {

                RadioRangeSettings BabelRangeSettings = new RadioRangeSettings() { IdleUsage = Plugin.Singleton.Config.BabelRadio.RadioIdleUsage, MaxRange = Plugin.Singleton.Config.BabelRadio.RadioMaxRange, TalkingUsage = Plugin.Singleton.Config.BabelRadio.TalkingUsage };
                r.SetRangeSettings(Exiled.API.Enums.RadioRange.Long, BabelRangeSettings);
                r.SetRangeSettings(Exiled.API.Enums.RadioRange.Ultra, BabelRangeSettings);
                r.SetRangeSettings(Exiled.API.Enums.RadioRange.Medium, BabelRangeSettings);
                r.SetRangeSettings(Exiled.API.Enums.RadioRange.Short, BabelRangeSettings);

                StartUsing(p);

            } else
            {
                StoppedUsing(p);
            }
        }

        private void StartUsing(Player p)
        {
            var vc = ScpToRadio[1];
            vc.Add(p.Id);
            ScpToRadio[1] = vc;


            if (Plugin.Singleton.Config.BabelRadio.CanSpeak)
            {
                var vc2 = RadioToScp[0];
                vc2.Add(p.Id);
                RadioToScp[0] = vc2;

                RadioToScp[1].Clear();
                RadioToScp[1].AddRange(Player.List.Where(x => x.Role.Team == PlayerRoles.Team.SCPs).Select(x => x.Id));
            }
        }
        private void StoppedUsing(Player player)
        {
            var vc = ScpToRadio[1];
            vc.RemoveAll(x => x == player.Id);
            ScpToRadio[1] = vc;

            if (Plugin.Singleton.Config.BabelRadio.CanSpeak)
            {
                var vc2 = RadioToScp[0];
                vc2.RemoveAll(x => x == player.Id);
                RadioToScp[0] = vc2;
            }
        }
    }
}
