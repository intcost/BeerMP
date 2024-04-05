using System;
using System.Runtime.InteropServices;

namespace Discord
{
	// Token: 0x02000023 RID: 35
	public struct FileStat
	{
		// Token: 0x040000B2 RID: 178
		[MarshalAs(UnmanagedType.ByValTStr, SizeConst = 260)]
		public string Filename;

		// Token: 0x040000B3 RID: 179
		public ulong Size;

		// Token: 0x040000B4 RID: 180
		public ulong LastModified;
	}
}
