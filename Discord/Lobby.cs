using System;
using System.Runtime.InteropServices;

namespace Discord
{
	// Token: 0x02000022 RID: 34
	public struct Lobby
	{
		// Token: 0x040000AC RID: 172
		public long Id;

		// Token: 0x040000AD RID: 173
		public LobbyType Type;

		// Token: 0x040000AE RID: 174
		public long OwnerId;

		// Token: 0x040000AF RID: 175
		[MarshalAs(UnmanagedType.ByValTStr, SizeConst = 128)]
		public string Secret;

		// Token: 0x040000B0 RID: 176
		public uint Capacity;

		// Token: 0x040000B1 RID: 177
		public bool Locked;
	}
}
