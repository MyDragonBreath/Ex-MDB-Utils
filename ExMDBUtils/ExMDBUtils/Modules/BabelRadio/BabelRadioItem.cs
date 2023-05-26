using Exiled.API.Enums;
using Exiled.API.Features;
using Exiled.API.Features.Attributes;
using Exiled.API.Features.Items;
using Exiled.API.Features.Pickups;
using Exiled.API.Features.Spawn;
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

namespace ExMDBUtils.Modules.BabelRadio
{
    [CustomItem(ItemType.Radio)]
    public class BabelRadioItem : CustomItem
    {
        public override uint Id { get; set; } = 60;
        public override string Name { get; set; } = "Babel Radio";

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

                //if (Plugin.Singleton.Config.Debug) Log.Info($"Finding valid lockers");
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

        private CustomVoiceChannel voiceChannel = new CustomVoiceChannel(new int[] { },VoiceChat.VoiceChatChannel.ScpChat, new int[] { -1 }, VoiceChat.VoiceChatChannel.Radio, false);

        protected override void OnAcquired(Player player, bool displayMessage)
        {
            if (!PlayerVoiceExtentions.ValidationPatches.ValidChannels.Contains(voiceChannel)) PlayerVoiceExtentions.ValidationPatches.ValidChannels.Add(voiceChannel);
            
        }

        protected override void OnDropping(DroppingItemEventArgs ev)
        {
            StoppedUsing(ev.Player);
            base.OnDropping(ev);
        }

        protected override void OnOwnerDying(OwnerDyingEventArgs ev)
        {
            StoppedUsing(ev.Player);
            base.OnOwnerDying(ev);
        }

        protected override void OnOwnerHandcuffing(OwnerHandcuffingEventArgs ev)
        {
            StoppedUsing(ev.Player);
            base.OnOwnerHandcuffing(ev);
        }

        protected override void OnOwnerChangingRole(OwnerChangingRoleEventArgs ev)
        {
            StoppedUsing(ev.Player);
            base.OnOwnerChangingRole(ev);
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
            Exiled.Events.Handlers.Player.ChangingItem += Player_ChangingItem;
            Exiled.Events.Handlers.Player.UsedItem += Player_UsedItem;
            Exiled.Events.Handlers.Player.MakingNoise += Player_MakingNoise;
            base.SubscribeEvents();
        }

        private void Player_MakingNoise(MakingNoiseEventArgs ev)
        {
            UsingItem(ev.Player.CurrentItem, ev.Player);
        }

        private void Player_UsedItem(UsedItemEventArgs ev)
        {
            UsingItem(ev.Item, ev.Player);
        }

        private void Player_ChangingItem(ChangingItemEventArgs ev)
        {
            //((Radio)ev.Player.CurrentItem).IsEnabled
            UsingItem(ev.NewItem, ev.Player);
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
                r.SetRangeSettings(Exiled.API.Enums.RadioRange.Long, new() { IdleUsage = 100f, MaxRange = 99, TalkingUsage = 20 });
                r.SetRangeSettings(Exiled.API.Enums.RadioRange.Ultra, new() { IdleUsage = 100f, MaxRange = 99, TalkingUsage = 20 });
                r.SetRangeSettings(Exiled.API.Enums.RadioRange.Medium, new() { IdleUsage = 100f, MaxRange = 99, TalkingUsage = 20 });
                r.SetRangeSettings(Exiled.API.Enums.RadioRange.Short, new() { IdleUsage = 100f, MaxRange = 99, TalkingUsage = 20 });

                StartUsing(p);

            } else
            {
                StoppedUsing(p);
            }
        }

        private void StartUsing(Player p)
        {
            var vc = voiceChannel[1];
            vc.Add(p.Id);
            voiceChannel[1] = vc;
        }
        private void StoppedUsing(Player player)
        {
            var vc = voiceChannel[1];
            vc.RemoveAll(x => x == player.Id);
            voiceChannel[1] = vc;
        }

        /*protected override void OnChanging(ChangingItemEventArgs ev)
        {
            
            if (Check(ev.NewItem)) {

                ((Radio)ev.NewItem).SetRangeSettings(Exiled.API.Enums.RadioRange.Long, new() { IdleUsage = 100f, MaxRange = 99, TalkingUsage = 20 });
                ((Radio)ev.NewItem).SetRangeSettings(Exiled.API.Enums.RadioRange.Ultra, new() { IdleUsage = 100f, MaxRange = 99, TalkingUsage = 20 });
                ((Radio)ev.NewItem).SetRangeSettings(Exiled.API.Enums.RadioRange.Medium, new() { IdleUsage = 100f, MaxRange = 99, TalkingUsage = 20 });
                ((Radio)ev.NewItem).SetRangeSettings(Exiled.API.Enums.RadioRange.Short, new() { IdleUsage = 100f, MaxRange = 99, TalkingUsage = 20 });

                var vc = voiceChannel[1];
                vc.Add(ev.Player.Id);
                voiceChannel[1] = vc;
            } else
            {
                var vc = voiceChannel[1];
                vc.RemoveAll(x => x == ev.Player.Id);
                voiceChannel[1] = vc;
            }
        }*/
    }
}
