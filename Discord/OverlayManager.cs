using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Threading;

namespace Discord
{
	// Token: 0x02000035 RID: 53
	public class OverlayManager
	{
		// Token: 0x1700000C RID: 12
		// (get) Token: 0x060000CB RID: 203 RVA: 0x00005197 File Offset: 0x00003397
		private OverlayManager.FFIMethods Methods
		{
			get
			{
				if (this.MethodsStructure == null)
				{
					this.MethodsStructure = Marshal.PtrToStructure(this.MethodsPtr, typeof(OverlayManager.FFIMethods));
				}
				return (OverlayManager.FFIMethods)this.MethodsStructure;
			}
		}

		// Token: 0x14000012 RID: 18
		// (add) Token: 0x060000CC RID: 204 RVA: 0x000051C8 File Offset: 0x000033C8
		// (remove) Token: 0x060000CD RID: 205 RVA: 0x00005200 File Offset: 0x00003400
		public event OverlayManager.ToggleHandler OnToggle
		{
			[CompilerGenerated]
			add
			{
				OverlayManager.ToggleHandler toggleHandler = this.OnToggle;
				OverlayManager.ToggleHandler toggleHandler2;
				do
				{
					toggleHandler2 = toggleHandler;
					OverlayManager.ToggleHandler toggleHandler3 = (OverlayManager.ToggleHandler)Delegate.Combine(toggleHandler2, value);
					toggleHandler = Interlocked.CompareExchange<OverlayManager.ToggleHandler>(ref this.OnToggle, toggleHandler3, toggleHandler2);
				}
				while (toggleHandler != toggleHandler2);
			}
			[CompilerGenerated]
			remove
			{
				OverlayManager.ToggleHandler toggleHandler = this.OnToggle;
				OverlayManager.ToggleHandler toggleHandler2;
				do
				{
					toggleHandler2 = toggleHandler;
					OverlayManager.ToggleHandler toggleHandler3 = (OverlayManager.ToggleHandler)Delegate.Remove(toggleHandler2, value);
					toggleHandler = Interlocked.CompareExchange<OverlayManager.ToggleHandler>(ref this.OnToggle, toggleHandler3, toggleHandler2);
				}
				while (toggleHandler != toggleHandler2);
			}
		}

		// Token: 0x060000CE RID: 206 RVA: 0x00005238 File Offset: 0x00003438
		internal OverlayManager(IntPtr ptr, IntPtr eventsPtr, ref OverlayManager.FFIEvents events)
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

		// Token: 0x060000CF RID: 207 RVA: 0x00005287 File Offset: 0x00003487
		private void InitEvents(IntPtr eventsPtr, ref OverlayManager.FFIEvents events)
		{
			events.OnToggle = new OverlayManager.FFIEvents.ToggleHandler(OverlayManager.OnToggleImpl);
			Marshal.StructureToPtr(events, eventsPtr, false);
		}

		// Token: 0x060000D0 RID: 208 RVA: 0x000052B0 File Offset: 0x000034B0
		public bool IsEnabled()
		{
			bool flag = false;
			this.Methods.IsEnabled(this.MethodsPtr, ref flag);
			return flag;
		}

		// Token: 0x060000D1 RID: 209 RVA: 0x000052D8 File Offset: 0x000034D8
		public bool IsLocked()
		{
			bool flag = false;
			this.Methods.IsLocked(this.MethodsPtr, ref flag);
			return flag;
		}

		// Token: 0x060000D2 RID: 210 RVA: 0x00005300 File Offset: 0x00003500
		[MonoPInvokeCallback]
		private static void SetLockedCallbackImpl(IntPtr ptr, Result result)
		{
			GCHandle gchandle = GCHandle.FromIntPtr(ptr);
			OverlayManager.SetLockedHandler setLockedHandler = (OverlayManager.SetLockedHandler)gchandle.Target;
			gchandle.Free();
			setLockedHandler(result);
		}

		// Token: 0x060000D3 RID: 211 RVA: 0x00005330 File Offset: 0x00003530
		public void SetLocked(bool locked, OverlayManager.SetLockedHandler callback)
		{
			GCHandle gchandle = GCHandle.Alloc(callback);
			this.Methods.SetLocked(this.MethodsPtr, locked, GCHandle.ToIntPtr(gchandle), new OverlayManager.FFIMethods.SetLockedCallback(OverlayManager.SetLockedCallbackImpl));
		}

		// Token: 0x060000D4 RID: 212 RVA: 0x00005370 File Offset: 0x00003570
		[MonoPInvokeCallback]
		private static void OpenActivityInviteCallbackImpl(IntPtr ptr, Result result)
		{
			GCHandle gchandle = GCHandle.FromIntPtr(ptr);
			OverlayManager.OpenActivityInviteHandler openActivityInviteHandler = (OverlayManager.OpenActivityInviteHandler)gchandle.Target;
			gchandle.Free();
			openActivityInviteHandler(result);
		}

		// Token: 0x060000D5 RID: 213 RVA: 0x000053A0 File Offset: 0x000035A0
		public void OpenActivityInvite(ActivityActionType type, OverlayManager.OpenActivityInviteHandler callback)
		{
			GCHandle gchandle = GCHandle.Alloc(callback);
			this.Methods.OpenActivityInvite(this.MethodsPtr, type, GCHandle.ToIntPtr(gchandle), new OverlayManager.FFIMethods.OpenActivityInviteCallback(OverlayManager.OpenActivityInviteCallbackImpl));
		}

		// Token: 0x060000D6 RID: 214 RVA: 0x000053E0 File Offset: 0x000035E0
		[MonoPInvokeCallback]
		private static void OpenGuildInviteCallbackImpl(IntPtr ptr, Result result)
		{
			GCHandle gchandle = GCHandle.FromIntPtr(ptr);
			OverlayManager.OpenGuildInviteHandler openGuildInviteHandler = (OverlayManager.OpenGuildInviteHandler)gchandle.Target;
			gchandle.Free();
			openGuildInviteHandler(result);
		}

		// Token: 0x060000D7 RID: 215 RVA: 0x00005410 File Offset: 0x00003610
		public void OpenGuildInvite(string code, OverlayManager.OpenGuildInviteHandler callback)
		{
			GCHandle gchandle = GCHandle.Alloc(callback);
			this.Methods.OpenGuildInvite(this.MethodsPtr, code, GCHandle.ToIntPtr(gchandle), new OverlayManager.FFIMethods.OpenGuildInviteCallback(OverlayManager.OpenGuildInviteCallbackImpl));
		}

		// Token: 0x060000D8 RID: 216 RVA: 0x00005450 File Offset: 0x00003650
		[MonoPInvokeCallback]
		private static void OpenVoiceSettingsCallbackImpl(IntPtr ptr, Result result)
		{
			GCHandle gchandle = GCHandle.FromIntPtr(ptr);
			OverlayManager.OpenVoiceSettingsHandler openVoiceSettingsHandler = (OverlayManager.OpenVoiceSettingsHandler)gchandle.Target;
			gchandle.Free();
			openVoiceSettingsHandler(result);
		}

		// Token: 0x060000D9 RID: 217 RVA: 0x00005480 File Offset: 0x00003680
		public void OpenVoiceSettings(OverlayManager.OpenVoiceSettingsHandler callback)
		{
			GCHandle gchandle = GCHandle.Alloc(callback);
			this.Methods.OpenVoiceSettings(this.MethodsPtr, GCHandle.ToIntPtr(gchandle), new OverlayManager.FFIMethods.OpenVoiceSettingsCallback(OverlayManager.OpenVoiceSettingsCallbackImpl));
		}

		// Token: 0x060000DA RID: 218 RVA: 0x000054BC File Offset: 0x000036BC
		[MonoPInvokeCallback]
		private static void OnToggleImpl(IntPtr ptr, bool locked)
		{
			Discord discord = (Discord)GCHandle.FromIntPtr(ptr).Target;
			if (discord.OverlayManagerInstance.OnToggle != null)
			{
				discord.OverlayManagerInstance.OnToggle(locked);
			}
		}

		// Token: 0x0400010E RID: 270
		private IntPtr MethodsPtr;

		// Token: 0x0400010F RID: 271
		private object MethodsStructure;

		// Token: 0x04000110 RID: 272
		[CompilerGenerated]
		private OverlayManager.ToggleHandler OnToggle;

		// Token: 0x020000C2 RID: 194
		internal struct FFIEvents
		{
			// Token: 0x04000421 RID: 1057
			internal OverlayManager.FFIEvents.ToggleHandler OnToggle;

			// Token: 0x020001DE RID: 478
			// (Invoke) Token: 0x0600085D RID: 2141
			[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
			internal delegate void ToggleHandler(IntPtr ptr, bool locked);
		}

		// Token: 0x020000C3 RID: 195
		internal struct FFIMethods
		{
			// Token: 0x04000422 RID: 1058
			internal OverlayManager.FFIMethods.IsEnabledMethod IsEnabled;

			// Token: 0x04000423 RID: 1059
			internal OverlayManager.FFIMethods.IsLockedMethod IsLocked;

			// Token: 0x04000424 RID: 1060
			internal OverlayManager.FFIMethods.SetLockedMethod SetLocked;

			// Token: 0x04000425 RID: 1061
			internal OverlayManager.FFIMethods.OpenActivityInviteMethod OpenActivityInvite;

			// Token: 0x04000426 RID: 1062
			internal OverlayManager.FFIMethods.OpenGuildInviteMethod OpenGuildInvite;

			// Token: 0x04000427 RID: 1063
			internal OverlayManager.FFIMethods.OpenVoiceSettingsMethod OpenVoiceSettings;

			// Token: 0x020001DF RID: 479
			// (Invoke) Token: 0x06000861 RID: 2145
			[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
			internal delegate void IsEnabledMethod(IntPtr methodsPtr, ref bool enabled);

			// Token: 0x020001E0 RID: 480
			// (Invoke) Token: 0x06000865 RID: 2149
			[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
			internal delegate void IsLockedMethod(IntPtr methodsPtr, ref bool locked);

			// Token: 0x020001E1 RID: 481
			// (Invoke) Token: 0x06000869 RID: 2153
			[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
			internal delegate void SetLockedCallback(IntPtr ptr, Result result);

			// Token: 0x020001E2 RID: 482
			// (Invoke) Token: 0x0600086D RID: 2157
			[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
			internal delegate void SetLockedMethod(IntPtr methodsPtr, bool locked, IntPtr callbackData, OverlayManager.FFIMethods.SetLockedCallback callback);

			// Token: 0x020001E3 RID: 483
			// (Invoke) Token: 0x06000871 RID: 2161
			[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
			internal delegate void OpenActivityInviteCallback(IntPtr ptr, Result result);

			// Token: 0x020001E4 RID: 484
			// (Invoke) Token: 0x06000875 RID: 2165
			[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
			internal delegate void OpenActivityInviteMethod(IntPtr methodsPtr, ActivityActionType type, IntPtr callbackData, OverlayManager.FFIMethods.OpenActivityInviteCallback callback);

			// Token: 0x020001E5 RID: 485
			// (Invoke) Token: 0x06000879 RID: 2169
			[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
			internal delegate void OpenGuildInviteCallback(IntPtr ptr, Result result);

			// Token: 0x020001E6 RID: 486
			// (Invoke) Token: 0x0600087D RID: 2173
			[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
			internal delegate void OpenGuildInviteMethod(IntPtr methodsPtr, [MarshalAs(UnmanagedType.LPStr)] string code, IntPtr callbackData, OverlayManager.FFIMethods.OpenGuildInviteCallback callback);

			// Token: 0x020001E7 RID: 487
			// (Invoke) Token: 0x06000881 RID: 2177
			[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
			internal delegate void OpenVoiceSettingsCallback(IntPtr ptr, Result result);

			// Token: 0x020001E8 RID: 488
			// (Invoke) Token: 0x06000885 RID: 2181
			[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
			internal delegate void OpenVoiceSettingsMethod(IntPtr methodsPtr, IntPtr callbackData, OverlayManager.FFIMethods.OpenVoiceSettingsCallback callback);
		}

		// Token: 0x020000C4 RID: 196
		// (Invoke) Token: 0x06000457 RID: 1111
		public delegate void SetLockedHandler(Result result);

		// Token: 0x020000C5 RID: 197
		// (Invoke) Token: 0x0600045B RID: 1115
		public delegate void OpenActivityInviteHandler(Result result);

		// Token: 0x020000C6 RID: 198
		// (Invoke) Token: 0x0600045F RID: 1119
		public delegate void OpenGuildInviteHandler(Result result);

		// Token: 0x020000C7 RID: 199
		// (Invoke) Token: 0x06000463 RID: 1123
		public delegate void OpenVoiceSettingsHandler(Result result);

		// Token: 0x020000C8 RID: 200
		// (Invoke) Token: 0x06000467 RID: 1127
		public delegate void ToggleHandler(bool locked);
	}
}
