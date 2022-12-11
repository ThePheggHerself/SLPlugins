using CommandSystem;
using InventorySystem.Items.ThrowableProjectiles;
using NWAPIPermissionSystem;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Android;

namespace Pheggmod.Commands
{
	[CommandHandler(typeof(RemoteAdminCommandHandler))]
	public class FlashCommand : ICommand, IUsageProvider
	{
		public string Command => "flash";

		public string Permission => "pheggmod.flash";

		public string[] Aliases => null;

		public string Description => "Spawns a flashbang at a player's location";

		public string[] Usage { get; } = { "%player%" };

		public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
		{
			if (!sender.CheckPermission(Permission))
			{
				response = Permission.RejectReasonNoPerms();
				return false;
			}

			var hubs = Extensions.GetPlayersFromString(arguments.Array[1]);

			foreach (ReferenceHub plr in hubs)
			{
				if (plr.characterClassManager.CurClass == RoleType.Spectator)
					continue;

				ThrowableItem ItemBase = (ThrowableItem)plr.inventory.CreateItemInstance(ItemType.GrenadeFlash, false);

				Vector3 Pos = plr.playerMovementSync.GetRealPosition();
				Pos.y += 1;

				FlashbangGrenade grenade = (FlashbangGrenade)UnityEngine.Object.Instantiate(ItemBase.Projectile, Pos, Quaternion.identity);

				grenade.PreviousOwner = new Footprinting.Footprint(plr);
				Mirror.NetworkServer.Spawn(grenade.gameObject.gameObject);
				grenade.ServerActivate();
			}

			response = $"#Spawned grenade on {hubs.Count} {(hubs.Count > 1 ? "players" : "player")}";
			return true;
		}
	}
}
