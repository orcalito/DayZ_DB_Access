/* * * * * * * * * * * * * * * * * * * * * * * * * * * * * *
 * BattleNET v1.3 - BattlEye Library and Client            *
 *                                                         *
 *  Copyright (C) 2013 by it's authors.                    *
 *  Some rights reserved. See license.txt, authors.txt.    *
 * * * * * * * * * * * * * * * * * * * * * * * * * * * * * */

using System.ComponentModel;

namespace BattleNET
{
	public enum BattlEyeDisconnectionType
	{
        [Description("Disconnected from BattlEye !")]
		Manual,

        [Description("Disconnected from BattlEye ! (Connection timeout)")]
		ConnectionLost,

        [Description("Disconnected from BattlEye ! (Socket Exception)")]
		SocketException,
	}
}
