using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Threading;

namespace Discord
{
	// Token: 0x02000038 RID: 56
	public class VoiceManager
	{
		// Token: 0x1700000F RID: 15
		// (get) Token: 0x06000105 RID: 261 RVA: 0x00005E24 File Offset: 0x00004024
		private VoiceManager.FFIMethods Methods
		{
			get
			{
				if (this.MethodsStructure == null)
				{
					this.MethodsStructure = Marshal.PtrToStructure(this.MethodsPtr, typeof(VoiceManager.FFIMethods));
				}
				return (VoiceManager.FFIMethods)this.MethodsStructure;
			}
		}

		// Token: 0x14000015 RID: 21
		// (add) Token: 0x06000106 RID: 262 RVA: 0x00005E54 File Offset: 0x00004054
		// (remove) Token: 0x06000107 RID: 263 RVA: 0x00005E8C File Offset: 0x0000408C
		public event VoiceManager.SettingsUpdateHandler OnSettingsUpdate
		{
			[CompilerGenerated]
			add
			{
				VoiceManager.SettingsUpdateHandler settingsUpdateHandler = this.OnSettingsUpdate;
				VoiceManager.SettingsUpdateHandler settingsUpdateHandler2;
				do
				{
					settingsUpdateHandler2 = settingsUpdateHandler;
					VoiceManager.SettingsUpdateHandler settingsUpdateHandler3 = (VoiceManager.SettingsUpdateHandler)Delegate.Combine(settingsUpdateHandler2, value);
					settingsUpdateHandler = Interlocked.CompareExchange<VoiceManager.SettingsUpdateHandler>(ref this.OnSettingsUpdate, settingsUpdateHandler3, settingsUpdateHandler2);
				}
				while (settingsUpdateHandler != settingsUpdateHandler2);
			}
			[CompilerGenerated]
			remove
			{
				VoiceManager.SettingsUpdateHandler settingsUpdateHandler = this.OnSettingsUpdate;
				VoiceManager.SettingsUpdateHandler settingsUpdateHandler2;
				do
				{
					settingsUpdateHandler2 = settingsUpdateHandler;
					VoiceManager.SettingsUpdateHandler settingsUpdateHandler3 = (VoiceManager.SettingsUpdateHandler)Delegate.Remove(settingsUpdateHandler2, value);
					settingsUpdateHandler = Interlocked.CompareExchange<VoiceManager.SettingsUpdateHandler>(ref this.OnSettingsUpdate, settingsUpdateHandler3, settingsUpdateHandler2);
				}
				while (settingsUpdateHandler != settingsUpdateHandler2);
			}
		}

		// Token: 0x06000108 RID: 264 RVA: 0x00005EC4 File Offset: 0x000040C4
		internal VoiceManager(IntPtr ptr, IntPtr eventsPtr, ref VoiceManager.FFIEvents events)
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

		// Token: 0x06000109 RID: 265 RVA: 0x00005F13 File Offset: 0x00004113
		private void InitEvents(IntPtr eventsPtr, ref VoiceManager.FFIEvents events)
		{
			events.OnSettingsUpdate = new VoiceManager.FFIEvents.SettingsUpdateHandler(VoiceManager.OnSettingsUpdateImpl);
			Marshal.StructureToPtr(events, eventsPtr, false);
		}

		// Token: 0x0600010A RID: 266 RVA: 0x00005F3C File Offset: 0x0000413C
		public InputMode GetInputMode()
		{
			InputMode inputMode = default(InputMode);
			Result result = this.Methods.GetInputMode(this.MethodsPtr, ref inputMode);
			if (result != Result.Ok)
			{
				throw new ResultException(result);
			}
			return inputMode;
		}

		// Token: 0x0600010B RID: 267 RVA: 0x00005F78 File Offset: 0x00004178
		[MonoPInvokeCallback]
		private static void SetInputModeCallbackImpl(IntPtr ptr, Result result)
		{
			GCHandle gchandle = GCHandle.FromIntPtr(ptr);
			VoiceManager.SetInputModeHandler setInputModeHandler = (VoiceManager.SetInputModeHandler)gchandle.Target;
			gchandle.Free();
			setInputModeHandler(result);
		}

		// Token: 0x0600010C RID: 268 RVA: 0x00005FA8 File Offset: 0x000041A8
		public void SetInputMode(InputMode inputMode, VoiceManager.SetInputModeHandler callback)
		{
			GCHandle gchandle = GCHandle.Alloc(callback);
			this.Methods.SetInputMode(this.MethodsPtr, inputMode, GCHandle.ToIntPtr(gchandle), new VoiceManager.FFIMethods.SetInputModeCallback(VoiceManager.SetInputModeCallbackImpl));
		}

		// Token: 0x0600010D RID: 269 RVA: 0x00005FE8 File Offset: 0x000041E8
		public bool IsSelfMute()
		{
			bool flag = false;
			Result result = this.Methods.IsSelfMute(this.MethodsPtr, ref flag);
			if (result != Result.Ok)
			{
				throw new ResultException(result);
			}
			return flag;
		}

		// Token: 0x0600010E RID: 270 RVA: 0x0000601C File Offset: 0x0000421C
		public void SetSelfMute(bool mute)
		{
			Result result = this.Methods.SetSelfMute(this.MethodsPtr, mute);
			if (result != Result.Ok)
			{
				throw new ResultException(result);
			}
		}

		// Token: 0x0600010F RID: 271 RVA: 0x0000604C File Offset: 0x0000424C
		public bool IsSelfDeaf()
		{
			bool flag = false;
			Result result = this.Methods.IsSelfDeaf(this.MethodsPtr, ref flag);
			if (result != Result.Ok)
			{
				throw new ResultException(result);
			}
			return flag;
		}

		// Token: 0x06000110 RID: 272 RVA: 0x00006080 File Offset: 0x00004280
		public void SetSelfDeaf(bool deaf)
		{
			Result result = this.Methods.SetSelfDeaf(this.MethodsPtr, deaf);
			if (result != Result.Ok)
			{
				throw new ResultException(result);
			}
		}

		// Token: 0x06000111 RID: 273 RVA: 0x000060B0 File Offset: 0x000042B0
		public bool IsLocalMute(long userId)
		{
			bool flag = false;
			Result result = this.Methods.IsLocalMute(this.MethodsPtr, userId, ref flag);
			if (result != Result.Ok)
			{
				throw new ResultException(result);
			}
			return flag;
		}

		// Token: 0x06000112 RID: 274 RVA: 0x000060E4 File Offset: 0x000042E4
		public void SetLocalMute(long userId, bool mute)
		{
			Result result = this.Methods.SetLocalMute(this.MethodsPtr, userId, mute);
			if (result != Result.Ok)
			{
				throw new ResultException(result);
			}
		}

		// Token: 0x06000113 RID: 275 RVA: 0x00006114 File Offset: 0x00004314
		public byte GetLocalVolume(long userId)
		{
			byte b = 0;
			Result result = this.Methods.GetLocalVolume(this.MethodsPtr, userId, ref b);
			if (result != Result.Ok)
			{
				throw new ResultException(result);
			}
			return b;
		}

		// Token: 0x06000114 RID: 276 RVA: 0x00006148 File Offset: 0x00004348
		public void SetLocalVolume(long userId, byte volume)
		{
			Result result = this.Methods.SetLocalVolume(this.MethodsPtr, userId, volume);
			if (result != Result.Ok)
			{
				throw new ResultException(result);
			}
		}

		// Token: 0x06000115 RID: 277 RVA: 0x00006178 File Offset: 0x00004378
		[MonoPInvokeCallback]
		private static void OnSettingsUpdateImpl(IntPtr ptr)
		{
			Discord discord = (Discord)GCHandle.FromIntPtr(ptr).Target;
			if (discord.VoiceManagerInstance.OnSettingsUpdate != null)
			{
				discord.VoiceManagerInstance.OnSettingsUpdate();
			}
		}

		// Token: 0x04000117 RID: 279
		private IntPtr MethodsPtr;

		// Token: 0x04000118 RID: 280
		private object MethodsStructure;

		// Token: 0x04000119 RID: 281
		[CompilerGenerated]
		private VoiceManager.SettingsUpdateHandler OnSettingsUpdate;

		// Token: 0x020000D5 RID: 213
		internal struct FFIEvents
		{
			// Token: 0x0400043F RID: 1087
			internal VoiceManager.FFIEvents.SettingsUpdateHandler OnSettingsUpdate;

			// Token: 0x02000206 RID: 518
			// (Invoke) Token: 0x060008FD RID: 2301
			[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
			internal delegate void SettingsUpdateHandler(IntPtr ptr);
		}

		// Token: 0x020000D6 RID: 214
		internal struct FFIMethods
		{
			// Token: 0x04000440 RID: 1088
			internal VoiceManager.FFIMethods.GetInputModeMethod GetInputMode;

			// Token: 0x04000441 RID: 1089
			internal VoiceManager.FFIMethods.SetInputModeMethod SetInputMode;

			// Token: 0x04000442 RID: 1090
			internal VoiceManager.FFIMethods.IsSelfMuteMethod IsSelfMute;

			// Token: 0x04000443 RID: 1091
			internal VoiceManager.FFIMethods.SetSelfMuteMethod SetSelfMute;

			// Token: 0x04000444 RID: 1092
			internal VoiceManager.FFIMethods.IsSelfDeafMethod IsSelfDeaf;

			// Token: 0x04000445 RID: 1093
			internal VoiceManager.FFIMethods.SetSelfDeafMethod SetSelfDeaf;

			// Token: 0x04000446 RID: 1094
			internal VoiceManager.FFIMethods.IsLocalMuteMethod IsLocalMute;

			// Token: 0x04000447 RID: 1095
			internal VoiceManager.FFIMethods.SetLocalMuteMethod SetLocalMute;

			// Token: 0x04000448 RID: 1096
			internal VoiceManager.FFIMethods.GetLocalVolumeMethod GetLocalVolume;

			// Token: 0x04000449 RID: 1097
			internal VoiceManager.FFIMethods.SetLocalVolumeMethod SetLocalVolume;

			// Token: 0x02000207 RID: 519
			// (Invoke) Token: 0x06000901 RID: 2305
			[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
			internal delegate Result GetInputModeMethod(IntPtr methodsPtr, ref InputMode inputMode);

			// Token: 0x02000208 RID: 520
			// (Invoke) Token: 0x06000905 RID: 2309
			[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
			internal delegate void SetInputModeCallback(IntPtr ptr, Result result);

			// Token: 0x02000209 RID: 521
			// (Invoke) Token: 0x06000909 RID: 2313
			[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
			internal delegate void SetInputModeMethod(IntPtr methodsPtr, InputMode inputMode, IntPtr callbackData, VoiceManager.FFIMethods.SetInputModeCallback callback);

			// Token: 0x0200020A RID: 522
			// (Invoke) Token: 0x0600090D RID: 2317
			[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
			internal delegate Result IsSelfMuteMethod(IntPtr methodsPtr, ref bool mute);

			// Token: 0x0200020B RID: 523
			// (Invoke) Token: 0x06000911 RID: 2321
			[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
			internal delegate Result SetSelfMuteMethod(IntPtr methodsPtr, bool mute);

			// Token: 0x0200020C RID: 524
			// (Invoke) Token: 0x06000915 RID: 2325
			[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
			internal delegate Result IsSelfDeafMethod(IntPtr methodsPtr, ref bool deaf);

			// Token: 0x0200020D RID: 525
			// (Invoke) Token: 0x06000919 RID: 2329
			[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
			internal delegate Result SetSelfDeafMethod(IntPtr methodsPtr, bool deaf);

			// Token: 0x0200020E RID: 526
			// (Invoke) Token: 0x0600091D RID: 2333
			[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
			internal delegate Result IsLocalMuteMethod(IntPtr methodsPtr, long userId, ref bool mute);

			// Token: 0x0200020F RID: 527
			// (Invoke) Token: 0x06000921 RID: 2337
			[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
			internal delegate Result SetLocalMuteMethod(IntPtr methodsPtr, long userId, bool mute);

			// Token: 0x02000210 RID: 528
			// (Invoke) Token: 0x06000925 RID: 2341
			[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
			internal delegate Result GetLocalVolumeMethod(IntPtr methodsPtr, long userId, ref byte volume);

			// Token: 0x02000211 RID: 529
			// (Invoke) Token: 0x06000929 RID: 2345
			[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
			internal delegate Result SetLocalVolumeMethod(IntPtr methodsPtr, long userId, byte volume);
		}

		// Token: 0x020000D7 RID: 215
		// (Invoke) Token: 0x0600048B RID: 1163
		public delegate void SetInputModeHandler(Result result);

		// Token: 0x020000D8 RID: 216
		// (Invoke) Token: 0x0600048F RID: 1167
		public delegate void SettingsUpdateHandler();
	}
}
