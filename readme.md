# Extra eXiled Utils

A collection of utilites for the exiled plugin loader of SCP Secret Lab.
<br> Currently supported Exiled Version: `7.0.0-rc5`+

Collated into modules for easy use.


| Module | Use | Link |
| ------ | --- | ---- | 
| Core | -- | [Core](#Core) |
| Scp Select | Control which SCP's can spawn | [Scp Select](#Core) |
| Player Assist | Control certain players spawning role | [Player Assist](#Scp-Select) |
| Detain Use | Provides a win-condition use to detaining roles | [Detain Use](#Player-Assist) |
| Command Watcher | Watch for pesky mod abuse | [Command Watcher](#Command-Watcher) |
| Realistic Elevators | Elevators have realistic physical distance length | [Realistic Elevators](#Realistic-Elevators) |
| Babel Radio | An SCP Item that can be found in the game, that allows players to listen to the scp chat | [Babel Radio](#Babel-Radio) |

## Modules

### Core

Basic plugin.

| Setting | Use | Default |
| ------- | --- | ------- |
| `is_enabled` | enable the plugin | true |
| `debug` | print debug text | false | 


### Scp Select

Adds a command that allows server mods to choose which SCP roles can spawn.
In effect, the server will iterate over the currently spawned SCP's, and replace them for a role on the list.
If there are more SCP's than roles, they will stay as their original.
If there are multiple 079's, all but 1 will be switched to a valid SCP role.

**Commands**
<details>
	<summary> <b>scpselect</b> </summary>
	Usage: scpselect (add/remove/list/all) [RoleType] <br> <br>
	<b>all</b> - list all possible RoleType Enum values. <br>
	<b>list</b> - lists the currently active RoleTypes. <br>
	<b>add</b> - chooses a RoleType to switch a SCP to. <br>
	<b>remove</b> - removes a RoleType to switch a SCP to. <br>
</details>

| Setting | Use | Default |
| ------- | --- | ------- |
| `is_enabled` | enable the module | true |

### Player Assist

Adds a command that allows server mods to dictate if a player will spawn as a DClass on that round.
Great for teaching new players how to play the game!
Upon game start, if the player selected isn't already an DClass, they will switch roles with one of the DClass.

**Commands**
<details>
	<summary> <b>playerassist</b> </summary>
	Usage: (playerassist/pa) [playername] <br> <br>
	Inputed player name will toggle between their forced DClass role upon game start.
	Doesn't persist between rounds.
</details>

| Setting | Use | Default |
| ------- | --- | ------- |
| `is_enabled` | enable the module | true | 

### Detain Use

Provides a use to detaining DClass and/or Scientists for retrieval.
By 1: Making the detained roles the same as the normal escapee counterparts
And 2: Makes the detained class count towards the opposite win condition.

| Setting | Use | Default |
| ------- | --- | ------- |
| `is_enabled` | enable the module | true |

### Command Watcher

Alerts admins of pesky mod abuse. 
Places a broadcast on screen and sends a full message to the console.

| Setting | Use | Default |
| ------- | --- | ------- |
| `is_enabled` | enable the module | true |

### Realistic Elevators

Elevator's have realistic speeds based on their distances physically within the world

| Setting | Use | Default |
| ------- | --- | ------- |
| `is_enabled` | enable the module | false |
| `surface_timer_extention` | increase surface lifts delay by x amount | 1 |
| `light_timer_extention` | increase light lifts delay by x amount | 0.5 |


### Babel Radio

A Custom SCP item that can be found in pedestals around the map.
This item, when active, allows the players to listen to the SCP's chat - whilst also acting as a normal radio.
They cannot speak to the SCP's only listen, as was the nature of the babel fish in Hitchhikers.

| Settings | Use | Default |
| -------- | --- | ------- |
| `is_enabled` | enable module | true |
| `is_anonymous` | anonymous scp chat audio | false |
| `can_speak` | allows the radio user to communicate with the the SCP's | false |
| `spawnproperties` | radio spawn properties | see config file |
| `radio_idle_usage` | the idle usage of the radio | 160 (roughly a minute of usage) |
| `radio_max_range` | the max listening range for normal radio functionality | 99 |
| `talking_usage` | the usage when talking over the radio | 20 |
| `radio_to_vc` | the channel users speaking with `can_speak` speak into, that is heard by the SCP's | RoundSummary |

It is recommended you do not change radio_to_vc unless you know what you are doing.
