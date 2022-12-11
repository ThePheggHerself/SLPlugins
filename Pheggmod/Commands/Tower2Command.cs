using CommandSystem;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Pheggmod.Commands
{
	[CommandHandler(typeof(RemoteAdminCommandHandler))]
	public class Tower2Command : ICommand, IUsageProvider
	{
		public string Command => "tower2";

		public string[] Aliases => null;

		public string Description => "Teleports the player to a second tower on the surface";

		public string[] Usage { get; } = { "%player%" };

		public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
		{
			var hubs = Extensions.GetPlayersFromString(arguments.Array[1]);

			foreach (ReferenceHub refhub in hubs)
			{
				refhub.playerMovementSync.AddSafeTime(2f);

				if (refhub.characterClassManager.CurClass == RoleType.Spectator)
					refhub.characterClassManager.SetClassID(RoleType.Tutorial, CharacterClassManager.SpawnReason.ForceClass);
				refhub.playerMovementSync.OverridePosition(new Vector3(-21, 1021f, -43));
			}

			response = $"Teleported {hubs.Count} {(hubs.Count == 1 ? "player" : "players")} to tower 2";
			return true;
		}
	}
}
