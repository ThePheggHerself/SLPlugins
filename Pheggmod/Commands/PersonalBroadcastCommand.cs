using CommandSystem;
using Mirror;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Pheggmod;
using UnityEngine.Android;

namespace Pheggmod.Commands
{
	[CommandHandler(typeof(RemoteAdminCommandHandler))]
	public class PersonalBroadcastCommand : ICommand
	{
		public string Command => "pbc";
		public string[] Aliases { get; } = { "personalbroadcast", "privatebroadcast" };
		public string Description => "Sends a private broadcast message to the specified player(s)";

		public string[] Usage { get; } = { "%player%", "duration", "message" };

		public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
		{
			if (!sender.CheckPermission(PlayerPermissions.Broadcasting))
			{
				response = Extensions.RejectReasonNoPerms("Broadcasting");
				return false;
			}

			var hubs = Extensions.GetPlayersFromString(arguments.Array[1]);

			if (!ushort.TryParse(arguments.Array[2], out ushort duration) || duration < 1 || duration > 254)
			{
				response = "Invalid duration given";
				return false;
			}

			string message = $"<color=#FFA500><b>[Private]</b></color> <color=green>{string.Join(" ", arguments.Skip(2))}</color>";

			GameObject senderObject = PlayerManager.players.Where(p => p.GetComponent<NicknameSync>().MyNick == ((CommandSender)sender).Nickname).FirstOrDefault();
			if (senderObject != null)
				hubs.Add(senderObject.GetComponent<ReferenceHub>());

			foreach (ReferenceHub refhub in hubs)
			{
				GameObject go = refhub.gameObject;
				go.GetComponent<Broadcast>().TargetAddElement(go.GetComponent<NetworkConnection>(), message, duration, Broadcast.BroadcastFlags.Normal);
			}

			response = "Broadcast sent";
			return true;
		}
	}
}
