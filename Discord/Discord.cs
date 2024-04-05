using System;
using System.Runtime.InteropServices;

namespace Discord
{
	// Token: 0x0200002D RID: 45
	public class Discord : IDisposable
	{
		// Token: 0x0600002F RID: 47
		[DllImport("discord_game_sdk", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
		private static extern Result DiscordCreate(uint version, ref Discord.FFICreateParams createParams, out IntPtr manager);

		// Token: 0x17000005 RID: 5
		// (get) Token: 0x06000030 RID: 48 RVA: 0x00002AA6 File Offset: 0x00000CA6
		private Discord.FFIMethods Methods
		{
			get
			{
				if (this.MethodsStructure == null)
				{
					this.MethodsStructure = Marshal.PtrToStructure(this.MethodsPtr, typeof(Discord.FFIMethods));
				}
				return (Discord.FFIMethods)this.MethodsStructure;
			}
		}

		// Token: 0x06000031 RID: 49 RVA: 0x00002AD8 File Offset: 0x00000CD8
		public Discord(long clientId, ulong flags)
		{
			Discord.FFICreateParams fficreateParams;
			fficreateParams.ClientId = clientId;
			fficreateParams.Flags = flags;
			this.Events = default(Discord.FFIEvents);
			this.EventsPtr = Marshal.AllocHGlobal(Marshal.SizeOf(this.Events));
			fficreateParams.Events = this.EventsPtr;
			this.SelfHandle = GCHandle.Alloc(this);
			fficreateParams.EventData = GCHandle.ToIntPtr(this.SelfHandle);
			this.ApplicationEvents = default(ApplicationManager.FFIEvents);
			this.ApplicationEventsPtr = Marshal.AllocHGlobal(Marshal.SizeOf(this.ApplicationEvents));
			fficreateParams.ApplicationEvents = this.ApplicationEventsPtr;
			fficreateParams.ApplicationVersion = 1U;
			this.UserEvents = default(UserManager.FFIEvents);
			this.UserEventsPtr = Marshal.AllocHGlobal(Marshal.SizeOf(this.UserEvents));
			fficreateParams.UserEvents = this.UserEventsPtr;
			fficreateParams.UserVersion = 1U;
			this.ImageEvents = default(ImageManager.FFIEvents);
			this.ImageEventsPtr = Marshal.AllocHGlobal(Marshal.SizeOf(this.ImageEvents));
			fficreateParams.ImageEvents = this.ImageEventsPtr;
			fficreateParams.ImageVersion = 1U;
			this.ActivityEvents = default(ActivityManager.FFIEvents);
			this.ActivityEventsPtr = Marshal.AllocHGlobal(Marshal.SizeOf(this.ActivityEvents));
			fficreateParams.ActivityEvents = this.ActivityEventsPtr;
			fficreateParams.ActivityVersion = 1U;
			this.RelationshipEvents = default(RelationshipManager.FFIEvents);
			this.RelationshipEventsPtr = Marshal.AllocHGlobal(Marshal.SizeOf(this.RelationshipEvents));
			fficreateParams.RelationshipEvents = this.RelationshipEventsPtr;
			fficreateParams.RelationshipVersion = 1U;
			this.LobbyEvents = default(LobbyManager.FFIEvents);
			this.LobbyEventsPtr = Marshal.AllocHGlobal(Marshal.SizeOf(this.LobbyEvents));
			fficreateParams.LobbyEvents = this.LobbyEventsPtr;
			fficreateParams.LobbyVersion = 1U;
			this.NetworkEvents = default(NetworkManager.FFIEvents);
			this.NetworkEventsPtr = Marshal.AllocHGlobal(Marshal.SizeOf(this.NetworkEvents));
			fficreateParams.NetworkEvents = this.NetworkEventsPtr;
			fficreateParams.NetworkVersion = 1U;
			this.OverlayEvents = default(OverlayManager.FFIEvents);
			this.OverlayEventsPtr = Marshal.AllocHGlobal(Marshal.SizeOf(this.OverlayEvents));
			fficreateParams.OverlayEvents = this.OverlayEventsPtr;
			fficreateParams.OverlayVersion = 1U;
			this.StorageEvents = default(StorageManager.FFIEvents);
			this.StorageEventsPtr = Marshal.AllocHGlobal(Marshal.SizeOf(this.StorageEvents));
			fficreateParams.StorageEvents = this.StorageEventsPtr;
			fficreateParams.StorageVersion = 1U;
			this.StoreEvents = default(StoreManager.FFIEvents);
			this.StoreEventsPtr = Marshal.AllocHGlobal(Marshal.SizeOf(this.StoreEvents));
			fficreateParams.StoreEvents = this.StoreEventsPtr;
			fficreateParams.StoreVersion = 1U;
			this.VoiceEvents = default(VoiceManager.FFIEvents);
			this.VoiceEventsPtr = Marshal.AllocHGlobal(Marshal.SizeOf(this.VoiceEvents));
			fficreateParams.VoiceEvents = this.VoiceEventsPtr;
			fficreateParams.VoiceVersion = 1U;
			this.AchievementEvents = default(AchievementManager.FFIEvents);
			this.AchievementEventsPtr = Marshal.AllocHGlobal(Marshal.SizeOf(this.AchievementEvents));
			fficreateParams.AchievementEvents = this.AchievementEventsPtr;
			fficreateParams.AchievementVersion = 1U;
			this.InitEvents(this.EventsPtr, ref this.Events);
			Result result = Discord.DiscordCreate(2U, ref fficreateParams, out this.MethodsPtr);
			if (result != Result.Ok)
			{
				this.Dispose();
				throw new ResultException(result);
			}
		}

		// Token: 0x06000032 RID: 50 RVA: 0x00002E4E File Offset: 0x0000104E
		private void InitEvents(IntPtr eventsPtr, ref Discord.FFIEvents events)
		{
			Marshal.StructureToPtr(events, eventsPtr, false);
		}

		// Token: 0x06000033 RID: 51 RVA: 0x00002E64 File Offset: 0x00001064
		public void Dispose()
		{
			if (this.MethodsPtr != IntPtr.Zero)
			{
				this.Methods.Destroy(this.MethodsPtr);
			}
			this.SelfHandle.Free();
			Marshal.FreeHGlobal(this.EventsPtr);
			Marshal.FreeHGlobal(this.ApplicationEventsPtr);
			Marshal.FreeHGlobal(this.UserEventsPtr);
			Marshal.FreeHGlobal(this.ImageEventsPtr);
			Marshal.FreeHGlobal(this.ActivityEventsPtr);
			Marshal.FreeHGlobal(this.RelationshipEventsPtr);
			Marshal.FreeHGlobal(this.LobbyEventsPtr);
			Marshal.FreeHGlobal(this.NetworkEventsPtr);
			Marshal.FreeHGlobal(this.OverlayEventsPtr);
			Marshal.FreeHGlobal(this.StorageEventsPtr);
			Marshal.FreeHGlobal(this.StoreEventsPtr);
			Marshal.FreeHGlobal(this.VoiceEventsPtr);
			Marshal.FreeHGlobal(this.AchievementEventsPtr);
			if (this.setLogHook != null)
			{
				this.setLogHook.Value.Free();
			}
		}

		// Token: 0x06000034 RID: 52 RVA: 0x00002F54 File Offset: 0x00001154
		public void RunCallbacks()
		{
			Result result = this.Methods.RunCallbacks(this.MethodsPtr);
			if (result != Result.Ok)
			{
				throw new ResultException(result);
			}
		}

		// Token: 0x06000035 RID: 53 RVA: 0x00002F84 File Offset: 0x00001184
		[MonoPInvokeCallback]
		private static void SetLogHookCallbackImpl(IntPtr ptr, LogLevel level, string message)
		{
			((Discord.SetLogHookHandler)GCHandle.FromIntPtr(ptr).Target)(level, message);
		}

		// Token: 0x06000036 RID: 54 RVA: 0x00002FAC File Offset: 0x000011AC
		public void SetLogHook(LogLevel minLevel, Discord.SetLogHookHandler callback)
		{
			if (this.setLogHook != null)
			{
				this.setLogHook.Value.Free();
			}
			this.setLogHook = new GCHandle?(GCHandle.Alloc(callback));
			this.Methods.SetLogHook(this.MethodsPtr, minLevel, GCHandle.ToIntPtr(this.setLogHook.Value), new Discord.FFIMethods.SetLogHookCallback(Discord.SetLogHookCallbackImpl));
		}

		// Token: 0x06000037 RID: 55 RVA: 0x0000301D File Offset: 0x0000121D
		public ApplicationManager GetApplicationManager()
		{
			if (this.ApplicationManagerInstance == null)
			{
				this.ApplicationManagerInstance = new ApplicationManager(this.Methods.GetApplicationManager(this.MethodsPtr), this.ApplicationEventsPtr, ref this.ApplicationEvents);
			}
			return this.ApplicationManagerInstance;
		}

		// Token: 0x06000038 RID: 56 RVA: 0x0000305A File Offset: 0x0000125A
		public UserManager GetUserManager()
		{
			if (this.UserManagerInstance == null)
			{
				this.UserManagerInstance = new UserManager(this.Methods.GetUserManager(this.MethodsPtr), this.UserEventsPtr, ref this.UserEvents);
			}
			return this.UserManagerInstance;
		}

		// Token: 0x06000039 RID: 57 RVA: 0x00003097 File Offset: 0x00001297
		public ImageManager GetImageManager()
		{
			if (this.ImageManagerInstance == null)
			{
				this.ImageManagerInstance = new ImageManager(this.Methods.GetImageManager(this.MethodsPtr), this.ImageEventsPtr, ref this.ImageEvents);
			}
			return this.ImageManagerInstance;
		}

		// Token: 0x0600003A RID: 58 RVA: 0x000030D4 File Offset: 0x000012D4
		public ActivityManager GetActivityManager()
		{
			if (this.ActivityManagerInstance == null)
			{
				this.ActivityManagerInstance = new ActivityManager(this.Methods.GetActivityManager(this.MethodsPtr), this.ActivityEventsPtr, ref this.ActivityEvents);
			}
			return this.ActivityManagerInstance;
		}

		// Token: 0x0600003B RID: 59 RVA: 0x00003111 File Offset: 0x00001311
		public RelationshipManager GetRelationshipManager()
		{
			if (this.RelationshipManagerInstance == null)
			{
				this.RelationshipManagerInstance = new RelationshipManager(this.Methods.GetRelationshipManager(this.MethodsPtr), this.RelationshipEventsPtr, ref this.RelationshipEvents);
			}
			return this.RelationshipManagerInstance;
		}

		// Token: 0x0600003C RID: 60 RVA: 0x0000314E File Offset: 0x0000134E
		public LobbyManager GetLobbyManager()
		{
			if (this.LobbyManagerInstance == null)
			{
				this.LobbyManagerInstance = new LobbyManager(this.Methods.GetLobbyManager(this.MethodsPtr), this.LobbyEventsPtr, ref this.LobbyEvents);
			}
			return this.LobbyManagerInstance;
		}

		// Token: 0x0600003D RID: 61 RVA: 0x0000318B File Offset: 0x0000138B
		public NetworkManager GetNetworkManager()
		{
			if (this.NetworkManagerInstance == null)
			{
				this.NetworkManagerInstance = new NetworkManager(this.Methods.GetNetworkManager(this.MethodsPtr), this.NetworkEventsPtr, ref this.NetworkEvents);
			}
			return this.NetworkManagerInstance;
		}

		// Token: 0x0600003E RID: 62 RVA: 0x000031C8 File Offset: 0x000013C8
		public OverlayManager GetOverlayManager()
		{
			if (this.OverlayManagerInstance == null)
			{
				this.OverlayManagerInstance = new OverlayManager(this.Methods.GetOverlayManager(this.MethodsPtr), this.OverlayEventsPtr, ref this.OverlayEvents);
			}
			return this.OverlayManagerInstance;
		}

		// Token: 0x0600003F RID: 63 RVA: 0x00003205 File Offset: 0x00001405
		public StorageManager GetStorageManager()
		{
			if (this.StorageManagerInstance == null)
			{
				this.StorageManagerInstance = new StorageManager(this.Methods.GetStorageManager(this.MethodsPtr), this.StorageEventsPtr, ref this.StorageEvents);
			}
			return this.StorageManagerInstance;
		}

		// Token: 0x06000040 RID: 64 RVA: 0x00003242 File Offset: 0x00001442
		public StoreManager GetStoreManager()
		{
			if (this.StoreManagerInstance == null)
			{
				this.StoreManagerInstance = new StoreManager(this.Methods.GetStoreManager(this.MethodsPtr), this.StoreEventsPtr, ref this.StoreEvents);
			}
			return this.StoreManagerInstance;
		}

		// Token: 0x06000041 RID: 65 RVA: 0x0000327F File Offset: 0x0000147F
		public VoiceManager GetVoiceManager()
		{
			if (this.VoiceManagerInstance == null)
			{
				this.VoiceManagerInstance = new VoiceManager(this.Methods.GetVoiceManager(this.MethodsPtr), this.VoiceEventsPtr, ref this.VoiceEvents);
			}
			return this.VoiceManagerInstance;
		}

		// Token: 0x06000042 RID: 66 RVA: 0x000032BC File Offset: 0x000014BC
		public AchievementManager GetAchievementManager()
		{
			if (this.AchievementManagerInstance == null)
			{
				this.AchievementManagerInstance = new AchievementManager(this.Methods.GetAchievementManager(this.MethodsPtr), this.AchievementEventsPtr, ref this.AchievementEvents);
			}
			return this.AchievementManagerInstance;
		}

		// Token: 0x040000CB RID: 203
		private GCHandle SelfHandle;

		// Token: 0x040000CC RID: 204
		private IntPtr EventsPtr;

		// Token: 0x040000CD RID: 205
		private Discord.FFIEvents Events;

		// Token: 0x040000CE RID: 206
		private IntPtr ApplicationEventsPtr;

		// Token: 0x040000CF RID: 207
		private ApplicationManager.FFIEvents ApplicationEvents;

		// Token: 0x040000D0 RID: 208
		internal ApplicationManager ApplicationManagerInstance;

		// Token: 0x040000D1 RID: 209
		private IntPtr UserEventsPtr;

		// Token: 0x040000D2 RID: 210
		private UserManager.FFIEvents UserEvents;

		// Token: 0x040000D3 RID: 211
		internal UserManager UserManagerInstance;

		// Token: 0x040000D4 RID: 212
		private IntPtr ImageEventsPtr;

		// Token: 0x040000D5 RID: 213
		private ImageManager.FFIEvents ImageEvents;

		// Token: 0x040000D6 RID: 214
		internal ImageManager ImageManagerInstance;

		// Token: 0x040000D7 RID: 215
		private IntPtr ActivityEventsPtr;

		// Token: 0x040000D8 RID: 216
		private ActivityManager.FFIEvents ActivityEvents;

		// Token: 0x040000D9 RID: 217
		internal ActivityManager ActivityManagerInstance;

		// Token: 0x040000DA RID: 218
		private IntPtr RelationshipEventsPtr;

		// Token: 0x040000DB RID: 219
		private RelationshipManager.FFIEvents RelationshipEvents;

		// Token: 0x040000DC RID: 220
		internal RelationshipManager RelationshipManagerInstance;

		// Token: 0x040000DD RID: 221
		private IntPtr LobbyEventsPtr;

		// Token: 0x040000DE RID: 222
		private LobbyManager.FFIEvents LobbyEvents;

		// Token: 0x040000DF RID: 223
		internal LobbyManager LobbyManagerInstance;

		// Token: 0x040000E0 RID: 224
		private IntPtr NetworkEventsPtr;

		// Token: 0x040000E1 RID: 225
		private NetworkManager.FFIEvents NetworkEvents;

		// Token: 0x040000E2 RID: 226
		internal NetworkManager NetworkManagerInstance;

		// Token: 0x040000E3 RID: 227
		private IntPtr OverlayEventsPtr;

		// Token: 0x040000E4 RID: 228
		private OverlayManager.FFIEvents OverlayEvents;

		// Token: 0x040000E5 RID: 229
		internal OverlayManager OverlayManagerInstance;

		// Token: 0x040000E6 RID: 230
		private IntPtr StorageEventsPtr;

		// Token: 0x040000E7 RID: 231
		private StorageManager.FFIEvents StorageEvents;

		// Token: 0x040000E8 RID: 232
		internal StorageManager StorageManagerInstance;

		// Token: 0x040000E9 RID: 233
		private IntPtr StoreEventsPtr;

		// Token: 0x040000EA RID: 234
		private StoreManager.FFIEvents StoreEvents;

		// Token: 0x040000EB RID: 235
		internal StoreManager StoreManagerInstance;

		// Token: 0x040000EC RID: 236
		private IntPtr VoiceEventsPtr;

		// Token: 0x040000ED RID: 237
		private VoiceManager.FFIEvents VoiceEvents;

		// Token: 0x040000EE RID: 238
		internal VoiceManager VoiceManagerInstance;

		// Token: 0x040000EF RID: 239
		private IntPtr AchievementEventsPtr;

		// Token: 0x040000F0 RID: 240
		private AchievementManager.FFIEvents AchievementEvents;

		// Token: 0x040000F1 RID: 241
		internal AchievementManager AchievementManagerInstance;

		// Token: 0x040000F2 RID: 242
		private IntPtr MethodsPtr;

		// Token: 0x040000F3 RID: 243
		private object MethodsStructure;

		// Token: 0x040000F4 RID: 244
		private GCHandle? setLogHook;

		// Token: 0x02000094 RID: 148
		internal struct FFIEvents
		{
		}

		// Token: 0x02000095 RID: 149
		internal struct FFIMethods
		{
			// Token: 0x040003B0 RID: 944
			internal Discord.FFIMethods.DestroyHandler Destroy;

			// Token: 0x040003B1 RID: 945
			internal Discord.FFIMethods.RunCallbacksMethod RunCallbacks;

			// Token: 0x040003B2 RID: 946
			internal Discord.FFIMethods.SetLogHookMethod SetLogHook;

			// Token: 0x040003B3 RID: 947
			internal Discord.FFIMethods.GetApplicationManagerMethod GetApplicationManager;

			// Token: 0x040003B4 RID: 948
			internal Discord.FFIMethods.GetUserManagerMethod GetUserManager;

			// Token: 0x040003B5 RID: 949
			internal Discord.FFIMethods.GetImageManagerMethod GetImageManager;

			// Token: 0x040003B6 RID: 950
			internal Discord.FFIMethods.GetActivityManagerMethod GetActivityManager;

			// Token: 0x040003B7 RID: 951
			internal Discord.FFIMethods.GetRelationshipManagerMethod GetRelationshipManager;

			// Token: 0x040003B8 RID: 952
			internal Discord.FFIMethods.GetLobbyManagerMethod GetLobbyManager;

			// Token: 0x040003B9 RID: 953
			internal Discord.FFIMethods.GetNetworkManagerMethod GetNetworkManager;

			// Token: 0x040003BA RID: 954
			internal Discord.FFIMethods.GetOverlayManagerMethod GetOverlayManager;

			// Token: 0x040003BB RID: 955
			internal Discord.FFIMethods.GetStorageManagerMethod GetStorageManager;

			// Token: 0x040003BC RID: 956
			internal Discord.FFIMethods.GetStoreManagerMethod GetStoreManager;

			// Token: 0x040003BD RID: 957
			internal Discord.FFIMethods.GetVoiceManagerMethod GetVoiceManager;

			// Token: 0x040003BE RID: 958
			internal Discord.FFIMethods.GetAchievementManagerMethod GetAchievementManager;

			// Token: 0x02000177 RID: 375
			// (Invoke) Token: 0x060006C1 RID: 1729
			[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
			internal delegate void DestroyHandler(IntPtr MethodsPtr);

			// Token: 0x02000178 RID: 376
			// (Invoke) Token: 0x060006C5 RID: 1733
			[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
			internal delegate Result RunCallbacksMethod(IntPtr methodsPtr);

			// Token: 0x02000179 RID: 377
			// (Invoke) Token: 0x060006C9 RID: 1737
			[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
			internal delegate void SetLogHookCallback(IntPtr ptr, LogLevel level, [MarshalAs(UnmanagedType.LPStr)] string message);

			// Token: 0x0200017A RID: 378
			// (Invoke) Token: 0x060006CD RID: 1741
			[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
			internal delegate void SetLogHookMethod(IntPtr methodsPtr, LogLevel minLevel, IntPtr callbackData, Discord.FFIMethods.SetLogHookCallback callback);

			// Token: 0x0200017B RID: 379
			// (Invoke) Token: 0x060006D1 RID: 1745
			[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
			internal delegate IntPtr GetApplicationManagerMethod(IntPtr discordPtr);

			// Token: 0x0200017C RID: 380
			// (Invoke) Token: 0x060006D5 RID: 1749
			[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
			internal delegate IntPtr GetUserManagerMethod(IntPtr discordPtr);

			// Token: 0x0200017D RID: 381
			// (Invoke) Token: 0x060006D9 RID: 1753
			[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
			internal delegate IntPtr GetImageManagerMethod(IntPtr discordPtr);

			// Token: 0x0200017E RID: 382
			// (Invoke) Token: 0x060006DD RID: 1757
			[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
			internal delegate IntPtr GetActivityManagerMethod(IntPtr discordPtr);

			// Token: 0x0200017F RID: 383
			// (Invoke) Token: 0x060006E1 RID: 1761
			[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
			internal delegate IntPtr GetRelationshipManagerMethod(IntPtr discordPtr);

			// Token: 0x02000180 RID: 384
			// (Invoke) Token: 0x060006E5 RID: 1765
			[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
			internal delegate IntPtr GetLobbyManagerMethod(IntPtr discordPtr);

			// Token: 0x02000181 RID: 385
			// (Invoke) Token: 0x060006E9 RID: 1769
			[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
			internal delegate IntPtr GetNetworkManagerMethod(IntPtr discordPtr);

			// Token: 0x02000182 RID: 386
			// (Invoke) Token: 0x060006ED RID: 1773
			[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
			internal delegate IntPtr GetOverlayManagerMethod(IntPtr discordPtr);

			// Token: 0x02000183 RID: 387
			// (Invoke) Token: 0x060006F1 RID: 1777
			[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
			internal delegate IntPtr GetStorageManagerMethod(IntPtr discordPtr);

			// Token: 0x02000184 RID: 388
			// (Invoke) Token: 0x060006F5 RID: 1781
			[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
			internal delegate IntPtr GetStoreManagerMethod(IntPtr discordPtr);

			// Token: 0x02000185 RID: 389
			// (Invoke) Token: 0x060006F9 RID: 1785
			[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
			internal delegate IntPtr GetVoiceManagerMethod(IntPtr discordPtr);

			// Token: 0x02000186 RID: 390
			// (Invoke) Token: 0x060006FD RID: 1789
			[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
			internal delegate IntPtr GetAchievementManagerMethod(IntPtr discordPtr);
		}

		// Token: 0x02000096 RID: 150
		internal struct FFICreateParams
		{
			// Token: 0x040003BF RID: 959
			internal long ClientId;

			// Token: 0x040003C0 RID: 960
			internal ulong Flags;

			// Token: 0x040003C1 RID: 961
			internal IntPtr Events;

			// Token: 0x040003C2 RID: 962
			internal IntPtr EventData;

			// Token: 0x040003C3 RID: 963
			internal IntPtr ApplicationEvents;

			// Token: 0x040003C4 RID: 964
			internal uint ApplicationVersion;

			// Token: 0x040003C5 RID: 965
			internal IntPtr UserEvents;

			// Token: 0x040003C6 RID: 966
			internal uint UserVersion;

			// Token: 0x040003C7 RID: 967
			internal IntPtr ImageEvents;

			// Token: 0x040003C8 RID: 968
			internal uint ImageVersion;

			// Token: 0x040003C9 RID: 969
			internal IntPtr ActivityEvents;

			// Token: 0x040003CA RID: 970
			internal uint ActivityVersion;

			// Token: 0x040003CB RID: 971
			internal IntPtr RelationshipEvents;

			// Token: 0x040003CC RID: 972
			internal uint RelationshipVersion;

			// Token: 0x040003CD RID: 973
			internal IntPtr LobbyEvents;

			// Token: 0x040003CE RID: 974
			internal uint LobbyVersion;

			// Token: 0x040003CF RID: 975
			internal IntPtr NetworkEvents;

			// Token: 0x040003D0 RID: 976
			internal uint NetworkVersion;

			// Token: 0x040003D1 RID: 977
			internal IntPtr OverlayEvents;

			// Token: 0x040003D2 RID: 978
			internal uint OverlayVersion;

			// Token: 0x040003D3 RID: 979
			internal IntPtr StorageEvents;

			// Token: 0x040003D4 RID: 980
			internal uint StorageVersion;

			// Token: 0x040003D5 RID: 981
			internal IntPtr StoreEvents;

			// Token: 0x040003D6 RID: 982
			internal uint StoreVersion;

			// Token: 0x040003D7 RID: 983
			internal IntPtr VoiceEvents;

			// Token: 0x040003D8 RID: 984
			internal uint VoiceVersion;

			// Token: 0x040003D9 RID: 985
			internal IntPtr AchievementEvents;

			// Token: 0x040003DA RID: 986
			internal uint AchievementVersion;
		}

		// Token: 0x02000097 RID: 151
		// (Invoke) Token: 0x060003DB RID: 987
		public delegate void SetLogHookHandler(LogLevel level, string message);
	}
}
