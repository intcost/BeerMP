using System;
using System.Runtime.InteropServices;

namespace Discord
{
	// Token: 0x02000026 RID: 38
	public struct Sku
	{
		// Token: 0x040000BA RID: 186
		public long Id;

		// Token: 0x040000BB RID: 187
		public SkuType Type;

		// Token: 0x040000BC RID: 188
		[MarshalAs(UnmanagedType.ByValTStr, SizeConst = 256)]
		public string Name;

		// Token: 0x040000BD RID: 189
		public SkuPrice Price;
	}
}
