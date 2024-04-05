using System;

namespace BeerMP
{
	// Token: 0x0200003B RID: 59
	public static class DateTimeExtension
	{
		// Token: 0x06000125 RID: 293 RVA: 0x000064B8 File Offset: 0x000046B8
		public static long ToUnixTimestamp(this DateTime value)
		{
			return (long)value.ToUniversalTime().Subtract(new DateTime(1970, 1, 1)).TotalSeconds;
		}
	}
}
