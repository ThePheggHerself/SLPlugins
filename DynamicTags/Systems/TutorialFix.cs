using PlayerRoles;
using PlayerRoles.FirstPersonControl;
using PluginAPI.Core;
using PluginAPI.Core.Attributes;
using PluginAPI.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DynamicTags.Systems
{
	public class TutorialFix
	{
		[PluginEvent(ServerEventType.PlayerChangeRole)]
		public void OnPlayerRoleChange(Player player, PlayerRoleBase oldRoleBase, RoleTypeId newRole, RoleChangeReason reason)
		{
			if(newRole == RoleTypeId.Tutorial && reason == RoleChangeReason.RemoteAdmin)
			{
				player.Position = new UnityEngine.Vector3(39, 1014.5f, -32);
			}
		}
	}
}
