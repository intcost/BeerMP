using System;
using System.Runtime.InteropServices;

namespace Discord
{
	// Token: 0x0200001F RID: 31
	public struct Activity
	{
		// Token: 0x0400009D RID: 157
		public ActivityType Type;

		// Token: 0x0400009E RID: 158
		public long ApplicationId;

		// Token: 0x0400009F RID: 159
		[MarshalAs(UnmanagedType.ByValTStr, SizeConst = 128)]
		public string Name;

		// Token: 0x040000A0 RID: 160
		[MarshalAs(UnmanagedType.ByValTStr, SizeConst = 128)]
		public string State;

		// Token: 0x040000A1 RID: 161
		[MarshalAs(UnmanagedType.ByValTStr, SizeConst = 128)]
		public string Details;

		// Token: 0x040000A2 RID: 162
		public ActivityTimestamps Timestamps;

		// Token: 0x040000A3 RID: 163
		public ActivityAssets Assets;

		// Token: 0x040000A4 RID: 164
		public ActivityParty Party;

		// Token: 0x040000A5 RID: 165
		public ActivitySecrets Secrets;

		// Token: 0x040000A6 RID: 166
		public bool Instance;
	}
}
