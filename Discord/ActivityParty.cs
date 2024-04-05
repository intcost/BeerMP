using System;
using System.Runtime.InteropServices;

namespace Discord
{
	// Token: 0x0200001D RID: 29
	public struct ActivityParty
	{
		// Token: 0x04000098 RID: 152
		[MarshalAs(UnmanagedType.ByValTStr, SizeConst = 128)]
		public string Id;

		// Token: 0x04000099 RID: 153
		public PartySize Size;
	}
}
