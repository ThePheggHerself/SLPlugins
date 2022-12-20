using PlayerStatsSystem;
using PluginAPI.Core.Attributes;
using PluginAPI.Core;
using PluginAPI.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PlayerRoles;
using static UnityEngine.GraphicsBuffer;
using static FriendlyFireDetector.DataClasses;
using UnityEngine;
using AdminToys;
using CustomPlayerEffects;

namespace FriendlyFireDetector
{
	public class Handler
	{
		public readonly Dictionary<string, FFInfo> ffInfo = new Dictionary<string, FFInfo>();

		[PluginEvent(ServerEventType.PlayerDamagedShootingTarget)]
		public void TargetDamagedEvent(Player attacker, ShootingTarget target, DamageHandlerBase damageHandler, float amount)
		{
			if (target.CommandName.ToLowerInvariant().Contains("dboy"))
				UpdateLegitDamage(attacker, false);
			if (target.CommandName.ToLowerInvariant().Contains("sport"))
			{
				UpdateFFDamage(attacker);
				HandlePunishments(attacker, amount);
			}
		}


		[PluginEvent(ServerEventType.PlayerDamage)]
		public bool PlayerDamageEvent(Player victim, Player attacker, DamageHandlerBase damageHandler)
		{
			if (!(damageHandler is AttackerDamageHandler aDH))
				return true;

			if (victim.IsServer || aDH.IsSuicide || attacker.ReferenceHub.IsSCP() || victim.ReferenceHub.IsSCP())
				return true;

			//Updates the legitimate damage. Caps the players "Value" buffer at 5, allowing up to 5 instances of FF damage before reversal becomes active
			if (!IsFF(victim, attacker))
			{
				UpdateLegitDamage(attacker);
				return true;
			}

			if (aDH is ExplosionDamageHandler eADH) { }
			else
			{
				FFInfo info = GetInfo(attacker);
				List<Player> friendlies = new List<Player>();
				List<Player> hostiles = new List<Player>();

				foreach (var plr in GetNearbyPlayers(attacker))
				{
					if (IsFF(plr, attacker))
						friendlies.Add(plr);
					else hostiles.Add(plr);
				}

				if (hostiles.Count > 0 || (DateTime.Now - info.LastUpdate).TotalSeconds < 2)
				{
					UpdateLegitDamage(attacker, false);
					return true;
				}
				else
				{
					info.Value = Mathf.Clamp(info.Value + 1, -5, 15);

					Log.Info($"{attacker.Nickname} | {info.Value} | {hostiles.Count} | {(DateTime.Now - info.LastUpdate).TotalSeconds}");
					return false;
				}
			}

			return true;
		}

		public static bool IsFF(Player victim, Player attacker)
		{
			if ((victim.Role == RoleTypeId.ClassD || victim.ReferenceHub.roleManager.CurrentRole.Team == Team.ChaosInsurgency) && (attacker.Role == RoleTypeId.ClassD || victim.ReferenceHub.roleManager.CurrentRole.Team == Team.ChaosInsurgency))
			{
				if (victim.Role == RoleTypeId.ClassD && attacker.Role == RoleTypeId.ClassD)
					return false;
				return true;
			}
			else if ((victim.Role == RoleTypeId.Scientist || victim.ReferenceHub.roleManager.CurrentRole.Team == Team.FoundationForces) && (attacker.Role == RoleTypeId.Scientist || victim.ReferenceHub.roleManager.CurrentRole.Team == Team.FoundationForces))
				return true;

			return false;
		}

		public FFInfo GetInfo(Player player)
		{
			if (!ffInfo.ContainsKey(player.UserId))
				ffInfo.Add(player.UserId, new FFInfo());

			return ffInfo[player.UserId];
		}

		public void UpdateLegitDamage(Player player, bool updateTime = true)
		{
			FFInfo info = GetInfo(player);

			info.Value = Mathf.Clamp(info.Value - 1, -5, 15);
			if (updateTime)
				info.LastUpdate = DateTime.Now;
		}

		public void UpdateFFDamage(Player player)
		{
			FFInfo info = GetInfo(player);

			info.Value = Mathf.Clamp(info.Value + 1, -5, 15);
		}

		public void HandlePunishments(Player player, float damage)
		{
			FFInfo info = GetInfo(player);

			if (info.Value > 2)
				player.Damage(damage, "Anti-FF: Damage reversal due to friendly fire");
			if (info.Value > 4)
				player.DropEverything();
			if(info.Value > 6)
			{
				player.EffectsManager.EnableEffect<Deafened>(info.Value);
				player.EffectsManager.EnableEffect<Blinded>(info.Value);
				player.EffectsManager.EnableEffect<Disabled>(info.Value * 2);
			}
			if(info.Value > 9)
			{
				player.Kill();
			}
			if(info.Value > 12)
			{
				player.Kick("Anti-FF: Automatic kick for too much friendly damage");
			}
			if(info.Value > 14)
			{
				player.Ban("Anti-FF: Automatic ban for too much friendly damage", 1440 * 60);
			}
		}

		/// <summary>
		/// Gets a list of all players close to the attacker (100 meters for Surface, 50 for the facility)
		/// </summary>
		/// <returns></returns>
		public List<Player> GetNearbyPlayers(Player atkr)
		{
			float distanceCheck = atkr.Position.y > 900 ? 90 : 40;
			List<Player> nearbyPlayers = new List<Player>();

			foreach (var plr in Server.GetPlayers())
			{
				if (plr.IsServer)
					continue;

				var distance = Vector3.Distance(atkr.Position, plr.Position);
				var angle = Vector3.Angle(atkr.GameObject.transform.forward, atkr.Position - plr.Position);

				if ((distance <= distanceCheck && angle > 130) || distance < 5)
					nearbyPlayers.Add(plr);
			}

			return nearbyPlayers;
		}
	}
}
