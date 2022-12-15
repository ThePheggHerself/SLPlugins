using MEC;
using PluginAPI.Core;
using PluginAPI.Core.Attributes;
using PluginAPI.Core.Factories;
using PluginAPI.Events;
using System;
using System.Net;

namespace DiscordLab
{
	public class DiscordLab
	{
		[PluginConfig]
		public static DiscordLabConfig Config;
		public static BotConnector Bot;

		[PluginEntryPoint("DiscordLab", "1.0.0", "Bridge between SCP:SL servers, and Discord", "ThePheggHerself")]
		public void OnPluginStart()
		{
			Log.Info($"Plugin is loading...");

			Bot = new BotConnector();

			EventManager.RegisterEvents<Events>(this);

			//FactoryManager.RegisterPlayerFactory(this, new PlayerFactory());

			if (string.IsNullOrEmpty(Config.Address))
				Config.Address = Server.ServerIpAddress;

			Log.Info($"Plugin has fully loaded");
		}
	}
}
