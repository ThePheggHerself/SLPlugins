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

namespace DynamicTags.Systems
{
	public class Tracker
	{
		[PluginEvent(ServerEventType.PlayerPreauth)]
		public void OnPreauth(string userid, string ipAddress, long expiration, CentralAuthPreauthFlags flags, string region, byte[] signature, ConnectionRequest connectionRequest, int readerStartPosition)
		{
			//if (flags.HasFlag(CentralAuthPreauthFlags.NorthwoodStaff))
			//	return;

			using (WebClient client = new WebClient())
			{
				if (Extensions.TryParseJSON(client.UploadString(Plugin.Config.ApiEndpoint + "PlayerJoin.php", JsonConvert.SerializeObject(new PlayerDetails
				{
					UserId = userid,
					UserName = "null",
					Address = ipAddress,
					ServerAddress = Server.ServerIpAddress,
					ServerPort = Server.Port.ToString()
				})), out JObject jObj))
				{
					//Checks if the external server has blocked the player from joining.
					if (bool.TryParse(jObj["block"].ToString(), out bool shouldKick) && shouldKick)
					{
						Log.Info($"{userid}'s connection request was rejected. Reason: {jObj["serverReason"].ToString()}");
						NetDataWriter Writer = new NetDataWriter();
						Writer.Put((byte)RejectionReason.Custom);
						Writer.Put(jObj["reason"].ToString());
						connectionRequest.RejectForce(Writer);
					}
				}
			}
		}

		[PluginEvent(ServerEventType.PlayerLeft)]
		public void OnPlayerLeave(Player player)
		{
			using (WebClient client = new WebClient())
			{
				client.UploadString(Plugin.Config.ApiEndpoint + "PlayerLeave.php", JsonConvert.SerializeObject(new PlayerDetails
				{
					UserId = player.UserId,
					UserName = player.Nickname.Replace(':', ' '),
					Address = player.IpAddress,
					ServerAddress = Server.ServerIpAddress,
					ServerPort = Server.Port.ToString()
				}));
			}
		}

		[PluginEvent(ServerEventType.PlayerBanned)]
		public void OnPlayerBanned(Player player, Player admin, string reason, long duration)
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
