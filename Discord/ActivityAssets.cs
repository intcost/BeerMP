using System;
using System.Runtime.InteropServices;

namespace Discord
{
	// Token: 0x0200001B RID: 27
	public struct ActivityAssets
	{
		// Token: 0x04000092 RID: 146
		[MarshalAs(UnmanagedType.ByValTStr, SizeConst = 128)]
		public string LargeImage;

		// Token: 0x04000093 RID: 147
		[MarshalAs(UnmanagedType.ByValTStr, SizeConst = 128)]
		public string LargeText;

		// Token: 0x04000094 RID: 148
		[MarshalAs(UnmanagedType.ByValTStr, SizeConst = 128)]
		public string SmallImage;

		// Token: 0x04000095 RID: 149
		[MarshalAs(UnmanagedType.ByValTStr, SizeConst = 128)]
		public string SmallText;
	}
}
