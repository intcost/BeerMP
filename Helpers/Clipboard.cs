using System;
using System.Reflection;
using UnityEngine;

namespace BeerMP.Helpers
{
	// Token: 0x0200007F RID: 127
	internal class Clipboard
	{
		// Token: 0x1700002F RID: 47
		// (get) Token: 0x06000397 RID: 919 RVA: 0x0001B01B File Offset: 0x0001921B
		// (set) Token: 0x06000398 RID: 920 RVA: 0x0001B02E File Offset: 0x0001922E
		public static string text
		{
			get
			{
				return Clipboard.cp.GetValue(null, null).ToString();
			}
			set
			{
				Clipboard.cp.SetValue(null, value, null);
			}
		}

		// Token: 0x06000399 RID: 921 RVA: 0x0001B03D File Offset: 0x0001923D
		public Clipboard()
		{
		}

		// Token: 0x0600039A RID: 922 RVA: 0x0001B045 File Offset: 0x00019245
		// Note: this type is marked as 'beforefieldinit'.
		static Clipboard()
		{
		}

		// Token: 0x04000387 RID: 903
		private static PropertyInfo cp = typeof(GUIUtility).GetProperty("systemCopyBuffer", BindingFlags.Static | BindingFlags.NonPublic);
	}
}
