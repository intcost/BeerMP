using System;
using System.Runtime.InteropServices;

namespace Discord
{
	// Token: 0x0200001E RID: 30
	public struct ActivitySecrets
	{
		// Token: 0x0400009A RID: 154
		[MarshalAs(UnmanagedType.ByValTStr, SizeConst = 128)]
		public string Match;

		// Token: 0x0400009B RID: 155
		[MarshalAs(UnmanagedType.ByValTStr, SizeConst = 128)]
		public string Join;

		// Token: 0x0400009C RID: 156
		[MarshalAs(UnmanagedType.ByValTStr, SizeConst = 128)]
		public string Spectate;
	}
}
