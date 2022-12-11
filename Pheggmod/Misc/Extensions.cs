using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pheggmod
{
	public static class Extensions
	{
		public static string RejectReasonNoPerms(this string permission) => $"You do not have the required permission to execute this command: {permission}";
		public static List<ReferenceHub> GetPlayersFromString(string users)
		{
			if (users.ToLower() == "*")
				return ReferenceHub.GetAllHubs().Values.Where(p => !string.Equals(p.nicknameSync.MyNick, "Dedicated Server", StringComparison.OrdinalIgnoreCase)).ToList();

			string[] playerStrings = users.Split('.');
			List<ReferenceHub> playerList = new List<ReferenceHub>();
			List<ReferenceHub> hubs = ReferenceHub.GetAllHubs().Values.ToList();

			foreach (string player in playerStrings)
			{
				if (int.TryParse(player, out int id) && ReferenceHub.TryGetHub(id, out ReferenceHub hub))
					playerList.Add(hub);
				else
				{
					int index = hubs.FindIndex(p => p.queryProcessor.PlayerId.ToString() == player || p.characterClassManager.UserId == player || (player.Length > 2 && p.nicknameSync.MyNick.ToLower().Contains(player)));
					if (index > -1)
					{
						playerList.Add(hubs[index]);
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
