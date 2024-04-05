using System;
using System.Runtime.InteropServices;

namespace Discord
{
	// Token: 0x02000027 RID: 39
	public struct InputMode
	{
		// Token: 0x040000BE RID: 190
		public InputModeType Type;

		// Token: 0x040000BF RID: 191
		[MarshalAs(UnmanagedType.ByValTStr, SizeConst = 256)]
		public string Shortcut;
	}
}
