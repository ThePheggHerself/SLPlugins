using PluginAPI.Core;
using PluginAPI.Core.Attributes;
using PluginAPI.Core.Factories;
using PluginAPI.Events;

namespace FriendlyFireDetector
{
    public class Plugin
    {
		[PluginConfig]
		public static Config Config;

		[PluginEntryPoint("Friendly Fire Detector", "1.0.0", "Anti-Friendly Fire system", "ThePheggHerself")]
		public void OnPluginStart()
		{
			Log.Info($"Plugin is loading...");

			EventManager.RegisterEvents<Handler>(this);
		}
	}
}
