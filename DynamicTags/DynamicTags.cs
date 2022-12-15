using PluginAPI.Core;
using PluginAPI.Core.Attributes;
using PluginAPI.Core.Factories;
using PluginAPI.Events;

namespace DynamicTags
{
	public class Plugin
	{
		[PluginConfig]
		public static DynamicTagsConfig Config;



		[PluginEntryPoint("Dynamic Tags & Tracker", "1.0.0", "Simple plugin to handle dynamic tags and player tracking via external APIs", "ThePheggHerself")]
		public void OnPluginStart()
		{
			Log.Info($"Plugin is loading...");

			//Registers the events used in the DynamicTags class
			EventManager.RegisterEvents<Systems.DynamicTags>(this);

			//Registers the events used in the Tracker class
			EventManager.RegisterEvents<Systems.Tracker>(this);

			Log.Info($"Plugin is loaded. API Endpoint is: {Config.ApiEndpoint}");

		}
	}
}
