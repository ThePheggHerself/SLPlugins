using PlayerRoles.PlayableScps.Scp939;
using PlayerStatsSystem;
using PluginAPI.Core;
using System;
using System.Collections.Generic;
using System.Text;

namespace DiscordLab
{
	public static class Extensions
	{
		public static string GetDamageSource(this AttackerDamageHandler aDH)
		{
			if (aDH is FirearmDamageHandler fDH)
				return fDH.WeaponType.ToString();
			else if (aDH is ExplosionDamageHandler eDH)
				return "Grenade";
			else if (aDH is MicroHidDamageHandler mhidDH)
				return "Micro HID";
			else if (aDH is RecontainmentDamageHandler reconDH)
				return "Recontainment";
			else if (aDH is Scp018DamageHandler scp018DH)
				return "SCP 018";
			else if (aDH is Scp096DamageHandler scp096DH)
				return "SCP 096";
			else if (aDH is Scp049DamageHandler scp049DH)
				return "SCP 049";
			else if (aDH is Scp939DamageHandler scp939DH)
				return "SCP 939";
			else if (aDH is ScpDamageHandler scpDH)
				return scpDH.Attacker.Hub.roleManager.CurrentRole.name;

			else return "THIS SHOULD NEVER APPEAR!!!";
		}

		public static string ToLogString(this Player plr) => $"{plr.Nickname} ({plr.UserId})";
	}
}
