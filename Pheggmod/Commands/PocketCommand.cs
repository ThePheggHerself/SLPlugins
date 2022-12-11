using CommandSystem;
using NWAPIPermissionSystem;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Pheggmod.Commands
{
	[CommandHandler(typeof(RemoteAdminCommandHandler))]
	public class PocketCommand : ICommand, IUsageProvider
	{
		public string Command => "pocket";

		public string Permission => "pheggmod.pocket";

		public string[] Aliases => null;

		public string Description => "Teleports the player into the pocket dimention";

		public string[] Usage { get; } = { "%player%" };

		public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
		{
			if (!sender.CheckPermission(Permission))
			{
				response = Permission.RejectReasonNoPerms();
				return false;
			}

			var hubs = Extensions.GetPlayersFromString(arguments.Array[1]);

			foreach (ReferenceHub refhub in hubs)
				refhub.playerMovementSync.OverridePosition(Vector3.down * 1998.5f);

			response = $"Teleported {hubs.Count} {(hubs.Count == 1 ? "player" : "players")} to the pocket dimension";
			return true;
		}
	}
}
