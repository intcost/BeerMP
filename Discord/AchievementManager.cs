using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Threading;

namespace Discord
{
	// Token: 0x02000039 RID: 57
	public class AchievementManager
	{
		// Token: 0x17000010 RID: 16
		// (get) Token: 0x06000116 RID: 278 RVA: 0x000061B6 File Offset: 0x000043B6
		private AchievementManager.FFIMethods Methods
		{
			get
			{
				if (this.MethodsStructure == null)
				{
					this.MethodsStructure = Marshal.PtrToStructure(this.MethodsPtr, typeof(AchievementManager.FFIMethods));
				}
				return (AchievementManager.FFIMethods)this.MethodsStructure;
			}
		}

		// Token: 0x14000016 RID: 22
		// (add) Token: 0x06000117 RID: 279 RVA: 0x000061E8 File Offset: 0x000043E8
		// (remove) Token: 0x06000118 RID: 280 RVA: 0x00006220 File Offset: 0x00004420
		public event AchievementManager.UserAchievementUpdateHandler OnUserAchievementUpdate
		{
			[CompilerGenerated]
			add
			{
				AchievementManager.UserAchievementUpdateHandler userAchievementUpdateHandler = this.OnUserAchievementUpdate;
				AchievementManager.UserAchievementUpdateHandler userAchievementUpdateHandler2;
				do
				{
					userAchievementUpdateHandler2 = userAchievementUpdateHandler;
					AchievementManager.UserAchievementUpdateHandler userAchievementUpdateHandler3 = (AchievementManager.UserAchievementUpdateHandler)Delegate.Combine(userAchievementUpdateHandler2, value);
					userAchievementUpdateHandler = Interlocked.CompareExchange<AchievementManager.UserAchievementUpdateHandler>(ref this.OnUserAchievementUpdate, userAchievementUpdateHandler3, userAchievementUpdateHandler2);
				}
				while (userAchievementUpdateHandler != userAchievementUpdateHandler2);
			}
			[CompilerGenerated]
			remove
			{
				AchievementManager.UserAchievementUpdateHandler userAchievementUpdateHandler = this.OnUserAchievementUpdate;
				AchievementManager.UserAchievementUpdateHandler userAchievementUpdateHandler2;
				do
				{
					userAchievementUpdateHandler2 = userAchievementUpdateHandler;
					AchievementManager.UserAchievementUpdateHandler userAchievementUpdateHandler3 = (AchievementManager.UserAchievementUpdateHandler)Delegate.Remove(userAchievementUpdateHandler2, value);
					userAchievementUpdateHandler = Interlocked.CompareExchange<AchievementManager.UserAchievementUpdateHandler>(ref this.OnUserAchievementUpdate, userAchievementUpdateHandler3, userAchievementUpdateHandler2);
				}
				while (userAchievementUpdateHandler != userAchievementUpdateHandler2);
			}
		}

		// Token: 0x06000119 RID: 281 RVA: 0x00006258 File Offset: 0x00004458
		internal AchievementManager(IntPtr ptr, IntPtr eventsPtr, ref AchievementManager.FFIEvents events)
		{
			if (eventsPtr == IntPtr.Zero)
			{
				throw new ResultException(Result.InternalError);
			}
			this.InitEvents(eventsPtr, ref events);
			this.MethodsPtr = ptr;
			if (this.MethodsPtr == IntPtr.Zero)
			{
				throw new ResultException(Result.InternalError);
			}
		}

		// Token: 0x0600011A RID: 282 RVA: 0x000062A7 File Offset: 0x000044A7
		private void InitEvents(IntPtr eventsPtr, ref AchievementManager.FFIEvents events)
		{
			events.OnUserAchievementUpdate = new AchievementManager.FFIEvents.UserAchievementUpdateHandler(AchievementManager.OnUserAchievementUpdateImpl);
			Marshal.StructureToPtr(events, eventsPtr, false);
		}

		// Token: 0x0600011B RID: 283 RVA: 0x000062D0 File Offset: 0x000044D0
		[MonoPInvokeCallback]
		private static void SetUserAchievementCallbackImpl(IntPtr ptr, Result result)
		{
			GCHandle gchandle = GCHandle.FromIntPtr(ptr);
			AchievementManager.SetUserAchievementHandler setUserAchievementHandler = (AchievementManager.SetUserAchievementHandler)gchandle.Target;
			gchandle.Free();
			setUserAchievementHandler(result);
		}

		// Token: 0x0600011C RID: 284 RVA: 0x00006300 File Offset: 0x00004500
		public void SetUserAchievement(long achievementId, byte percentComplete, AchievementManager.SetUserAchievementHandler callback)
		{
			GCHandle gchandle = GCHandle.Alloc(callback);
			this.Methods.SetUserAchievement(this.MethodsPtr, achievementId, percentComplete, GCHandle.ToIntPtr(gchandle), new AchievementManager.FFIMethods.SetUserAchievementCallback(AchievementManager.SetUserAchievementCallbackImpl));
		}

		// Token: 0x0600011D RID: 285 RVA: 0x00006340 File Offset: 0x00004540
		[MonoPInvokeCallback]
		private static void FetchUserAchievementsCallbackImpl(IntPtr ptr, Result result)
		{
			GCHandle gchandle = GCHandle.FromIntPtr(ptr);
			AchievementManager.FetchUserAchievementsHandler fetchUserAchievementsHandler = (AchievementManager.FetchUserAchievementsHandler)gchandle.Target;
			gchandle.Free();
			fetchUserAchievementsHandler(result);
		}

		// Token: 0x0600011E RID: 286 RVA: 0x00006370 File Offset: 0x00004570
		public void FetchUserAchievements(AchievementManager.FetchUserAchievementsHandler callback)
		{
			GCHandle gchandle = GCHandle.Alloc(callback);
			this.Methods.FetchUserAchievements(this.MethodsPtr, GCHandle.ToIntPtr(gchandle), new AchievementManager.FFIMethods.FetchUserAchievementsCallback(AchievementManager.FetchUserAchievementsCallbackImpl));
		}

		// Token: 0x0600011F RID: 287 RVA: 0x000063AC File Offset: 0x000045AC
		public int CountUserAchievements()
		{
			int num = 0;
			this.Methods.CountUserAchievements(this.MethodsPtr, ref num);
			return num;
		}

		// Token: 0x06000120 RID: 288 RVA: 0x000063D4 File Offset: 0x000045D4
		public UserAchievement GetUserAchievement(long userAchievementId)
		{
			UserAchievement userAchievement = default(UserAchievement);
			Result result = this.Methods.GetUserAchievement(this.MethodsPtr, userAchievementId, ref userAchievement);
			if (result != Result.Ok)
			{
				throw new ResultException(result);
			}
			return userAchievement;
		}

		// Token: 0x06000121 RID: 289 RVA: 0x00006410 File Offset: 0x00004610
		public UserAchievement GetUserAchievementAt(int index)
		{
			UserAchievement userAchievement = default(UserAchievement);
			Result result = this.Methods.GetUserAchievementAt(this.MethodsPtr, index, ref userAchievement);
			if (result != Result.Ok)
			{
				throw new ResultException(result);
			}
			return userAchievement;
		}

		// Token: 0x06000122 RID: 290 RVA: 0x0000644C File Offset: 0x0000464C
		[MonoPInvokeCallback]
		private static void OnUserAchievementUpdateImpl(IntPtr ptr, ref UserAchievement userAchievement)
		{
			Discord discord = (Discord)GCHandle.FromIntPtr(ptr).Target;
			if (discord.AchievementManagerInstance.OnUserAchievementUpdate != null)
			{
				discord.AchievementManagerInstance.OnUserAchievementUpdate(ref userAchievement);
			}
		}

		// Token: 0x0400011A RID: 282
		private IntPtr MethodsPtr;

		// Token: 0x0400011B RID: 283
		private object MethodsStructure;

		// Token: 0x0400011C RID: 284
		[CompilerGenerated]
		private AchievementManager.UserAchievementUpdateHandler OnUserAchievementUpdate;

		// Token: 0x020000D9 RID: 217
		internal struct FFIEvents
		{
			// Token: 0x0400044A RID: 1098
			internal AchievementManager.FFIEvents.UserAchievementUpdateHandler OnUserAchievementUpdate;

			// Token: 0x02000212 RID: 530
			// (Invoke) Token: 0x0600092D RID: 2349
			[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
			internal delegate void UserAchievementUpdateHandler(IntPtr ptr, ref UserAchievement userAchievement);
		}

		// Token: 0x020000DA RID: 218
		internal struct FFIMethods
		{
			// Token: 0x0400044B RID: 1099
			internal AchievementManager.FFIMethods.SetUserAchievementMethod SetUserAchievement;

			// Token: 0x0400044C RID: 1100
			internal AchievementManager.FFIMethods.FetchUserAchievementsMethod FetchUserAchievements;

			// Token: 0x0400044D RID: 1101
			internal AchievementManager.FFIMethods.CountUserAchievementsMethod CountUserAchievements;

			// Token: 0x0400044E RID: 1102
			internal AchievementManager.FFIMethods.GetUserAchievementMethod GetUserAchievement;

			// Token: 0x0400044F RID: 1103
			internal AchievementManager.FFIMethods.GetUserAchievementAtMethod GetUserAchievementAt;

			// Token: 0x02000213 RID: 531
			// (Invoke) Token: 0x06000931 RID: 2353
			[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
			internal delegate void SetUserAchievementCallback(IntPtr ptr, Result result);

			// Token: 0x02000214 RID: 532
			// (Invoke) Token: 0x06000935 RID: 2357
			[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
			internal delegate void SetUserAchievementMethod(IntPtr methodsPtr, long achievementId, byte percentComplete, IntPtr callbackData, AchievementManager.FFIMethods.SetUserAchievementCallback callback);

			// Token: 0x02000215 RID: 533
			// (Invoke) Token: 0x06000939 RID: 2361
			[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
			internal delegate void FetchUserAchievementsCallback(IntPtr ptr, Result result);

			// Token: 0x02000216 RID: 534
			// (Invoke) Token: 0x0600093D RID: 2365
			[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
			internal delegate void FetchUserAchievementsMethod(IntPtr methodsPtr, IntPtr callbackData, AchievementManager.FFIMethods.FetchUserAchievementsCallback callback);

			// Token: 0x02000217 RID: 535
			// (Invoke) Token: 0x06000941 RID: 2369
			[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
			internal delegate void CountUserAchievementsMethod(IntPtr methodsPtr, ref int count);

			// Token: 0x02000218 RID: 536
			// (Invoke) Token: 0x06000945 RID: 2373
			[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
			internal delegate Result GetUserAchievementMethod(IntPtr methodsPtr, long userAchievementId, ref UserAchievement userAchievement);

			// Token: 0x02000219 RID: 537
			// (Invoke) Token: 0x06000949 RID: 2377
			[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
			internal delegate Result GetUserAchievementAtMethod(IntPtr methodsPtr, int index, ref UserAchievement userAchievement);
		}

		// Token: 0x020000DB RID: 219
		// (Invoke) Token: 0x06000493 RID: 1171
		public delegate void SetUserAchievementHandler(Result result);

		// Token: 0x020000DC RID: 220
		// (Invoke) Token: 0x06000497 RID: 1175
		public delegate void FetchUserAchievementsHandler(Result result);

		// Token: 0x020000DD RID: 221
		// (Invoke) Token: 0x0600049B RID: 1179
		public delegate void UserAchievementUpdateHandler(ref UserAchievement userAchievement);
	}
}
