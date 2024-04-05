using System;
using System.Runtime.InteropServices;

namespace Discord
{
	// Token: 0x02000028 RID: 40
	public struct UserAchievement
	{
		// Token: 0x040000C0 RID: 192
		public long UserId;

		// Token: 0x040000C1 RID: 193
		public long AchievementId;

		// Token: 0x040000C2 RID: 194
		public byte PercentComplete;

		// Token: 0x040000C3 RID: 195
		[MarshalAs(UnmanagedType.ByValTStr, SizeConst = 64)]
		public string UnlockedAt;
	}
}
