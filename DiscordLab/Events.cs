using GameCore;
using InventorySystem.Items.Pickups;
using InventorySystem.Items.ThrowableProjectiles;
using LiteNetLib;
using PlayerRoles;
using PlayerStatsSystem;
using PluginAPI.Core;
using PluginAPI.Core.Attributes;
using PluginAPI.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace DiscordLab
{
	public class Events
	{
		public static DateTime RoundEndTime = new DateTime(), RoundStartTime = new DateTime();

		[PluginEvent(ServerEventType.PlayerJoined)]
		public void PlayerJoinedEvent(Player plr) => DiscordLab.Bot.SendMessage(new Msg($"**{plr.Nickname} ({plr.UserId} from ||~~{plr.IpAddress}~~||) has joined the server**"));

		[PluginEvent(ServerEventType.PlayerLeft)]
		public void PlayerLeftEvent(Player plr) => DiscordLab.Bot.SendMessage(new Msg($"**{plr.Nickname} ({plr.UserId}) has disconnected from the server**"));

		[PluginEvent(ServerEventType.PlayerDeath)]
		public void PlayerDeathEvent(Player victim, Player attacker, DamageHandlerBase damageHandler)
		{
			if(damageHandler is AttackerDamageHandler aDH)
			{
				if (aDH.IsSuicide)
					DiscordLab.Bot.SendMessage(new Msg($"**{victim.ToLogString()} killed themselves using {aDH.GetDamageSource()}**"));
				else if (aDH.IsFriendlyFire)
					DiscordLab.Bot.SendMessage(new Msg($"**Teamkill** \n```autohotkey\nPlayer: {attacker.Role} {attacker.ToLogString()} \nKilled: {victim.Role} {victim.ToLogString()}\nUsing: {aDH.GetDamageSource()}```"));
				else if (victim.IsDisarmed && !attacker.ReferenceHub.IsSCP())
					DiscordLab.Bot.SendMessage(new Msg($"__Disarmed Kill__\n```autohotkey\nPlayer: {attacker.Role} {attacker.ToLogString()} \nKilled: {victim.Role} {victim.ToLogString()}\nUsing: {aDH.GetDamageSource()}```"));
				else
					DiscordLab.Bot.SendMessage(new Msg($"{attacker.Role} {attacker.Nickname} killed {victim.Role} {victim.Nickname} using {aDH.GetDamageSource()}"));
			}
			else if (damageHandler is WarheadDamageHandler wDH)
			{
				DiscordLab.Bot.SendMessage(new Msg($"{victim.ToLogString()} was vaporized by alpha warhead"));
			}
			else if (damageHandler is UniversalDamageHandler uDH)
			{
				DiscordLab.Bot.SendMessage(new Msg($"{victim.ToLogString()} died to {DeathTranslations.TranslationsById[uDH.TranslationId].LogLabel}"));
			}
		}

		[PluginEvent(ServerEventType.GrenadeExploded)]
		public void GrenadeExplodeEvent(ItemPickupBase grenade)
		{
			if(grenade is ExplosionGrenade expGrenade)
				DiscordLab.Bot.SendMessage(new Msg($"Frag grenade of {expGrenade.PreviousOwner.Nickname} has exploded"));
			else if (grenade is FlashbangGrenade flshGrenade)
				DiscordLab.Bot.SendMessage(new Msg($"Flashbang of {flshGrenade.PreviousOwner.Nickname} has exploded"));
			else if (grenade is Scp018Projectile scp018)
				DiscordLab.Bot.SendMessage(new Msg($"SCP-018 of {scp018.PreviousOwner.Nickname} has exploded"));
			else
				DiscordLab.Bot.SendMessage(new Msg($"Grenade of unknown type thrown by {grenade.PreviousOwner.Nickname} has exploded"));
		}

		[PluginEvent(ServerEventType.PlayerBanned)]
		public void PlayerBannedEvent(Player player, Player admin, string reason, long duration) => DiscordLab.Bot.SendMessage(new Msg($"**New Ban!**```autohotkey\nUser: {player.ToLogString()}\nAdmin: {admin.ToLogString()}\nDuration: {duration / 60} {(duration / 60 > 1 ? "minutes" : "minute")}\nReason: {(string.IsNullOrEmpty(reason) ? "No reason provided" : reason)}```"));

		[PluginEvent(ServerEventType.PlayerEscape)]
		public void PlayerEscapeEvent(Player player, RoleTypeId newRole) => DiscordLab.Bot.SendMessage(new Msg($"{player.Nickname} escaped the facility and became {newRole}"));

		[PluginEvent(ServerEventType.PlayerHandcuff)]
		public void PlayerDisarmEvent(Player player, Player target) => DiscordLab.Bot.SendMessage(new Msg($"{player.Nickname} has disarmed {target.Nickname}"));

		[PluginEvent(ServerEventType.PlayerRemoveHandcuffs)]
		public void PlayerUndisarmEvent(Player player, Player target) => DiscordLab.Bot.SendMessage(new Msg($"{player.Nickname} has freed {target.Nickname}"));

		[PluginEvent(ServerEventType.PlayerDamage)]
		public void PlayerDamageEvent(Player victim, Player attacker, DamageHandlerBase damageHandler)
		{
			if (damageHandler is AttackerDamageHandler aDH)
			{
				if (aDH.IsSuicide)
					DiscordLab.Bot.SendMessage(new Msg($"**{victim.ToLogString()} damaged themselves using {aDH.GetDamageSource()}**"));
				else if (aDH.IsFriendlyFire)
					DiscordLab.Bot.SendMessage(new Msg($"**{attacker.Role} {attacker.ToLogString()} damaged {victim.Role} {victim.ToLogString()} using {aDH.GetDamageSource()} for {aDH.Damage}**"));
				else if (victim.IsDisarmed && !attacker.ReferenceHub.IsSCP())
					DiscordLab.Bot.SendMessage(new Msg($"__{attacker.Role} {attacker.ToLogString()} damaged {victim.Role} {victim.ToLogString()} using {aDH.GetDamageSource()} for {aDH.Damage}__"));
				else
					DiscordLab.Bot.SendMessage(new Msg($"{attacker.Nickname} -> {victim.Nickname} -> {aDH.Damage} ({aDH.GetDamageSource()})"));
			}
			else if (damageHandler is WarheadDamageHandler wDH)
			{
				DiscordLab.Bot.SendMessage(new Msg($"{victim.ToLogString()} was partially vaporized by alpha warhead"));
			}
			else if (damageHandler is UniversalDamageHandler uDH)
			{
				//DiscordLab.Bot.SendMessage(new msgMessage($"{victim.ToLogString()} was damaged by {DeathTranslations.TranslationsById[uDH.TranslationId].LogLabel}"));
			}
		}

		[PluginEvent(ServerEventType.PlayerKicked)]
		public void PlayerKickedEvent(Player player, Player admin, string reason) => DiscordLab.Bot.SendMessage(new Msg($"**Player Kicked!**```autohotkey\nUser: {player.ToLogString()}\nAdmin: {admin.ToLogString()}\nReason: {(string.IsNullOrEmpty(reason) ? "No reason provided" : reason)}```"));

		public void PlayerPreauthEvent(string userid, string ipAddress, long expiration, CentralAuthPreauthFlags flags, string region, byte[] signature, ConnectionRequest connectionRequest, int readerStartPosition) { }

		[PluginEvent(ServerEventType.PlayerChangeRole)]
		public void RoleChangeEvent(Player player, PlayerRoleBase oldRole, RoleTypeId newRole, RoleChangeReason reason)
		{
			if (newRole == RoleTypeId.Spectator || oldRole.RoleTypeId == RoleTypeId.Spectator)
				return;

			DiscordLab.Bot.SendMessage(new Msg($"{player.ToLogString()} changed from {oldRole.RoleTypeId} to {newRole} ({reason})"));
		}

		[PluginEvent(ServerEventType.PlayerSpawn)]
		public void PlayerSpawnEvent(Player player, RoleTypeId role)
		{
			if (role == RoleTypeId.None)
				return;
			DiscordLab.Bot.SendMessage(new Msg($"{player.ToLogString()} spawned as {role}"));
		}

		[PluginEvent(ServerEventType.RoundEnd)]
		public void RoundEndEvent()
		{
			RoundEndTime = DateTime.Now;
			DiscordLab.Bot.SendMessage(new Msg($"**Round Ended**\n```Round Time: {new DateTime(TimeSpan.FromSeconds((DateTime.Now - RoundStartTime).TotalSeconds).Ticks):HH:mm:ss}"
				+ $"\nEscaped Class-D: {RoundSummary.EscapedClassD}"
				+ $"\nRescued Scientists: {RoundSummary.EscapedScientists}"
				+ $"\nSurviving SCPs: {RoundSummary.SurvivingSCPs}"
				+ $"\nWarhead Status: {(Warhead.IsDetonated ? "Not Detonated" : $"Detonated")}" 
				+ $"\nDeaths: {RoundSummary.Kills} ({RoundSummary.KilledBySCPs} by SCPs)```"));
		}

		[PluginEvent(ServerEventType.RoundStart)]
		public void RoundStartEvent()
		{
			RoundStartTime = DateTime.Now;
			DiscordLab.Bot.SendMessage(new Msg("**A new round has begun**"));
		}

		[PluginEvent(ServerEventType.WaitingForPlayers)]
		public void WaitingForPlayersEvent() => DiscordLab.Bot.SendMessage(new Msg("The server is ready and waiting for players"));

		[PluginEvent(ServerEventType.WarheadStart)]
		public void WarheadStartEvent(bool isAutomatic, Player player)
		{
			if (!isAutomatic)
				DiscordLab.Bot.SendMessage(new Msg($"{player.ToLogString()} has started the alpha warhead countdown"));
			else
				DiscordLab.Bot.SendMessage(new Msg($"The alpha warhead countdown has begun"));
		}

		[PluginEvent(ServerEventType.WarheadStop)]
		public void WarheadStopEvent(Player player) => DiscordLab.Bot.SendMessage(new Msg($"{player.ToLogString()} has suspended the alpha warhead countdown"));

		[PluginEvent(ServerEventType.WarheadDetonation)]
		public void WarheadDetonateEvent() => DiscordLab.Bot.SendMessage(new Msg($"The alpha warhead has detonated"));

		[PluginEvent(ServerEventType.PlayerMuted)]
		public void PlayerMuteEvent(Player player, bool isIntercom) => DiscordLab.Bot.SendMessage(new Msg($"{player.ToLogString()} has been {(isIntercom ? "intercom" : "")}muted"));

		[PluginEvent(ServerEventType.PlayerUnmuted)]
		public void PlayerUnmuteEvent(Player player, bool isIntercom) => DiscordLab.Bot.SendMessage(new Msg($"{player.ToLogString()} is no longer {(isIntercom ? "intercom " : "")}muted"));

		[PluginEvent(ServerEventType.PlayerRemoteAdminCommand)]
		public void RemoteAdminCommandEvent(Player player, string command, string[] arguments) => DiscordLab.Bot.SendMessage(new Msg($"{player.ToLogString()} has run the following command: **{(arguments.Length > 0 ? $"{command} {string.Join(" ", arguments)}" : $"{command}")}**"));	
	}
}
