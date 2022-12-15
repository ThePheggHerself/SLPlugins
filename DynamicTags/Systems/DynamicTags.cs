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

namespace DynamicTags.Systems
{
	public class DynamicTags
	{
		public static Dictionary<string, TagData> Tags = new Dictionary<string, TagData>();

		[PluginEvent(ServerEventType.WaitingForPlayers)]
		public void OnWaitingForPlayers()
		{
			//Clears all previous tags held by the server (Prevents players from keeping tags when they have been removed from the external server).
			Tags.Clear();

			using (WebClient client = new WebClient())
			{
				//Downloads a JSON string recieved by the server.
				//TODO: Make generic list rather than relying on the tdlist field.

				var jObj = JsonConvert.DeserializeObject<JObject>(client.DownloadString(Plugin.Config.ApiEndpoint + "GetTags.php"))["tdlist"].ToObject<List<JObject>>();

				foreach (var a in jObj)
				{
					//Parses the data from the jSON string downloaded by the server.
					bool.TryParse(a["resSlot"].ToString(), out bool hasSlot);
					string prefix = a["prefix"].ToString();
					string suffix = a["suffix"].ToString();
					string colour = a["colour"].ToString();
					ulong.TryParse(a["perms"].ToString(), out ulong perms);

					string group = a["group"].ToString();

					//TODO: Merge these together into single "UserID" field.
					string discordID = a["discordID"].ToString() + "@discord";
					string userID = a["steamID"].ToString() + (a["steamID"].ToString().StartsWith("7656") ? "@steam" : "@northwood");

					//Adds the tags to the tag list.
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

			Log.Info($"{Tags.Count} tags loaded");
		}

		[PluginEvent(ServerEventType.PlayerCheckReservedSlot)]
		public void OnReservedSlotCheck(string userid, bool hasReservedSlot)
		{
			//Checks if the user has a reserved slot set by the external server. Northwood staff are automatically given a slot.
			//TODO: Make northwood bypass a config option
			if (userid.ToLowerInvariant().Contains("northwood") || (Tags.ContainsKey(userid) && Tags[userid].HasReservedSlot))
				hasReservedSlot = true;
		}

		[PluginEvent(ServerEventType.PlayerJoined)]
		public void OnPlayerJoin(Player player)
		{
			//Checks if the user has a tag
			if (Tags.ContainsKey(player.UserId))
			{
				TagData data = Tags[player.UserId];

				//This is to stop situations where users have locally assigned perms but gets overridden by NULL perms from the external server.
				if (!string.IsNullOrEmpty(data.Group))
					player.ReferenceHub.serverRoles.SetGroup(ServerStatic.GetPermissionsHandler().GetGroup(data.Group), true);

				player.ReferenceHub.serverRoles.SetText(data.FullTag);
				player.ReferenceHub.serverRoles.SetColor(data.Colour);

				if (data.Permissions != 0)
					player.ReferenceHub.serverRoles.Permissions = data.Permissions;

				player.SendConsoleMessage("Dynamic tag loaded: " + data.FullTag);
				Log.Info($"Tag found for {player.UserId}: {data.FullTag}");
			}
		}
	}
}
