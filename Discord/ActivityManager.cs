using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Threading;

namespace Discord
{
	// Token: 0x02000002 RID: 2
	public class ActivityManager
	{
		// Token: 0x06000001 RID: 1 RVA: 0x00002050 File Offset: 0x00000250
		public void RegisterCommand()
		{
			this.RegisterCommand(null);
		}

		// Token: 0x17000001 RID: 1
		// (get) Token: 0x06000002 RID: 2 RVA: 0x00002059 File Offset: 0x00000259
		private ActivityManager.FFIMethods Methods
		{
			get
			{
				if (this.MethodsStructure == null)
				{
					this.MethodsStructure = Marshal.PtrToStructure(this.MethodsPtr, typeof(ActivityManager.FFIMethods));
				}
				return (ActivityManager.FFIMethods)this.MethodsStructure;
			}
		}

		// Token: 0x14000001 RID: 1
		// (add) Token: 0x06000003 RID: 3 RVA: 0x0000208C File Offset: 0x0000028C
		// (remove) Token: 0x06000004 RID: 4 RVA: 0x000020C4 File Offset: 0x000002C4
		public event ActivityManager.ActivityJoinHandler OnActivityJoin
		{
			[CompilerGenerated]
			add
			{
				ActivityManager.ActivityJoinHandler activityJoinHandler = this.OnActivityJoin;
				ActivityManager.ActivityJoinHandler activityJoinHandler2;
				do
				{
					activityJoinHandler2 = activityJoinHandler;
					ActivityManager.ActivityJoinHandler activityJoinHandler3 = (ActivityManager.ActivityJoinHandler)Delegate.Combine(activityJoinHandler2, value);
					activityJoinHandler = Interlocked.CompareExchange<ActivityManager.ActivityJoinHandler>(ref this.OnActivityJoin, activityJoinHandler3, activityJoinHandler2);
				}
				while (activityJoinHandler != activityJoinHandler2);
			}
			[CompilerGenerated]
			remove
			{
				ActivityManager.ActivityJoinHandler activityJoinHandler = this.OnActivityJoin;
				ActivityManager.ActivityJoinHandler activityJoinHandler2;
				do
				{
					activityJoinHandler2 = activityJoinHandler;
					ActivityManager.ActivityJoinHandler activityJoinHandler3 = (ActivityManager.ActivityJoinHandler)Delegate.Remove(activityJoinHandler2, value);
					activityJoinHandler = Interlocked.CompareExchange<ActivityManager.ActivityJoinHandler>(ref this.OnActivityJoin, activityJoinHandler3, activityJoinHandler2);
				}
				while (activityJoinHandler != activityJoinHandler2);
			}
		}

		// Token: 0x14000002 RID: 2
		// (add) Token: 0x06000005 RID: 5 RVA: 0x000020FC File Offset: 0x000002FC
		// (remove) Token: 0x06000006 RID: 6 RVA: 0x00002134 File Offset: 0x00000334
		public event ActivityManager.ActivitySpectateHandler OnActivitySpectate
		{
			[CompilerGenerated]
			add
			{
				ActivityManager.ActivitySpectateHandler activitySpectateHandler = this.OnActivitySpectate;
				ActivityManager.ActivitySpectateHandler activitySpectateHandler2;
				do
				{
					activitySpectateHandler2 = activitySpectateHandler;
					ActivityManager.ActivitySpectateHandler activitySpectateHandler3 = (ActivityManager.ActivitySpectateHandler)Delegate.Combine(activitySpectateHandler2, value);
					activitySpectateHandler = Interlocked.CompareExchange<ActivityManager.ActivitySpectateHandler>(ref this.OnActivitySpectate, activitySpectateHandler3, activitySpectateHandler2);
				}
				while (activitySpectateHandler != activitySpectateHandler2);
			}
			[CompilerGenerated]
			remove
			{
				ActivityManager.ActivitySpectateHandler activitySpectateHandler = this.OnActivitySpectate;
				ActivityManager.ActivitySpectateHandler activitySpectateHandler2;
				do
				{
					activitySpectateHandler2 = activitySpectateHandler;
					ActivityManager.ActivitySpectateHandler activitySpectateHandler3 = (ActivityManager.ActivitySpectateHandler)Delegate.Remove(activitySpectateHandler2, value);
					activitySpectateHandler = Interlocked.CompareExchange<ActivityManager.ActivitySpectateHandler>(ref this.OnActivitySpectate, activitySpectateHandler3, activitySpectateHandler2);
				}
				while (activitySpectateHandler != activitySpectateHandler2);
			}
		}

		// Token: 0x14000003 RID: 3
		// (add) Token: 0x06000007 RID: 7 RVA: 0x0000216C File Offset: 0x0000036C
		// (remove) Token: 0x06000008 RID: 8 RVA: 0x000021A4 File Offset: 0x000003A4
		public event ActivityManager.ActivityJoinRequestHandler OnActivityJoinRequest
		{
			[CompilerGenerated]
			add
			{
				ActivityManager.ActivityJoinRequestHandler activityJoinRequestHandler = this.OnActivityJoinRequest;
				ActivityManager.ActivityJoinRequestHandler activityJoinRequestHandler2;
				do
				{
					activityJoinRequestHandler2 = activityJoinRequestHandler;
					ActivityManager.ActivityJoinRequestHandler activityJoinRequestHandler3 = (ActivityManager.ActivityJoinRequestHandler)Delegate.Combine(activityJoinRequestHandler2, value);
					activityJoinRequestHandler = Interlocked.CompareExchange<ActivityManager.ActivityJoinRequestHandler>(ref this.OnActivityJoinRequest, activityJoinRequestHandler3, activityJoinRequestHandler2);
				}
				while (activityJoinRequestHandler != activityJoinRequestHandler2);
			}
			[CompilerGenerated]
			remove
			{
				ActivityManager.ActivityJoinRequestHandler activityJoinRequestHandler = this.OnActivityJoinRequest;
				ActivityManager.ActivityJoinRequestHandler activityJoinRequestHandler2;
				do
				{
					activityJoinRequestHandler2 = activityJoinRequestHandler;
					ActivityManager.ActivityJoinRequestHandler activityJoinRequestHandler3 = (ActivityManager.ActivityJoinRequestHandler)Delegate.Remove(activityJoinRequestHandler2, value);
					activityJoinRequestHandler = Interlocked.CompareExchange<ActivityManager.ActivityJoinRequestHandler>(ref this.OnActivityJoinRequest, activityJoinRequestHandler3, activityJoinRequestHandler2);
				}
				while (activityJoinRequestHandler != activityJoinRequestHandler2);
			}
		}

		// Token: 0x14000004 RID: 4
		// (add) Token: 0x06000009 RID: 9 RVA: 0x000021DC File Offset: 0x000003DC
		// (remove) Token: 0x0600000A RID: 10 RVA: 0x00002214 File Offset: 0x00000414
		public event ActivityManager.ActivityInviteHandler OnActivityInvite
		{
			[CompilerGenerated]
			add
			{
				ActivityManager.ActivityInviteHandler activityInviteHandler = this.OnActivityInvite;
				ActivityManager.ActivityInviteHandler activityInviteHandler2;
				do
				{
					activityInviteHandler2 = activityInviteHandler;
					ActivityManager.ActivityInviteHandler activityInviteHandler3 = (ActivityManager.ActivityInviteHandler)Delegate.Combine(activityInviteHandler2, value);
					activityInviteHandler = Interlocked.CompareExchange<ActivityManager.ActivityInviteHandler>(ref this.OnActivityInvite, activityInviteHandler3, activityInviteHandler2);
				}
				while (activityInviteHandler != activityInviteHandler2);
			}
			[CompilerGenerated]
			remove
			{
				ActivityManager.ActivityInviteHandler activityInviteHandler = this.OnActivityInvite;
				ActivityManager.ActivityInviteHandler activityInviteHandler2;
				do
				{
					activityInviteHandler2 = activityInviteHandler;
					ActivityManager.ActivityInviteHandler activityInviteHandler3 = (ActivityManager.ActivityInviteHandler)Delegate.Remove(activityInviteHandler2, value);
					activityInviteHandler = Interlocked.CompareExchange<ActivityManager.ActivityInviteHandler>(ref this.OnActivityInvite, activityInviteHandler3, activityInviteHandler2);
				}
				while (activityInviteHandler != activityInviteHandler2);
			}
		}

		// Token: 0x0600000B RID: 11 RVA: 0x0000224C File Offset: 0x0000044C
		internal ActivityManager(IntPtr ptr, IntPtr eventsPtr, ref ActivityManager.FFIEvents events)
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

		// Token: 0x0600000C RID: 12 RVA: 0x0000229C File Offset: 0x0000049C
		private void InitEvents(IntPtr eventsPtr, ref ActivityManager.FFIEvents events)
		{
			events.OnActivityJoin = new ActivityManager.FFIEvents.ActivityJoinHandler(ActivityManager.OnActivityJoinImpl);
			events.OnActivitySpectate = new ActivityManager.FFIEvents.ActivitySpectateHandler(ActivityManager.OnActivitySpectateImpl);
			events.OnActivityJoinRequest = new ActivityManager.FFIEvents.ActivityJoinRequestHandler(ActivityManager.OnActivityJoinRequestImpl);
			events.OnActivityInvite = new ActivityManager.FFIEvents.ActivityInviteHandler(ActivityManager.OnActivityInviteImpl);
			Marshal.StructureToPtr(events, eventsPtr, false);
		}

		// Token: 0x0600000D RID: 13 RVA: 0x00002304 File Offset: 0x00000504
		public void RegisterCommand(string command)
		{
			Result result = this.Methods.RegisterCommand(this.MethodsPtr, command);
			if (result != Result.Ok)
			{
				throw new ResultException(result);
			}
		}

		// Token: 0x0600000E RID: 14 RVA: 0x00002334 File Offset: 0x00000534
		public void RegisterSteam(uint steamId)
		{
			Result result = this.Methods.RegisterSteam(this.MethodsPtr, steamId);
			if (result != Result.Ok)
			{
				throw new ResultException(result);
			}
		}

		// Token: 0x0600000F RID: 15 RVA: 0x00002364 File Offset: 0x00000564
		[MonoPInvokeCallback]
		private static void UpdateActivityCallbackImpl(IntPtr ptr, Result result)
		{
			GCHandle gchandle = GCHandle.FromIntPtr(ptr);
			ActivityManager.UpdateActivityHandler updateActivityHandler = (ActivityManager.UpdateActivityHandler)gchandle.Target;
			gchandle.Free();
			updateActivityHandler(result);
		}

		// Token: 0x06000010 RID: 16 RVA: 0x00002394 File Offset: 0x00000594
		public void UpdateActivity(Activity activity, ActivityManager.UpdateActivityHandler callback)
		{
			GCHandle gchandle = GCHandle.Alloc(callback);
			this.Methods.UpdateActivity(this.MethodsPtr, ref activity, GCHandle.ToIntPtr(gchandle), new ActivityManager.FFIMethods.UpdateActivityCallback(ActivityManager.UpdateActivityCallbackImpl));
		}

		// Token: 0x06000011 RID: 17 RVA: 0x000023D4 File Offset: 0x000005D4
		[MonoPInvokeCallback]
		private static void ClearActivityCallbackImpl(IntPtr ptr, Result result)
		{
			GCHandle gchandle = GCHandle.FromIntPtr(ptr);
			ActivityManager.ClearActivityHandler clearActivityHandler = (ActivityManager.ClearActivityHandler)gchandle.Target;
			gchandle.Free();
			clearActivityHandler(result);
		}

		// Token: 0x06000012 RID: 18 RVA: 0x00002404 File Offset: 0x00000604
		public void ClearActivity(ActivityManager.ClearActivityHandler callback)
		{
			GCHandle gchandle = GCHandle.Alloc(callback);
			this.Methods.ClearActivity(this.MethodsPtr, GCHandle.ToIntPtr(gchandle), new ActivityManager.FFIMethods.ClearActivityCallback(ActivityManager.ClearActivityCallbackImpl));
		}

		// Token: 0x06000013 RID: 19 RVA: 0x00002440 File Offset: 0x00000640
		[MonoPInvokeCallback]
		private static void SendRequestReplyCallbackImpl(IntPtr ptr, Result result)
		{
			GCHandle gchandle = GCHandle.FromIntPtr(ptr);
			ActivityManager.SendRequestReplyHandler sendRequestReplyHandler = (ActivityManager.SendRequestReplyHandler)gchandle.Target;
			gchandle.Free();
			sendRequestReplyHandler(result);
		}

		// Token: 0x06000014 RID: 20 RVA: 0x00002470 File Offset: 0x00000670
		public void SendRequestReply(long userId, ActivityJoinRequestReply reply, ActivityManager.SendRequestReplyHandler callback)
		{
			GCHandle gchandle = GCHandle.Alloc(callback);
			this.Methods.SendRequestReply(this.MethodsPtr, userId, reply, GCHandle.ToIntPtr(gchandle), new ActivityManager.FFIMethods.SendRequestReplyCallback(ActivityManager.SendRequestReplyCallbackImpl));
		}

		// Token: 0x06000015 RID: 21 RVA: 0x000024B0 File Offset: 0x000006B0
		[MonoPInvokeCallback]
		private static void SendInviteCallbackImpl(IntPtr ptr, Result result)
		{
			GCHandle gchandle = GCHandle.FromIntPtr(ptr);
			ActivityManager.SendInviteHandler sendInviteHandler = (ActivityManager.SendInviteHandler)gchandle.Target;
			gchandle.Free();
			sendInviteHandler(result);
		}

		// Token: 0x06000016 RID: 22 RVA: 0x000024E0 File Offset: 0x000006E0
		public void SendInvite(long userId, ActivityActionType type, string content, ActivityManager.SendInviteHandler callback)
		{
			GCHandle gchandle = GCHandle.Alloc(callback);
			this.Methods.SendInvite(this.MethodsPtr, userId, type, content, GCHandle.ToIntPtr(gchandle), new ActivityManager.FFIMethods.SendInviteCallback(ActivityManager.SendInviteCallbackImpl));
		}

		// Token: 0x06000017 RID: 23 RVA: 0x00002520 File Offset: 0x00000720
		[MonoPInvokeCallback]
		private static void AcceptInviteCallbackImpl(IntPtr ptr, Result result)
		{
			GCHandle gchandle = GCHandle.FromIntPtr(ptr);
			ActivityManager.AcceptInviteHandler acceptInviteHandler = (ActivityManager.AcceptInviteHandler)gchandle.Target;
			gchandle.Free();
			acceptInviteHandler(result);
		}

		// Token: 0x06000018 RID: 24 RVA: 0x00002550 File Offset: 0x00000750
		public void AcceptInvite(long userId, ActivityManager.AcceptInviteHandler callback)
		{
			GCHandle gchandle = GCHandle.Alloc(callback);
			this.Methods.AcceptInvite(this.MethodsPtr, userId, GCHandle.ToIntPtr(gchandle), new ActivityManager.FFIMethods.AcceptInviteCallback(ActivityManager.AcceptInviteCallbackImpl));
		}

		// Token: 0x06000019 RID: 25 RVA: 0x00002590 File Offset: 0x00000790
		[MonoPInvokeCallback]
		private static void OnActivityJoinImpl(IntPtr ptr, string secret)
		{
			Discord discord = (Discord)GCHandle.FromIntPtr(ptr).Target;
			if (discord.ActivityManagerInstance.OnActivityJoin != null)
			{
				discord.ActivityManagerInstance.OnActivityJoin(secret);
			}
		}

		// Token: 0x0600001A RID: 26 RVA: 0x000025D0 File Offset: 0x000007D0
		[MonoPInvokeCallback]
		private static void OnActivitySpectateImpl(IntPtr ptr, string secret)
		{
			Discord discord = (Discord)GCHandle.FromIntPtr(ptr).Target;
			if (discord.ActivityManagerInstance.OnActivitySpectate != null)
			{
				discord.ActivityManagerInstance.OnActivitySpectate(secret);
			}
		}

		// Token: 0x0600001B RID: 27 RVA: 0x00002610 File Offset: 0x00000810
		[MonoPInvokeCallback]
		private static void OnActivityJoinRequestImpl(IntPtr ptr, ref User user)
		{
			Discord discord = (Discord)GCHandle.FromIntPtr(ptr).Target;
			if (discord.ActivityManagerInstance.OnActivityJoinRequest != null)
			{
				discord.ActivityManagerInstance.OnActivityJoinRequest(ref user);
			}
		}

		// Token: 0x0600001C RID: 28 RVA: 0x00002650 File Offset: 0x00000850
		[MonoPInvokeCallback]
		private static void OnActivityInviteImpl(IntPtr ptr, ActivityActionType type, ref User user, ref Activity activity)
		{
			Discord discord = (Discord)GCHandle.FromIntPtr(ptr).Target;
			if (discord.ActivityManagerInstance.OnActivityInvite != null)
			{
				discord.ActivityManagerInstance.OnActivityInvite(type, ref user, ref activity);
			}
		}

		// Token: 0x04000001 RID: 1
		private IntPtr MethodsPtr;

		// Token: 0x04000002 RID: 2
		private object MethodsStructure;

		// Token: 0x04000003 RID: 3
		[CompilerGenerated]
		private ActivityManager.ActivityJoinHandler OnActivityJoin;

		// Token: 0x04000004 RID: 4
		[CompilerGenerated]
		private ActivityManager.ActivitySpectateHandler OnActivitySpectate;

		// Token: 0x04000005 RID: 5
		[CompilerGenerated]
		private ActivityManager.ActivityJoinRequestHandler OnActivityJoinRequest;

		// Token: 0x04000006 RID: 6
		[CompilerGenerated]
		private ActivityManager.ActivityInviteHandler OnActivityInvite;

		// Token: 0x02000086 RID: 134
		internal struct FFIEvents
		{
			// Token: 0x04000399 RID: 921
			internal ActivityManager.FFIEvents.ActivityJoinHandler OnActivityJoin;

			// Token: 0x0400039A RID: 922
			internal ActivityManager.FFIEvents.ActivitySpectateHandler OnActivitySpectate;

			// Token: 0x0400039B RID: 923
			internal ActivityManager.FFIEvents.ActivityJoinRequestHandler OnActivityJoinRequest;

			// Token: 0x0400039C RID: 924
			internal ActivityManager.FFIEvents.ActivityInviteHandler OnActivityInvite;

			// Token: 0x0200015B RID: 347
			// (Invoke) Token: 0x06000651 RID: 1617
			[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
			internal delegate void ActivityJoinHandler(IntPtr ptr, [MarshalAs(UnmanagedType.LPStr)] string secret);

			// Token: 0x0200015C RID: 348
			// (Invoke) Token: 0x06000655 RID: 1621
			[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
			internal delegate void ActivitySpectateHandler(IntPtr ptr, [MarshalAs(UnmanagedType.LPStr)] string secret);

			// Token: 0x0200015D RID: 349
			// (Invoke) Token: 0x06000659 RID: 1625
			[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
			internal delegate void ActivityJoinRequestHandler(IntPtr ptr, ref User user);

			// Token: 0x0200015E RID: 350
			// (Invoke) Token: 0x0600065D RID: 1629
			[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
			internal delegate void ActivityInviteHandler(IntPtr ptr, ActivityActionType type, ref User user, ref Activity activity);
		}

		// Token: 0x02000087 RID: 135
		internal struct FFIMethods
		{
			// Token: 0x0400039D RID: 925
			internal ActivityManager.FFIMethods.RegisterCommandMethod RegisterCommand;

			// Token: 0x0400039E RID: 926
			internal ActivityManager.FFIMethods.RegisterSteamMethod RegisterSteam;

			// Token: 0x0400039F RID: 927
			internal ActivityManager.FFIMethods.UpdateActivityMethod UpdateActivity;

			// Token: 0x040003A0 RID: 928
			internal ActivityManager.FFIMethods.ClearActivityMethod ClearActivity;

			// Token: 0x040003A1 RID: 929
			internal ActivityManager.FFIMethods.SendRequestReplyMethod SendRequestReply;

			// Token: 0x040003A2 RID: 930
			internal ActivityManager.FFIMethods.SendInviteMethod SendInvite;

			// Token: 0x040003A3 RID: 931
			internal ActivityManager.FFIMethods.AcceptInviteMethod AcceptInvite;

			// Token: 0x0200015F RID: 351
			// (Invoke) Token: 0x06000661 RID: 1633
			[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
			internal delegate Result RegisterCommandMethod(IntPtr methodsPtr, [MarshalAs(UnmanagedType.LPStr)] string command);

			// Token: 0x02000160 RID: 352
			// (Invoke) Token: 0x06000665 RID: 1637
			[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
			internal delegate Result RegisterSteamMethod(IntPtr methodsPtr, uint steamId);

			// Token: 0x02000161 RID: 353
			// (Invoke) Token: 0x06000669 RID: 1641
			[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
			internal delegate void UpdateActivityCallback(IntPtr ptr, Result result);

			// Token: 0x02000162 RID: 354
			// (Invoke) Token: 0x0600066D RID: 1645
			[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
			internal delegate void UpdateActivityMethod(IntPtr methodsPtr, ref Activity activity, IntPtr callbackData, ActivityManager.FFIMethods.UpdateActivityCallback callback);

			// Token: 0x02000163 RID: 355
			// (Invoke) Token: 0x06000671 RID: 1649
			[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
			internal delegate void ClearActivityCallback(IntPtr ptr, Result result);

			// Token: 0x02000164 RID: 356
			// (Invoke) Token: 0x06000675 RID: 1653
			[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
			internal delegate void ClearActivityMethod(IntPtr methodsPtr, IntPtr callbackData, ActivityManager.FFIMethods.ClearActivityCallback callback);

			// Token: 0x02000165 RID: 357
			// (Invoke) Token: 0x06000679 RID: 1657
			[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
			internal delegate void SendRequestReplyCallback(IntPtr ptr, Result result);

			// Token: 0x02000166 RID: 358
			// (Invoke) Token: 0x0600067D RID: 1661
			[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
			internal delegate void SendRequestReplyMethod(IntPtr methodsPtr, long userId, ActivityJoinRequestReply reply, IntPtr callbackData, ActivityManager.FFIMethods.SendRequestReplyCallback callback);

			// Token: 0x02000167 RID: 359
			// (Invoke) Token: 0x06000681 RID: 1665
			[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
			internal delegate void SendInviteCallback(IntPtr ptr, Result result);

			// Token: 0x02000168 RID: 360
			// (Invoke) Token: 0x06000685 RID: 1669
			[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
			internal delegate void SendInviteMethod(IntPtr methodsPtr, long userId, ActivityActionType type, [MarshalAs(UnmanagedType.LPStr)] string content, IntPtr callbackData, ActivityManager.FFIMethods.SendInviteCallback callback);

			// Token: 0x02000169 RID: 361
			// (Invoke) Token: 0x06000689 RID: 1673
			[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
			internal delegate void AcceptInviteCallback(IntPtr ptr, Result result);

			// Token: 0x0200016A RID: 362
			// (Invoke) Token: 0x0600068D RID: 1677
			[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
			internal delegate void AcceptInviteMethod(IntPtr methodsPtr, long userId, IntPtr callbackData, ActivityManager.FFIMethods.AcceptInviteCallback callback);
		}

		// Token: 0x02000088 RID: 136
		// (Invoke) Token: 0x060003B7 RID: 951
		public delegate void UpdateActivityHandler(Result result);

		// Token: 0x02000089 RID: 137
		// (Invoke) Token: 0x060003BB RID: 955
		public delegate void ClearActivityHandler(Result result);

		// Token: 0x0200008A RID: 138
		// (Invoke) Token: 0x060003BF RID: 959
		public delegate void SendRequestReplyHandler(Result result);

		// Token: 0x0200008B RID: 139
		// (Invoke) Token: 0x060003C3 RID: 963
		public delegate void SendInviteHandler(Result result);

		// Token: 0x0200008C RID: 140
		// (Invoke) Token: 0x060003C7 RID: 967
		public delegate void AcceptInviteHandler(Result result);

		// Token: 0x0200008D RID: 141
		// (Invoke) Token: 0x060003CB RID: 971
		public delegate void ActivityJoinHandler(string secret);

		// Token: 0x0200008E RID: 142
		// (Invoke) Token: 0x060003CF RID: 975
		public delegate void ActivitySpectateHandler(string secret);

		// Token: 0x0200008F RID: 143
		// (Invoke) Token: 0x060003D3 RID: 979
		public delegate void ActivityJoinRequestHandler(ref User user);

		// Token: 0x02000090 RID: 144
		// (Invoke) Token: 0x060003D7 RID: 983
		public delegate void ActivityInviteHandler(ActivityActionType type, ref User user, ref Activity activity);
	}
}
