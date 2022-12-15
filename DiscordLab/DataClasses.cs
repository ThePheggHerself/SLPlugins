using PluginAPI.Core;
using System;
using System.Collections.Generic;
using System.Text;

namespace DiscordLab
{
	public class msgBase
	{
		public msgBase() { }
		public string Type;
	}

	public class msgMessage : msgBase
	{
		public msgMessage(string message)
		{
			Type = "msg";
			Message = BotConnector.MsgRgx.Replace(message, string.Empty);
		}

		public string Message;
	}
	public class msgCommand : msgBase
	{
		public msgCommand(string channelID, string staffID, string msg)
		{
			ChannelID = channelID;
			StaffID = staffID;
			CommandMessage = msg;
			Type = "cmdmsg";
		}

		public string CommandMessage;
		public string ChannelID;
		public string StaffID;
	}
	public class msgPList : msgBase
	{
		public msgPList(string channelId)
		{
			Type = "plist";
			ChannelID = channelId;

			var players = Player.GetPlayers();

			if (Player.Count < 1)
			{
				if ((DateTime.Now - Events.RoundEndTime).TotalSeconds < 45)
					PlayerNames = "***The round has recently restarted, and players are rejoining";
				else
					PlayerNames = "**No online players**";
			}
			else
			{
				int DntCount = 0;
				List<string> plrNames = new List<string>();

				foreach (Player plr in players)
				{
					if (plr.IsServer)
						continue;
					if (!plr.DoNotTrack)
						plrNames.Add(BotConnector.NameRgx.Replace(plr.Nickname, string.Empty));
					else
						DntCount++;
				}

				if (DntCount > 0)
					plrNames.Add($"{(Player.Count > 1 ? "and " : "")}{DntCount}{(Player.Count > 1 ? " other" : "")} DNT user{(DntCount > 1 ? "s" : "")}");

				PlayerNames = $"**{Player.Count}/{Server.MaxPlayers}**\n```\n{string.Join(", ", plrNames)}```";
			}
		}

		public string PlayerNames;
		public string ChannelID;
	}
	public class msgStatus : msgBase
	{
		public msgStatus()
		{
			Type = "supdate";
			CurrentPlayers = Player.Count + "/" + Server.MaxPlayers;
			LastUpdate = DateTime.Now;
		}

		public string CurrentPlayers { get; }
		internal DateTime LastUpdate { get; }
	}
	public class msgKeepAlive : msgBase
	{
		public msgKeepAlive()
		{
			Type = "keepalive";
		}
	}

	public class botmessage
	{
		//Types: plist, command

		public string Type;
		public string channel;
		public string Message = null;
		public string StaffID = null;
		public string Staff = null;
	}

	public enum messageType
	{
		MSG = 0,
		CMD = 1,
		PLIST = 2,
		SUPDATE = 3,
		KEEPALIVE = 4
	}
}
