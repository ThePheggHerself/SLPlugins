using LiteNetLib.Utils;
using LiteNetLib;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using PluginAPI.Core.Attributes;
using PluginAPI.Core;
using PluginAPI.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Pheggmod
{
	public class Tracker
	{
		[PluginEvent(ServerEventType.PlayerPreauth)]
		void OnPreauth(string userid, string ipAddress, long expiration, CentralAuthPreauthFlags flags, string country, byte[] signature, ConnectionRequest req, Int32 index)
		{
			if (flags.HasFlag(CentralAuthPreauthFlags.NorthwoodStaff))
				return;

			using (WebClient client = new WebClient())
			{
				if (Extensions.TryParseJSON(client.UploadString(Plugin.Config.ApiEndpoint + "PlayerJoin.php", JsonConvert.SerializeObject(new PlayerDetails
				{
					UserId = userid,
					UserName = "null",
					Address = ipAddress,
					ServerAddress = ServerConsole.Ip,
					ServerPort = ServerConsole.Port.ToString()
				})), out JObject jObj))
				{
					if (bool.TryParse(jObj["block"].ToString(), out bool shouldKick) && shouldKick)
					{
						Log.Info($"{userid}'s connection request was rejected. Reason: {jObj["serverReason"].ToString()}");
						NetDataWriter Writer = new NetDataWriter();
						Writer.Put((byte)RejectionReason.Custom);
						Writer.Put(jObj["reason"].ToString());
						req.RejectForce(Writer);
					}

					Log.Info(JsonConvert.SerializeObject(jObj));
				}
			}
		}

		[PluginEvent(ServerEventType.PlayerLeft)]
		void OnPlayerLeave(Player player)
		{
			using (WebClient client = new WebClient())
			{
				client.UploadString(Plugin.Config.ApiEndpoint + "PlayerLeave.php", JsonConvert.SerializeObject(new PlayerDetails
				{
					UserId = player.UserId,
					UserName = player.Nickname.Replace(':', ' '),
					Address = player.IpAddress,
					ServerAddress = ServerConsole.Ip,
					ServerPort = ServerConsole.Port.ToString()
				}));
			}
		}

		[PluginEvent(ServerEventType.PlayerBanned)]
		void OnPlayerBanned(Player player, Player admin, string reason, long duration)
		{
			using (WebClient client = new WebClient())
			{
				client.UploadString(Plugin.Config.ApiEndpoint + "PlayerBan.php", JsonConvert.SerializeObject(new PlayerBanDetails
				{
					playerName = player.Nickname.Replace(':', ' '),
					playerID = player.UserId,
					adminName = admin.Nickname.Replace(':', ' '),
					adminID = admin.UserId,
					duration = (duration / 60).ToString(),
					reason = string.IsNullOrEmpty(reason) ? "No reason provided" : reason
				}));
			}
		}
	}
}
