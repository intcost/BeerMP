using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.Resources;
using System.Runtime.CompilerServices;

namespace BeerMP.Properties
{
	// Token: 0x02000044 RID: 68
	[GeneratedCode("System.Resources.Tools.StronglyTypedResourceBuilder", "17.0.0.0")]
	[DebuggerNonUserCode]
	[CompilerGenerated]
	internal class Resources
	{
		// Token: 0x06000151 RID: 337 RVA: 0x000073D4 File Offset: 0x000055D4
		internal Resources()
		{
		}

		// Token: 0x17000015 RID: 21
		// (get) Token: 0x06000152 RID: 338 RVA: 0x000073DC File Offset: 0x000055DC
		[EditorBrowsable(EditorBrowsableState.Advanced)]
		internal static ResourceManager ResourceManager
		{
			get
			{
				if (Resources.resourceMan == null)
				{
					Resources.resourceMan = new ResourceManager("BeerMP.Properties.Resources", typeof(Resources).Assembly);
				}
				return Resources.resourceMan;
			}
		}

		// Token: 0x17000016 RID: 22
		// (get) Token: 0x06000153 RID: 339 RVA: 0x00007408 File Offset: 0x00005608
		// (set) Token: 0x06000154 RID: 340 RVA: 0x0000740F File Offset: 0x0000560F
		[EditorBrowsable(EditorBrowsableState.Advanced)]
		internal static CultureInfo Culture
		{
			get
			{
				return Resources.resourceCulture;
			}
			set
			{
				Resources.resourceCulture = value;
			}
		}

		// Token: 0x17000017 RID: 23
		// (get) Token: 0x06000155 RID: 341 RVA: 0x00007417 File Offset: 0x00005617
		internal static string clientID
		{
			get
			{
				return Resources.ResourceManager.GetString("clientID", Resources.resourceCulture);
			}
		}

		// Token: 0x17000018 RID: 24
		// (get) Token: 0x06000156 RID: 342 RVA: 0x0000742D File Offset: 0x0000562D
		internal static byte[] clothes
		{
			get
			{
				return (byte[])Resources.ResourceManager.GetObject("clothes", Resources.resourceCulture);
			}
		}

		// Token: 0x04000136 RID: 310
		private static ResourceManager resourceMan;

		// Token: 0x04000137 RID: 311
		private static CultureInfo resourceCulture;
	}
}
