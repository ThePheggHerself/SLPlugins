using CommandSystem;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pheggmod.Commands
{
	public class TestCommand : ICommand, IUsageProvider
	{
		public string Command => "nevergonna";

		public string[] Aliases => null;

		public string Description => "Test command. Try it :)";

		public string[] Usage { get; } = { "give", "you", "up" };

		public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
		{
			response = $"Never gonna give you up,\nNever gonna let you down.\nNever gonna run around,\nAnd desert you.\nNever gonna make you cry,\nNever gonna say goodbye.\nNever gonna tell a lie,\nAnd hurt you.";
			return true;
		}
	}
}
