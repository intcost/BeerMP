using System;

namespace Discord
{
	// Token: 0x0200002C RID: 44
	public class ResultException : Exception
	{
		// Token: 0x0600002E RID: 46 RVA: 0x00002A91 File Offset: 0x00000C91
		public ResultException(Result result)
			: base(result.ToString())
		{
		}

		// Token: 0x040000CA RID: 202
		public readonly Result Result;
	}
}
