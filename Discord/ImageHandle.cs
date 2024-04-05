using System;

namespace Discord
{
	// Token: 0x02000018 RID: 24
	public struct ImageHandle
	{
		// Token: 0x0600001D RID: 29 RVA: 0x00002691 File Offset: 0x00000891
		public static ImageHandle User(long id)
		{
			return ImageHandle.User(id, 128U);
		}

		// Token: 0x0600001E RID: 30 RVA: 0x000026A0 File Offset: 0x000008A0
		public static ImageHandle User(long id, uint size)
		{
			return new ImageHandle
			{
				Type = ImageType.User,
				Id = id,
				Size = size
			};
		}

		// Token: 0x0400008B RID: 139
		public ImageType Type;

		// Token: 0x0400008C RID: 140
		public long Id;

		// Token: 0x0400008D RID: 141
		public uint Size;
	}
}
