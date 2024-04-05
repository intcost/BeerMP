using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using BeerMP.Helpers;
using BeerMP.Networking.Managers;
using Steamworks;

namespace BeerMP.Networking
{
	// Token: 0x02000045 RID: 69
	public class BeerMPGlobals
	{
		// Token: 0x17000019 RID: 25
		// (get) Token: 0x06000157 RID: 343 RVA: 0x00007448 File Offset: 0x00005648
		public static bool IsHost
		{
			get
			{
				return NetManager.IsHost;
			}
		}

		// Token: 0x1700001A RID: 26
		// (get) Token: 0x06000158 RID: 344 RVA: 0x00007450 File Offset: 0x00005650
		public static ulong HostID
		{
			get
			{
				if (!(NetManager.HostID != default(CSteamID)))
				{
					return 0UL;
				}
				return NetManager.HostID.m_SteamID;
			}
		}

		// Token: 0x1700001B RID: 27
		// (get) Token: 0x06000159 RID: 345 RVA: 0x00007480 File Offset: 0x00005680
		public static ulong UserID
		{
			get
			{
				if (!(NetManager.UserID != default(CSteamID)))
				{
					return 0UL;
				}
				return NetManager.UserID.m_SteamID;
			}
		}

		// Token: 0x0600015A RID: 346 RVA: 0x000074AF File Offset: 0x000056AF
		public BeerMPGlobals()
		{
		}

		// Token: 0x0600015B RID: 347 RVA: 0x000074B8 File Offset: 0x000056B8
		// Note: this type is marked as 'beforefieldinit'.
		static BeerMPGlobals()
		{
		}

		// Token: 0x04000138 RID: 312
		public static ActionContainer<ulong> OnMemberJoin = new ActionContainer<ulong>();

		// Token: 0x04000139 RID: 313
		public static ActionContainer<ulong> OnMemberReady = new ActionContainer<ulong>();

		// Token: 0x0400013A RID: 314
		public static ActionContainer<ulong> OnMemberExit = new ActionContainer<ulong>();

		// Token: 0x0400013B RID: 315
		internal static bool ModLoaderInstalled = File.Exists("mysummercar_Data\\Managed\\MSCLoader.dll") && !Environment.GetCommandLineArgs().Any((string x) => x.Contains("-mscloader-disable"));

		// Token: 0x0400013C RID: 316
		internal static Assembly mscloader = (BeerMPGlobals.ModLoaderInstalled ? Assembly.LoadFile("mysummercar_Data\\Managed\\MSCLoader.dll") : null);

		// Token: 0x020000E3 RID: 227
		[CompilerGenerated]
		[Serializable]
		private sealed class <>c
		{
			// Token: 0x060004B2 RID: 1202 RVA: 0x0001BF8E File Offset: 0x0001A18E
			// Note: this type is marked as 'beforefieldinit'.
			static <>c()
			{
			}

			// Token: 0x060004B3 RID: 1203 RVA: 0x0001BF9A File Offset: 0x0001A19A
			public <>c()
			{
			}

			// Token: 0x060004B4 RID: 1204 RVA: 0x0001BFA2 File Offset: 0x0001A1A2
			internal bool <.cctor>b__12_0(string x)
			{
				return x.Contains("-mscloader-disable");
			}

			// Token: 0x0400045E RID: 1118
			public static readonly BeerMPGlobals.<>c <>9 = new BeerMPGlobals.<>c();
		}
	}
}
