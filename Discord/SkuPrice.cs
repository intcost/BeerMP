using System;
using System.Runtime.InteropServices;

namespace Discord
{
	// Token: 0x02000025 RID: 37
	public struct SkuPrice
	{
		// Token: 0x040000B8 RID: 184
		public uint Amount;

		// Token: 0x040000B9 RID: 185
		[MarshalAs(UnmanagedType.ByValTStr, SizeConst = 16)]
		public string Currency;
	}
}
