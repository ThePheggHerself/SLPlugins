using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DynamicTags
{
	public class TagData
	{
		public string UserID;
		public string Prefix;
		public string Suffix;
		public string Colour;
		public bool HasReservedSlot;
		public ulong Permissions;

		public string Group;

		public string FullTag
		{
			get => $"{Prefix} - {Suffix}";
		}
	}
	public class PlayerDetails
	{
		public string UserId;
		public string UserName;
		public string Address;

		public string ServerAddress;
		public string ServerPort;
	}
	internal class PlayerBanDetails
	{
		public string playerName { get; set; }
		public string playerID { get; set; }
		public string adminName { get; set; }
		public string adminID { get; set; }
		public string duration { get; set; }
		public string reason { get; set; }
	}
}
