using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using LiteNetLib;
using LiteNetLib.Utils;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using PluginAPI.Core;
using PluginAPI.Core.Attributes;
using PluginAPI.Enums;

namespace Pheggmod
{
	public class Plugin
	{


		[PluginConfig]
		public static Config Config;



		[PluginEntryPoint("Dynamic Tags & Tracker", "1.0.0", "Simple plugin to handle dynamic tags and player tracking via external APIs", "ThePheggHerself")]
		public void OnPluginStart()
		{
			Log.Info($"Plugin is loading. API Endpoint is: {Config.ApiEndpoint}");

		}





	}
}
