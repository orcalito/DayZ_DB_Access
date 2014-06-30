/* * * * * * * * * * * * * * * * * * * * * * * * * * * * * *
 * BattleNET v1.3 - BattlEye Library and Client            *
 *                                                         *
 *  Copyright (C) 2013 by it's authors.                    *
 *  Some rights reserved. See license.txt, authors.txt.    *
 * * * * * * * * * * * * * * * * * * * * * * * * * * * * * */

using System.ComponentModel;

namespace BattleNET
{
	public enum BattlEyeConnectionResult
	{
		[Description("Connected to BattlEye !")]
		Success,

        [Description("BattlEye : Host unreachable!")]
		ConnectionFailed,

        [Description("BattlEye : Invalid login details!")]
		InvalidLogin
	}
}
