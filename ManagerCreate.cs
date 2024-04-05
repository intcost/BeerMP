using System;
using System.Runtime.CompilerServices;

namespace BeerMP
{
	// Token: 0x02000040 RID: 64
	internal class ManagerCreate : Attribute
	{
		// Token: 0x17000014 RID: 20
		// (get) Token: 0x06000145 RID: 325 RVA: 0x00007196 File Offset: 0x00005396
		// (set) Token: 0x06000146 RID: 326 RVA: 0x0000719E File Offset: 0x0000539E
		public int priority
		{
			[CompilerGenerated]
			get
			{
				return this.<priority>k__BackingField;
			}
			[CompilerGenerated]
			private set
			{
				this.<priority>k__BackingField = value;
			}
		}

		// Token: 0x06000147 RID: 327 RVA: 0x000071A7 File Offset: 0x000053A7
		public ManagerCreate(int priority = 10)
		{
			this.priority = priority;
		}

		// Token: 0x04000131 RID: 305
		[CompilerGenerated]
		private int <priority>k__BackingField;
	}
}
