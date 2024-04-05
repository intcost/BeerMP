using System;
using System.Runtime.InteropServices;

namespace Discord
{
	// Token: 0x02000016 RID: 22
	public struct User
	{
		// Token: 0x04000083 RID: 131
		public long Id;

		// Token: 0x04000084 RID: 132
		[MarshalAs(UnmanagedType.ByValTStr, SizeConst = 256)]
		public string Username;

		// Token: 0x04000085 RID: 133
		[MarshalAs(UnmanagedType.ByValTStr, SizeConst = 8)]
		public string Discriminator;

		// Token: 0x04000086 RID: 134
		[MarshalAs(UnmanagedType.ByValTStr, SizeConst = 128)]
		public string Avatar;

		// Token: 0x04000087 RID: 135
		public bool Bot;
	}
}
