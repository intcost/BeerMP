using System;
using UnityEngine;

namespace BeerMP
{
	// Token: 0x0200003A RID: 58
	internal class BeerMPEntry
	{
		// Token: 0x06000123 RID: 291 RVA: 0x0000648B File Offset: 0x0000468B
		internal static void Start()
		{
			if (BeerMPEntry.system == null)
			{
				BeerMPEntry.system = new GameObject("BeerMP").AddComponent<BeerMP>();
			}
		}

		// Token: 0x06000124 RID: 292 RVA: 0x000064AE File Offset: 0x000046AE
		public BeerMPEntry()
		{
		}

		// Token: 0x0400011D RID: 285
		internal static BeerMP system;
	}
}
