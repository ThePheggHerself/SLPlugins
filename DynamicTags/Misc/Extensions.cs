using Newtonsoft.Json.Linq;
using PluginAPI.Core.Factories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DynamicTags
{
	public static class Extensions
	{
		public static string RejectReasonNoPerms(this string permission) => $"You do not have the required permission to execute this command: {permission}";
		public static List<ReferenceHub> GetPlayersFromString(string users)
		{
			var allHubs = ReferenceHub.AllHubs;
			if (users.ToLower() == "*")
				return allHubs.Where(p => !string.Equals(p.nicknameSync.MyNick, "Dedicated Server", StringComparison.OrdinalIgnoreCase)).ToList();

			string[] playerStrings = users.Split('.');
			List<ReferenceHub> playerList = new List<ReferenceHub>();

			foreach (string player in playerStrings)
			{
				if (int.TryParse(player, out int id) && ReferenceHub.TryGetHub(id, out ReferenceHub hub))
					playerList.Add(hub);
				else
				{
					var index = allHubs.Where(p => p.PlayerId == id || p.characterClassManager.UserId == player || (player.Length > 2 && p.nicknameSync.MyNick.ToLower().Contains(player)));
					if (index.Any())
					{
						playerList.Add(index.First());
					}
				}
			}

			return playerList;
		}
		public static bool TryParseJSON(string json, out JObject jObject)
		{
			try
			{
				jObject = JObject.Parse(json);
				return true;
			}
			catch
			{
				jObject = null;
				return false;
			}
		}
	}
}
