using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using PluginAPI.Core.Attributes;
using PluginAPI.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using PluginAPI.Core;

namespace Pheggmod.Systems
{
	public class DynamicTags
	{
		public static Dictionary<string, TagData> Tags = new Dictionary<string, TagData>();

		[PluginEvent(ServerEventType.WaitingForPlayers)]
		void OnWaitingForPlayers()
		{
			Tags.Clear();

			using (WebClient client = new WebClient())
			{
				var jObj = JsonConvert.DeserializeObject<List<JObject>>(client.DownloadString(Plugin.Config.ApiEndpoint + "GetTags.php"));

				if (Tags?.Count > 0)
				{
					foreach (var a in jObj)
					{
						bool.TryParse(a["resSlot"].ToString(), out bool hasSlot);
						string prefix = a["prefix"].ToString();
						string suffix = a["suffix"].ToString();
						string colour = a["colour"].ToString();
						ulong.TryParse(a["perms"].ToString(), out ulong perms);

						string group = a["group"].ToString();

						string discordID = a["discordID"].ToString() + "@discord";
						string userID = a["steamID"].ToString() + (a["steamID"].ToString().StartsWith("7656") ? "@steam" : "@northwood");

						Tags.Add(discordID, new TagData
						{
							UserID = discordID,
							Prefix = prefix,
							Suffix = suffix,
							Colour = colour,
							HasReservedSlot = hasSlot,
							Permissions = perms,
							Group = group
						});
						Tags.Add(userID, new TagData
						{
							UserID = userID,
							Prefix = prefix,
							Suffix = suffix,
							Colour = colour,
							HasReservedSlot = hasSlot,
							Permissions = perms,
							Group = group
						});
					}
				}
			}
		}

		[PluginEvent(ServerEventType.PlayerCheckReservedSlot)]
		void OnReservedSlotCheck(string userid, bool hasReservedSlot)
		{
			if (userid.ToLowerInvariant().Contains("northwood") || (Tags.ContainsKey(userid) && Tags[userid].HasReservedSlot))
				hasReservedSlot = true;
		}

		[PluginEvent(ServerEventType.PlayerJoined)]
		void OnPlayerJoin(Player player)
		{
			if (Tags.ContainsKey(player.UserId))
			{
				TagData data = Tags[player.UserId];

				player.ReferenceHub.serverRoles.SetGroup(ServerStatic.GetPermissionsHandler().GetGroup(data.Group), true);
				player.ReferenceHub.serverRoles.SetText(data.FullTag);
				player.ReferenceHub.serverRoles.SetColor(data.Colour);

				if (data.Permissions != 0)
					player.ReferenceHub.serverRoles.Permissions = data.Permissions;
			}
		}
	}
}
