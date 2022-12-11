using CommandSystem;
using InventorySystem;
using NWAPIPermissionSystem;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pheggmod.Commands
{
	public class DropCommand : ICommand, IUsageProvider
	{
		public string Command => "drop";

		public string Permission => "pheggmod.drop";

		public string[] Aliases { get; } = { "dropall", "dropinv", "strip" };

		public string Description => "Drops all items and ammo from the specified player(s)";

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
				refhub.inventory.ServerDropEverything();


			response = $"Player {(hubs.Count > 1 ? "inventories" : "inventory")} dropped";


			return true;
		}
	}
}
