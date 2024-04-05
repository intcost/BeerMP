using System;
using System.Runtime.InteropServices;

namespace Discord
{
	// Token: 0x02000017 RID: 23
	public struct OAuth2Token
	{
		// Token: 0x04000088 RID: 136
		[MarshalAs(UnmanagedType.ByValTStr, SizeConst = 128)]
		public string AccessToken;

		// Token: 0x04000089 RID: 137
		[MarshalAs(UnmanagedType.ByValTStr, SizeConst = 1024)]
		public string Scopes;

		// Token: 0x0400008A RID: 138
		public long Expires;
	}
}
