using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;

namespace Discord
{
	// Token: 0x02000033 RID: 51
	public class LobbyManager
	{
		// Token: 0x1700000A RID: 10
		// (get) Token: 0x06000071 RID: 113 RVA: 0x00003C9B File Offset: 0x00001E9B
		private LobbyManager.FFIMethods Methods
		{
			get
			{
				if (this.MethodsStructure == null)
				{
					this.MethodsStructure = Marshal.PtrToStructure(this.MethodsPtr, typeof(LobbyManager.FFIMethods));
				}
				return (LobbyManager.FFIMethods)this.MethodsStructure;
			}
		}

		// Token: 0x14000008 RID: 8
		// (add) Token: 0x06000072 RID: 114 RVA: 0x00003CCC File Offset: 0x00001ECC
		// (remove) Token: 0x06000073 RID: 115 RVA: 0x00003D04 File Offset: 0x00001F04
		public event LobbyManager.LobbyUpdateHandler OnLobbyUpdate
		{
			[CompilerGenerated]
			add
			{
				LobbyManager.LobbyUpdateHandler lobbyUpdateHandler = this.OnLobbyUpdate;
				LobbyManager.LobbyUpdateHandler lobbyUpdateHandler2;
				do
				{
					lobbyUpdateHandler2 = lobbyUpdateHandler;
					LobbyManager.LobbyUpdateHandler lobbyUpdateHandler3 = (LobbyManager.LobbyUpdateHandler)Delegate.Combine(lobbyUpdateHandler2, value);
					lobbyUpdateHandler = Interlocked.CompareExchange<LobbyManager.LobbyUpdateHandler>(ref this.OnLobbyUpdate, lobbyUpdateHandler3, lobbyUpdateHandler2);
				}
				while (lobbyUpdateHandler != lobbyUpdateHandler2);
			}
			[CompilerGenerated]
			remove
			{
				LobbyManager.LobbyUpdateHandler lobbyUpdateHandler = this.OnLobbyUpdate;
				LobbyManager.LobbyUpdateHandler lobbyUpdateHandler2;
				do
				{
					lobbyUpdateHandler2 = lobbyUpdateHandler;
					LobbyManager.LobbyUpdateHandler lobbyUpdateHandler3 = (LobbyManager.LobbyUpdateHandler)Delegate.Remove(lobbyUpdateHandler2, value);
					lobbyUpdateHandler = Interlocked.CompareExchange<LobbyManager.LobbyUpdateHandler>(ref this.OnLobbyUpdate, lobbyUpdateHandler3, lobbyUpdateHandler2);
				}
				while (lobbyUpdateHandler != lobbyUpdateHandler2);
			}
		}

		// Token: 0x14000009 RID: 9
		// (add) Token: 0x06000074 RID: 116 RVA: 0x00003D3C File Offset: 0x00001F3C
		// (remove) Token: 0x06000075 RID: 117 RVA: 0x00003D74 File Offset: 0x00001F74
		public event LobbyManager.LobbyDeleteHandler OnLobbyDelete
		{
			[CompilerGenerated]
			add
			{
				LobbyManager.LobbyDeleteHandler lobbyDeleteHandler = this.OnLobbyDelete;
				LobbyManager.LobbyDeleteHandler lobbyDeleteHandler2;
				do
				{
					lobbyDeleteHandler2 = lobbyDeleteHandler;
					LobbyManager.LobbyDeleteHandler lobbyDeleteHandler3 = (LobbyManager.LobbyDeleteHandler)Delegate.Combine(lobbyDeleteHandler2, value);
					lobbyDeleteHandler = Interlocked.CompareExchange<LobbyManager.LobbyDeleteHandler>(ref this.OnLobbyDelete, lobbyDeleteHandler3, lobbyDeleteHandler2);
				}
				while (lobbyDeleteHandler != lobbyDeleteHandler2);
			}
			[CompilerGenerated]
			remove
			{
				LobbyManager.LobbyDeleteHandler lobbyDeleteHandler = this.OnLobbyDelete;
				LobbyManager.LobbyDeleteHandler lobbyDeleteHandler2;
				do
				{
					lobbyDeleteHandler2 = lobbyDeleteHandler;
					LobbyManager.LobbyDeleteHandler lobbyDeleteHandler3 = (LobbyManager.LobbyDeleteHandler)Delegate.Remove(lobbyDeleteHandler2, value);
					lobbyDeleteHandler = Interlocked.CompareExchange<LobbyManager.LobbyDeleteHandler>(ref this.OnLobbyDelete, lobbyDeleteHandler3, lobbyDeleteHandler2);
				}
				while (lobbyDeleteHandler != lobbyDeleteHandler2);
			}
		}

		// Token: 0x1400000A RID: 10
		// (add) Token: 0x06000076 RID: 118 RVA: 0x00003DAC File Offset: 0x00001FAC
		// (remove) Token: 0x06000077 RID: 119 RVA: 0x00003DE4 File Offset: 0x00001FE4
		public event LobbyManager.MemberConnectHandler OnMemberConnect
		{
			[CompilerGenerated]
			add
			{
				LobbyManager.MemberConnectHandler memberConnectHandler = this.OnMemberConnect;
				LobbyManager.MemberConnectHandler memberConnectHandler2;
				do
				{
					memberConnectHandler2 = memberConnectHandler;
					LobbyManager.MemberConnectHandler memberConnectHandler3 = (LobbyManager.MemberConnectHandler)Delegate.Combine(memberConnectHandler2, value);
					memberConnectHandler = Interlocked.CompareExchange<LobbyManager.MemberConnectHandler>(ref this.OnMemberConnect, memberConnectHandler3, memberConnectHandler2);
				}
				while (memberConnectHandler != memberConnectHandler2);
			}
			[CompilerGenerated]
			remove
			{
				LobbyManager.MemberConnectHandler memberConnectHandler = this.OnMemberConnect;
				LobbyManager.MemberConnectHandler memberConnectHandler2;
				do
				{
					memberConnectHandler2 = memberConnectHandler;
					LobbyManager.MemberConnectHandler memberConnectHandler3 = (LobbyManager.MemberConnectHandler)Delegate.Remove(memberConnectHandler2, value);
					memberConnectHandler = Interlocked.CompareExchange<LobbyManager.MemberConnectHandler>(ref this.OnMemberConnect, memberConnectHandler3, memberConnectHandler2);
				}
				while (memberConnectHandler != memberConnectHandler2);
			}
		}

		// Token: 0x1400000B RID: 11
		// (add) Token: 0x06000078 RID: 120 RVA: 0x00003E1C File Offset: 0x0000201C
		// (remove) Token: 0x06000079 RID: 121 RVA: 0x00003E54 File Offset: 0x00002054
		public event LobbyManager.MemberUpdateHandler OnMemberUpdate
		{
			[CompilerGenerated]
			add
			{
				LobbyManager.MemberUpdateHandler memberUpdateHandler = this.OnMemberUpdate;
				LobbyManager.MemberUpdateHandler memberUpdateHandler2;
				do
				{
					memberUpdateHandler2 = memberUpdateHandler;
					LobbyManager.MemberUpdateHandler memberUpdateHandler3 = (LobbyManager.MemberUpdateHandler)Delegate.Combine(memberUpdateHandler2, value);
					memberUpdateHandler = Interlocked.CompareExchange<LobbyManager.MemberUpdateHandler>(ref this.OnMemberUpdate, memberUpdateHandler3, memberUpdateHandler2);
				}
				while (memberUpdateHandler != memberUpdateHandler2);
			}
			[CompilerGenerated]
			remove
			{
				LobbyManager.MemberUpdateHandler memberUpdateHandler = this.OnMemberUpdate;
				LobbyManager.MemberUpdateHandler memberUpdateHandler2;
				do
				{
					memberUpdateHandler2 = memberUpdateHandler;
					LobbyManager.MemberUpdateHandler memberUpdateHandler3 = (LobbyManager.MemberUpdateHandler)Delegate.Remove(memberUpdateHandler2, value);
					memberUpdateHandler = Interlocked.CompareExchange<LobbyManager.MemberUpdateHandler>(ref this.OnMemberUpdate, memberUpdateHandler3, memberUpdateHandler2);
				}
				while (memberUpdateHandler != memberUpdateHandler2);
			}
		}

		// Token: 0x1400000C RID: 12
		// (add) Token: 0x0600007A RID: 122 RVA: 0x00003E8C File Offset: 0x0000208C
		// (remove) Token: 0x0600007B RID: 123 RVA: 0x00003EC4 File Offset: 0x000020C4
		public event LobbyManager.MemberDisconnectHandler OnMemberDisconnect
		{
			[CompilerGenerated]
			add
			{
				LobbyManager.MemberDisconnectHandler memberDisconnectHandler = this.OnMemberDisconnect;
				LobbyManager.MemberDisconnectHandler memberDisconnectHandler2;
				do
				{
					memberDisconnectHandler2 = memberDisconnectHandler;
					LobbyManager.MemberDisconnectHandler memberDisconnectHandler3 = (LobbyManager.MemberDisconnectHandler)Delegate.Combine(memberDisconnectHandler2, value);
					memberDisconnectHandler = Interlocked.CompareExchange<LobbyManager.MemberDisconnectHandler>(ref this.OnMemberDisconnect, memberDisconnectHandler3, memberDisconnectHandler2);
				}
				while (memberDisconnectHandler != memberDisconnectHandler2);
			}
			[CompilerGenerated]
			remove
			{
				LobbyManager.MemberDisconnectHandler memberDisconnectHandler = this.OnMemberDisconnect;
				LobbyManager.MemberDisconnectHandler memberDisconnectHandler2;
				do
				{
					memberDisconnectHandler2 = memberDisconnectHandler;
					LobbyManager.MemberDisconnectHandler memberDisconnectHandler3 = (LobbyManager.MemberDisconnectHandler)Delegate.Remove(memberDisconnectHandler2, value);
					memberDisconnectHandler = Interlocked.CompareExchange<LobbyManager.MemberDisconnectHandler>(ref this.OnMemberDisconnect, memberDisconnectHandler3, memberDisconnectHandler2);
				}
				while (memberDisconnectHandler != memberDisconnectHandler2);
			}
		}

		// Token: 0x1400000D RID: 13
		// (add) Token: 0x0600007C RID: 124 RVA: 0x00003EFC File Offset: 0x000020FC
		// (remove) Token: 0x0600007D RID: 125 RVA: 0x00003F34 File Offset: 0x00002134
		public event LobbyManager.LobbyMessageHandler OnLobbyMessage
		{
			[CompilerGenerated]
			add
			{
				LobbyManager.LobbyMessageHandler lobbyMessageHandler = this.OnLobbyMessage;
				LobbyManager.LobbyMessageHandler lobbyMessageHandler2;
				do
				{
					lobbyMessageHandler2 = lobbyMessageHandler;
					LobbyManager.LobbyMessageHandler lobbyMessageHandler3 = (LobbyManager.LobbyMessageHandler)Delegate.Combine(lobbyMessageHandler2, value);
					lobbyMessageHandler = Interlocked.CompareExchange<LobbyManager.LobbyMessageHandler>(ref this.OnLobbyMessage, lobbyMessageHandler3, lobbyMessageHandler2);
				}
				while (lobbyMessageHandler != lobbyMessageHandler2);
			}
			[CompilerGenerated]
			remove
			{
				LobbyManager.LobbyMessageHandler lobbyMessageHandler = this.OnLobbyMessage;
				LobbyManager.LobbyMessageHandler lobbyMessageHandler2;
				do
				{
					lobbyMessageHandler2 = lobbyMessageHandler;
					LobbyManager.LobbyMessageHandler lobbyMessageHandler3 = (LobbyManager.LobbyMessageHandler)Delegate.Remove(lobbyMessageHandler2, value);
					lobbyMessageHandler = Interlocked.CompareExchange<LobbyManager.LobbyMessageHandler>(ref this.OnLobbyMessage, lobbyMessageHandler3, lobbyMessageHandler2);
				}
				while (lobbyMessageHandler != lobbyMessageHandler2);
			}
		}

		// Token: 0x1400000E RID: 14
		// (add) Token: 0x0600007E RID: 126 RVA: 0x00003F6C File Offset: 0x0000216C
		// (remove) Token: 0x0600007F RID: 127 RVA: 0x00003FA4 File Offset: 0x000021A4
		public event LobbyManager.SpeakingHandler OnSpeaking
		{
			[CompilerGenerated]
			add
			{
				LobbyManager.SpeakingHandler speakingHandler = this.OnSpeaking;
				LobbyManager.SpeakingHandler speakingHandler2;
				do
				{
					speakingHandler2 = speakingHandler;
					LobbyManager.SpeakingHandler speakingHandler3 = (LobbyManager.SpeakingHandler)Delegate.Combine(speakingHandler2, value);
					speakingHandler = Interlocked.CompareExchange<LobbyManager.SpeakingHandler>(ref this.OnSpeaking, speakingHandler3, speakingHandler2);
				}
				while (speakingHandler != speakingHandler2);
			}
			[CompilerGenerated]
			remove
			{
				LobbyManager.SpeakingHandler speakingHandler = this.OnSpeaking;
				LobbyManager.SpeakingHandler speakingHandler2;
				do
				{
					speakingHandler2 = speakingHandler;
					LobbyManager.SpeakingHandler speakingHandler3 = (LobbyManager.SpeakingHandler)Delegate.Remove(speakingHandler2, value);
					speakingHandler = Interlocked.CompareExchange<LobbyManager.SpeakingHandler>(ref this.OnSpeaking, speakingHandler3, speakingHandler2);
				}
				while (speakingHandler != speakingHandler2);
			}
		}

		// Token: 0x1400000F RID: 15
		// (add) Token: 0x06000080 RID: 128 RVA: 0x00003FDC File Offset: 0x000021DC
		// (remove) Token: 0x06000081 RID: 129 RVA: 0x00004014 File Offset: 0x00002214
		public event LobbyManager.NetworkMessageHandler OnNetworkMessage
		{
			[CompilerGenerated]
			add
			{
				LobbyManager.NetworkMessageHandler networkMessageHandler = this.OnNetworkMessage;
				LobbyManager.NetworkMessageHandler networkMessageHandler2;
				do
				{
					networkMessageHandler2 = networkMessageHandler;
					LobbyManager.NetworkMessageHandler networkMessageHandler3 = (LobbyManager.NetworkMessageHandler)Delegate.Combine(networkMessageHandler2, value);
					networkMessageHandler = Interlocked.CompareExchange<LobbyManager.NetworkMessageHandler>(ref this.OnNetworkMessage, networkMessageHandler3, networkMessageHandler2);
				}
				while (networkMessageHandler != networkMessageHandler2);
			}
			[CompilerGenerated]
			remove
			{
				LobbyManager.NetworkMessageHandler networkMessageHandler = this.OnNetworkMessage;
				LobbyManager.NetworkMessageHandler networkMessageHandler2;
				do
				{
					networkMessageHandler2 = networkMessageHandler;
					LobbyManager.NetworkMessageHandler networkMessageHandler3 = (LobbyManager.NetworkMessageHandler)Delegate.Remove(networkMessageHandler2, value);
					networkMessageHandler = Interlocked.CompareExchange<LobbyManager.NetworkMessageHandler>(ref this.OnNetworkMessage, networkMessageHandler3, networkMessageHandler2);
				}
				while (networkMessageHandler != networkMessageHandler2);
			}
		}

		// Token: 0x06000082 RID: 130 RVA: 0x0000404C File Offset: 0x0000224C
		internal LobbyManager(IntPtr ptr, IntPtr eventsPtr, ref LobbyManager.FFIEvents events)
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

		// Token: 0x06000083 RID: 131 RVA: 0x0000409C File Offset: 0x0000229C
		private void InitEvents(IntPtr eventsPtr, ref LobbyManager.FFIEvents events)
		{
			events.OnLobbyUpdate = new LobbyManager.FFIEvents.LobbyUpdateHandler(LobbyManager.OnLobbyUpdateImpl);
			events.OnLobbyDelete = new LobbyManager.FFIEvents.LobbyDeleteHandler(LobbyManager.OnLobbyDeleteImpl);
			events.OnMemberConnect = new LobbyManager.FFIEvents.MemberConnectHandler(LobbyManager.OnMemberConnectImpl);
			events.OnMemberUpdate = new LobbyManager.FFIEvents.MemberUpdateHandler(LobbyManager.OnMemberUpdateImpl);
			events.OnMemberDisconnect = new LobbyManager.FFIEvents.MemberDisconnectHandler(LobbyManager.OnMemberDisconnectImpl);
			events.OnLobbyMessage = new LobbyManager.FFIEvents.LobbyMessageHandler(LobbyManager.OnLobbyMessageImpl);
			events.OnSpeaking = new LobbyManager.FFIEvents.SpeakingHandler(LobbyManager.OnSpeakingImpl);
			events.OnNetworkMessage = new LobbyManager.FFIEvents.NetworkMessageHandler(LobbyManager.OnNetworkMessageImpl);
			Marshal.StructureToPtr(events, eventsPtr, false);
		}

		// Token: 0x06000084 RID: 132 RVA: 0x0000414C File Offset: 0x0000234C
		public LobbyTransaction GetLobbyCreateTransaction()
		{
			LobbyTransaction lobbyTransaction = default(LobbyTransaction);
			Result result = this.Methods.GetLobbyCreateTransaction(this.MethodsPtr, ref lobbyTransaction.MethodsPtr);
			if (result != Result.Ok)
			{
				throw new ResultException(result);
			}
			return lobbyTransaction;
		}

		// Token: 0x06000085 RID: 133 RVA: 0x0000418C File Offset: 0x0000238C
		public LobbyTransaction GetLobbyUpdateTransaction(long lobbyId)
		{
			LobbyTransaction lobbyTransaction = default(LobbyTransaction);
			Result result = this.Methods.GetLobbyUpdateTransaction(this.MethodsPtr, lobbyId, ref lobbyTransaction.MethodsPtr);
			if (result != Result.Ok)
			{
				throw new ResultException(result);
			}
			return lobbyTransaction;
		}

		// Token: 0x06000086 RID: 134 RVA: 0x000041CC File Offset: 0x000023CC
		public LobbyMemberTransaction GetMemberUpdateTransaction(long lobbyId, long userId)
		{
			LobbyMemberTransaction lobbyMemberTransaction = default(LobbyMemberTransaction);
			Result result = this.Methods.GetMemberUpdateTransaction(this.MethodsPtr, lobbyId, userId, ref lobbyMemberTransaction.MethodsPtr);
			if (result != Result.Ok)
			{
				throw new ResultException(result);
			}
			return lobbyMemberTransaction;
		}

		// Token: 0x06000087 RID: 135 RVA: 0x0000420C File Offset: 0x0000240C
		[MonoPInvokeCallback]
		private static void CreateLobbyCallbackImpl(IntPtr ptr, Result result, ref Lobby lobby)
		{
			GCHandle gchandle = GCHandle.FromIntPtr(ptr);
			LobbyManager.CreateLobbyHandler createLobbyHandler = (LobbyManager.CreateLobbyHandler)gchandle.Target;
			gchandle.Free();
			createLobbyHandler(result, ref lobby);
		}

		// Token: 0x06000088 RID: 136 RVA: 0x0000423C File Offset: 0x0000243C
		public void CreateLobby(LobbyTransaction transaction, LobbyManager.CreateLobbyHandler callback)
		{
			GCHandle gchandle = GCHandle.Alloc(callback);
			this.Methods.CreateLobby(this.MethodsPtr, transaction.MethodsPtr, GCHandle.ToIntPtr(gchandle), new LobbyManager.FFIMethods.CreateLobbyCallback(LobbyManager.CreateLobbyCallbackImpl));
			transaction.MethodsPtr = IntPtr.Zero;
		}

		// Token: 0x06000089 RID: 137 RVA: 0x0000428C File Offset: 0x0000248C
		[MonoPInvokeCallback]
		private static void UpdateLobbyCallbackImpl(IntPtr ptr, Result result)
		{
			GCHandle gchandle = GCHandle.FromIntPtr(ptr);
			LobbyManager.UpdateLobbyHandler updateLobbyHandler = (LobbyManager.UpdateLobbyHandler)gchandle.Target;
			gchandle.Free();
			updateLobbyHandler(result);
		}

		// Token: 0x0600008A RID: 138 RVA: 0x000042BC File Offset: 0x000024BC
		public void UpdateLobby(long lobbyId, LobbyTransaction transaction, LobbyManager.UpdateLobbyHandler callback)
		{
			GCHandle gchandle = GCHandle.Alloc(callback);
			this.Methods.UpdateLobby(this.MethodsPtr, lobbyId, transaction.MethodsPtr, GCHandle.ToIntPtr(gchandle), new LobbyManager.FFIMethods.UpdateLobbyCallback(LobbyManager.UpdateLobbyCallbackImpl));
			transaction.MethodsPtr = IntPtr.Zero;
		}

		// Token: 0x0600008B RID: 139 RVA: 0x0000430C File Offset: 0x0000250C
		[MonoPInvokeCallback]
		private static void DeleteLobbyCallbackImpl(IntPtr ptr, Result result)
		{
			GCHandle gchandle = GCHandle.FromIntPtr(ptr);
			LobbyManager.DeleteLobbyHandler deleteLobbyHandler = (LobbyManager.DeleteLobbyHandler)gchandle.Target;
			gchandle.Free();
			deleteLobbyHandler(result);
		}

		// Token: 0x0600008C RID: 140 RVA: 0x0000433C File Offset: 0x0000253C
		public void DeleteLobby(long lobbyId, LobbyManager.DeleteLobbyHandler callback)
		{
			GCHandle gchandle = GCHandle.Alloc(callback);
			this.Methods.DeleteLobby(this.MethodsPtr, lobbyId, GCHandle.ToIntPtr(gchandle), new LobbyManager.FFIMethods.DeleteLobbyCallback(LobbyManager.DeleteLobbyCallbackImpl));
		}

		// Token: 0x0600008D RID: 141 RVA: 0x0000437C File Offset: 0x0000257C
		[MonoPInvokeCallback]
		private static void ConnectLobbyCallbackImpl(IntPtr ptr, Result result, ref Lobby lobby)
		{
			GCHandle gchandle = GCHandle.FromIntPtr(ptr);
			LobbyManager.ConnectLobbyHandler connectLobbyHandler = (LobbyManager.ConnectLobbyHandler)gchandle.Target;
			gchandle.Free();
			connectLobbyHandler(result, ref lobby);
		}

		// Token: 0x0600008E RID: 142 RVA: 0x000043AC File Offset: 0x000025AC
		public void ConnectLobby(long lobbyId, string secret, LobbyManager.ConnectLobbyHandler callback)
		{
			GCHandle gchandle = GCHandle.Alloc(callback);
			this.Methods.ConnectLobby(this.MethodsPtr, lobbyId, secret, GCHandle.ToIntPtr(gchandle), new LobbyManager.FFIMethods.ConnectLobbyCallback(LobbyManager.ConnectLobbyCallbackImpl));
		}

		// Token: 0x0600008F RID: 143 RVA: 0x000043EC File Offset: 0x000025EC
		[MonoPInvokeCallback]
		private static void ConnectLobbyWithActivitySecretCallbackImpl(IntPtr ptr, Result result, ref Lobby lobby)
		{
			GCHandle gchandle = GCHandle.FromIntPtr(ptr);
			LobbyManager.ConnectLobbyWithActivitySecretHandler connectLobbyWithActivitySecretHandler = (LobbyManager.ConnectLobbyWithActivitySecretHandler)gchandle.Target;
			gchandle.Free();
			connectLobbyWithActivitySecretHandler(result, ref lobby);
		}

		// Token: 0x06000090 RID: 144 RVA: 0x0000441C File Offset: 0x0000261C
		public void ConnectLobbyWithActivitySecret(string activitySecret, LobbyManager.ConnectLobbyWithActivitySecretHandler callback)
		{
			GCHandle gchandle = GCHandle.Alloc(callback);
			this.Methods.ConnectLobbyWithActivitySecret(this.MethodsPtr, activitySecret, GCHandle.ToIntPtr(gchandle), new LobbyManager.FFIMethods.ConnectLobbyWithActivitySecretCallback(LobbyManager.ConnectLobbyWithActivitySecretCallbackImpl));
		}

		// Token: 0x06000091 RID: 145 RVA: 0x0000445C File Offset: 0x0000265C
		[MonoPInvokeCallback]
		private static void DisconnectLobbyCallbackImpl(IntPtr ptr, Result result)
		{
			GCHandle gchandle = GCHandle.FromIntPtr(ptr);
			LobbyManager.DisconnectLobbyHandler disconnectLobbyHandler = (LobbyManager.DisconnectLobbyHandler)gchandle.Target;
			gchandle.Free();
			disconnectLobbyHandler(result);
		}

		// Token: 0x06000092 RID: 146 RVA: 0x0000448C File Offset: 0x0000268C
		public void DisconnectLobby(long lobbyId, LobbyManager.DisconnectLobbyHandler callback)
		{
			GCHandle gchandle = GCHandle.Alloc(callback);
			this.Methods.DisconnectLobby(this.MethodsPtr, lobbyId, GCHandle.ToIntPtr(gchandle), new LobbyManager.FFIMethods.DisconnectLobbyCallback(LobbyManager.DisconnectLobbyCallbackImpl));
		}

		// Token: 0x06000093 RID: 147 RVA: 0x000044CC File Offset: 0x000026CC
		public Lobby GetLobby(long lobbyId)
		{
			Lobby lobby = default(Lobby);
			Result result = this.Methods.GetLobby(this.MethodsPtr, lobbyId, ref lobby);
			if (result != Result.Ok)
			{
				throw new ResultException(result);
			}
			return lobby;
		}

		// Token: 0x06000094 RID: 148 RVA: 0x00004508 File Offset: 0x00002708
		public string GetLobbyActivitySecret(long lobbyId)
		{
			StringBuilder stringBuilder = new StringBuilder(128);
			Result result = this.Methods.GetLobbyActivitySecret(this.MethodsPtr, lobbyId, stringBuilder);
			if (result != Result.Ok)
			{
				throw new ResultException(result);
			}
			return stringBuilder.ToString();
		}

		// Token: 0x06000095 RID: 149 RVA: 0x0000454C File Offset: 0x0000274C
		public string GetLobbyMetadataValue(long lobbyId, string key)
		{
			StringBuilder stringBuilder = new StringBuilder(4096);
			Result result = this.Methods.GetLobbyMetadataValue(this.MethodsPtr, lobbyId, key, stringBuilder);
			if (result != Result.Ok)
			{
				throw new ResultException(result);
			}
			return stringBuilder.ToString();
		}

		// Token: 0x06000096 RID: 150 RVA: 0x00004590 File Offset: 0x00002790
		public string GetLobbyMetadataKey(long lobbyId, int index)
		{
			StringBuilder stringBuilder = new StringBuilder(256);
			Result result = this.Methods.GetLobbyMetadataKey(this.MethodsPtr, lobbyId, index, stringBuilder);
			if (result != Result.Ok)
			{
				throw new ResultException(result);
			}
			return stringBuilder.ToString();
		}

		// Token: 0x06000097 RID: 151 RVA: 0x000045D4 File Offset: 0x000027D4
		public int LobbyMetadataCount(long lobbyId)
		{
			int num = 0;
			Result result = this.Methods.LobbyMetadataCount(this.MethodsPtr, lobbyId, ref num);
			if (result != Result.Ok)
			{
				throw new ResultException(result);
			}
			return num;
		}

		// Token: 0x06000098 RID: 152 RVA: 0x00004608 File Offset: 0x00002808
		public int MemberCount(long lobbyId)
		{
			int num = 0;
			Result result = this.Methods.MemberCount(this.MethodsPtr, lobbyId, ref num);
			if (result != Result.Ok)
			{
				throw new ResultException(result);
			}
			return num;
		}

		// Token: 0x06000099 RID: 153 RVA: 0x0000463C File Offset: 0x0000283C
		public long GetMemberUserId(long lobbyId, int index)
		{
			long num = 0L;
			Result result = this.Methods.GetMemberUserId(this.MethodsPtr, lobbyId, index, ref num);
			if (result != Result.Ok)
			{
				throw new ResultException(result);
			}
			return num;
		}

		// Token: 0x0600009A RID: 154 RVA: 0x00004674 File Offset: 0x00002874
		public User GetMemberUser(long lobbyId, long userId)
		{
			User user = default(User);
			Result result = this.Methods.GetMemberUser(this.MethodsPtr, lobbyId, userId, ref user);
			if (result != Result.Ok)
			{
				throw new ResultException(result);
			}
			return user;
		}

		// Token: 0x0600009B RID: 155 RVA: 0x000046B0 File Offset: 0x000028B0
		public string GetMemberMetadataValue(long lobbyId, long userId, string key)
		{
			StringBuilder stringBuilder = new StringBuilder(4096);
			Result result = this.Methods.GetMemberMetadataValue(this.MethodsPtr, lobbyId, userId, key, stringBuilder);
			if (result != Result.Ok)
			{
				throw new ResultException(result);
			}
			return stringBuilder.ToString();
		}

		// Token: 0x0600009C RID: 156 RVA: 0x000046F4 File Offset: 0x000028F4
		public string GetMemberMetadataKey(long lobbyId, long userId, int index)
		{
			StringBuilder stringBuilder = new StringBuilder(256);
			Result result = this.Methods.GetMemberMetadataKey(this.MethodsPtr, lobbyId, userId, index, stringBuilder);
			if (result != Result.Ok)
			{
				throw new ResultException(result);
			}
			return stringBuilder.ToString();
		}

		// Token: 0x0600009D RID: 157 RVA: 0x00004738 File Offset: 0x00002938
		public int MemberMetadataCount(long lobbyId, long userId)
		{
			int num = 0;
			Result result = this.Methods.MemberMetadataCount(this.MethodsPtr, lobbyId, userId, ref num);
			if (result != Result.Ok)
			{
				throw new ResultException(result);
			}
			return num;
		}

		// Token: 0x0600009E RID: 158 RVA: 0x00004770 File Offset: 0x00002970
		[MonoPInvokeCallback]
		private static void UpdateMemberCallbackImpl(IntPtr ptr, Result result)
		{
			GCHandle gchandle = GCHandle.FromIntPtr(ptr);
			LobbyManager.UpdateMemberHandler updateMemberHandler = (LobbyManager.UpdateMemberHandler)gchandle.Target;
			gchandle.Free();
			updateMemberHandler(result);
		}

		// Token: 0x0600009F RID: 159 RVA: 0x000047A0 File Offset: 0x000029A0
		public void UpdateMember(long lobbyId, long userId, LobbyMemberTransaction transaction, LobbyManager.UpdateMemberHandler callback)
		{
			GCHandle gchandle = GCHandle.Alloc(callback);
			this.Methods.UpdateMember(this.MethodsPtr, lobbyId, userId, transaction.MethodsPtr, GCHandle.ToIntPtr(gchandle), new LobbyManager.FFIMethods.UpdateMemberCallback(LobbyManager.UpdateMemberCallbackImpl));
			transaction.MethodsPtr = IntPtr.Zero;
		}

		// Token: 0x060000A0 RID: 160 RVA: 0x000047F4 File Offset: 0x000029F4
		[MonoPInvokeCallback]
		private static void SendLobbyMessageCallbackImpl(IntPtr ptr, Result result)
		{
			GCHandle gchandle = GCHandle.FromIntPtr(ptr);
			LobbyManager.SendLobbyMessageHandler sendLobbyMessageHandler = (LobbyManager.SendLobbyMessageHandler)gchandle.Target;
			gchandle.Free();
			sendLobbyMessageHandler(result);
		}

		// Token: 0x060000A1 RID: 161 RVA: 0x00004824 File Offset: 0x00002A24
		public void SendLobbyMessage(long lobbyId, byte[] data, LobbyManager.SendLobbyMessageHandler callback)
		{
			GCHandle gchandle = GCHandle.Alloc(callback);
			this.Methods.SendLobbyMessage(this.MethodsPtr, lobbyId, data, data.Length, GCHandle.ToIntPtr(gchandle), new LobbyManager.FFIMethods.SendLobbyMessageCallback(LobbyManager.SendLobbyMessageCallbackImpl));
		}

		// Token: 0x060000A2 RID: 162 RVA: 0x00004868 File Offset: 0x00002A68
		public LobbySearchQuery GetSearchQuery()
		{
			LobbySearchQuery lobbySearchQuery = default(LobbySearchQuery);
			Result result = this.Methods.GetSearchQuery(this.MethodsPtr, ref lobbySearchQuery.MethodsPtr);
			if (result != Result.Ok)
			{
				throw new ResultException(result);
			}
			return lobbySearchQuery;
		}

		// Token: 0x060000A3 RID: 163 RVA: 0x000048A8 File Offset: 0x00002AA8
		[MonoPInvokeCallback]
		private static void SearchCallbackImpl(IntPtr ptr, Result result)
		{
			GCHandle gchandle = GCHandle.FromIntPtr(ptr);
			LobbyManager.SearchHandler searchHandler = (LobbyManager.SearchHandler)gchandle.Target;
			gchandle.Free();
			searchHandler(result);
		}

		// Token: 0x060000A4 RID: 164 RVA: 0x000048D8 File Offset: 0x00002AD8
		public void Search(LobbySearchQuery query, LobbyManager.SearchHandler callback)
		{
			GCHandle gchandle = GCHandle.Alloc(callback);
			this.Methods.Search(this.MethodsPtr, query.MethodsPtr, GCHandle.ToIntPtr(gchandle), new LobbyManager.FFIMethods.SearchCallback(LobbyManager.SearchCallbackImpl));
			query.MethodsPtr = IntPtr.Zero;
		}

		// Token: 0x060000A5 RID: 165 RVA: 0x00004928 File Offset: 0x00002B28
		public int LobbyCount()
		{
			int num = 0;
			this.Methods.LobbyCount(this.MethodsPtr, ref num);
			return num;
		}

		// Token: 0x060000A6 RID: 166 RVA: 0x00004950 File Offset: 0x00002B50
		public long GetLobbyId(int index)
		{
			long num = 0L;
			Result result = this.Methods.GetLobbyId(this.MethodsPtr, index, ref num);
			if (result != Result.Ok)
			{
				throw new ResultException(result);
			}
			return num;
		}

		// Token: 0x060000A7 RID: 167 RVA: 0x00004988 File Offset: 0x00002B88
		[MonoPInvokeCallback]
		private static void ConnectVoiceCallbackImpl(IntPtr ptr, Result result)
		{
			GCHandle gchandle = GCHandle.FromIntPtr(ptr);
			LobbyManager.ConnectVoiceHandler connectVoiceHandler = (LobbyManager.ConnectVoiceHandler)gchandle.Target;
			gchandle.Free();
			connectVoiceHandler(result);
		}

		// Token: 0x060000A8 RID: 168 RVA: 0x000049B8 File Offset: 0x00002BB8
		public void ConnectVoice(long lobbyId, LobbyManager.ConnectVoiceHandler callback)
		{
			GCHandle gchandle = GCHandle.Alloc(callback);
			this.Methods.ConnectVoice(this.MethodsPtr, lobbyId, GCHandle.ToIntPtr(gchandle), new LobbyManager.FFIMethods.ConnectVoiceCallback(LobbyManager.ConnectVoiceCallbackImpl));
		}

		// Token: 0x060000A9 RID: 169 RVA: 0x000049F8 File Offset: 0x00002BF8
		[MonoPInvokeCallback]
		private static void DisconnectVoiceCallbackImpl(IntPtr ptr, Result result)
		{
			GCHandle gchandle = GCHandle.FromIntPtr(ptr);
			LobbyManager.DisconnectVoiceHandler disconnectVoiceHandler = (LobbyManager.DisconnectVoiceHandler)gchandle.Target;
			gchandle.Free();
			disconnectVoiceHandler(result);
		}

		// Token: 0x060000AA RID: 170 RVA: 0x00004A28 File Offset: 0x00002C28
		public void DisconnectVoice(long lobbyId, LobbyManager.DisconnectVoiceHandler callback)
		{
			GCHandle gchandle = GCHandle.Alloc(callback);
			this.Methods.DisconnectVoice(this.MethodsPtr, lobbyId, GCHandle.ToIntPtr(gchandle), new LobbyManager.FFIMethods.DisconnectVoiceCallback(LobbyManager.DisconnectVoiceCallbackImpl));
		}

		// Token: 0x060000AB RID: 171 RVA: 0x00004A68 File Offset: 0x00002C68
		public void ConnectNetwork(long lobbyId)
		{
			Result result = this.Methods.ConnectNetwork(this.MethodsPtr, lobbyId);
			if (result != Result.Ok)
			{
				throw new ResultException(result);
			}
		}

		// Token: 0x060000AC RID: 172 RVA: 0x00004A98 File Offset: 0x00002C98
		public void DisconnectNetwork(long lobbyId)
		{
			Result result = this.Methods.DisconnectNetwork(this.MethodsPtr, lobbyId);
			if (result != Result.Ok)
			{
				throw new ResultException(result);
			}
		}

		// Token: 0x060000AD RID: 173 RVA: 0x00004AC8 File Offset: 0x00002CC8
		public void FlushNetwork()
		{
			Result result = this.Methods.FlushNetwork(this.MethodsPtr);
			if (result != Result.Ok)
			{
				throw new ResultException(result);
			}
		}

		// Token: 0x060000AE RID: 174 RVA: 0x00004AF8 File Offset: 0x00002CF8
		public void OpenNetworkChannel(long lobbyId, byte channelId, bool reliable)
		{
			Result result = this.Methods.OpenNetworkChannel(this.MethodsPtr, lobbyId, channelId, reliable);
			if (result != Result.Ok)
			{
				throw new ResultException(result);
			}
		}

		// Token: 0x060000AF RID: 175 RVA: 0x00004B2C File Offset: 0x00002D2C
		public void SendNetworkMessage(long lobbyId, long userId, byte channelId, byte[] data)
		{
			Result result = this.Methods.SendNetworkMessage(this.MethodsPtr, lobbyId, userId, channelId, data, data.Length);
			if (result != Result.Ok)
			{
				throw new ResultException(result);
			}
		}

		// Token: 0x060000B0 RID: 176 RVA: 0x00004B64 File Offset: 0x00002D64
		[MonoPInvokeCallback]
		private static void OnLobbyUpdateImpl(IntPtr ptr, long lobbyId)
		{
			Discord discord = (Discord)GCHandle.FromIntPtr(ptr).Target;
			if (discord.LobbyManagerInstance.OnLobbyUpdate != null)
			{
				discord.LobbyManagerInstance.OnLobbyUpdate(lobbyId);
			}
		}

		// Token: 0x060000B1 RID: 177 RVA: 0x00004BA4 File Offset: 0x00002DA4
		[MonoPInvokeCallback]
		private static void OnLobbyDeleteImpl(IntPtr ptr, long lobbyId, uint reason)
		{
			Discord discord = (Discord)GCHandle.FromIntPtr(ptr).Target;
			if (discord.LobbyManagerInstance.OnLobbyDelete != null)
			{
				discord.LobbyManagerInstance.OnLobbyDelete(lobbyId, reason);
			}
		}

		// Token: 0x060000B2 RID: 178 RVA: 0x00004BE4 File Offset: 0x00002DE4
		[MonoPInvokeCallback]
		private static void OnMemberConnectImpl(IntPtr ptr, long lobbyId, long userId)
		{
			Discord discord = (Discord)GCHandle.FromIntPtr(ptr).Target;
			if (discord.LobbyManagerInstance.OnMemberConnect != null)
			{
				discord.LobbyManagerInstance.OnMemberConnect(lobbyId, userId);
			}
		}

		// Token: 0x060000B3 RID: 179 RVA: 0x00004C24 File Offset: 0x00002E24
		[MonoPInvokeCallback]
		private static void OnMemberUpdateImpl(IntPtr ptr, long lobbyId, long userId)
		{
			Discord discord = (Discord)GCHandle.FromIntPtr(ptr).Target;
			if (discord.LobbyManagerInstance.OnMemberUpdate != null)
			{
				discord.LobbyManagerInstance.OnMemberUpdate(lobbyId, userId);
			}
		}

		// Token: 0x060000B4 RID: 180 RVA: 0x00004C64 File Offset: 0x00002E64
		[MonoPInvokeCallback]
		private static void OnMemberDisconnectImpl(IntPtr ptr, long lobbyId, long userId)
		{
			Discord discord = (Discord)GCHandle.FromIntPtr(ptr).Target;
			if (discord.LobbyManagerInstance.OnMemberDisconnect != null)
			{
				discord.LobbyManagerInstance.OnMemberDisconnect(lobbyId, userId);
			}
		}

		// Token: 0x060000B5 RID: 181 RVA: 0x00004CA4 File Offset: 0x00002EA4
		[MonoPInvokeCallback]
		private static void OnLobbyMessageImpl(IntPtr ptr, long lobbyId, long userId, IntPtr dataPtr, int dataLen)
		{
			Discord discord = (Discord)GCHandle.FromIntPtr(ptr).Target;
			if (discord.LobbyManagerInstance.OnLobbyMessage != null)
			{
				byte[] array = new byte[dataLen];
				Marshal.Copy(dataPtr, array, 0, dataLen);
				discord.LobbyManagerInstance.OnLobbyMessage(lobbyId, userId, array);
			}
		}

		// Token: 0x060000B6 RID: 182 RVA: 0x00004CF8 File Offset: 0x00002EF8
		[MonoPInvokeCallback]
		private static void OnSpeakingImpl(IntPtr ptr, long lobbyId, long userId, bool speaking)
		{
			Discord discord = (Discord)GCHandle.FromIntPtr(ptr).Target;
			if (discord.LobbyManagerInstance.OnSpeaking != null)
			{
				discord.LobbyManagerInstance.OnSpeaking(lobbyId, userId, speaking);
			}
		}

		// Token: 0x060000B7 RID: 183 RVA: 0x00004D3C File Offset: 0x00002F3C
		[MonoPInvokeCallback]
		private static void OnNetworkMessageImpl(IntPtr ptr, long lobbyId, long userId, byte channelId, IntPtr dataPtr, int dataLen)
		{
			Discord discord = (Discord)GCHandle.FromIntPtr(ptr).Target;
			if (discord.LobbyManagerInstance.OnNetworkMessage != null)
			{
				byte[] array = new byte[dataLen];
				Marshal.Copy(dataPtr, array, 0, dataLen);
				discord.LobbyManagerInstance.OnNetworkMessage(lobbyId, userId, channelId, array);
			}
		}

		// Token: 0x060000B8 RID: 184 RVA: 0x00004D94 File Offset: 0x00002F94
		public IEnumerable<User> GetMemberUsers(long lobbyID)
		{
			int num = this.MemberCount(lobbyID);
			List<User> list = new List<User>();
			for (int i = 0; i < num; i++)
			{
				list.Add(this.GetMemberUser(lobbyID, this.GetMemberUserId(lobbyID, i)));
			}
			return list;
		}

		// Token: 0x060000B9 RID: 185 RVA: 0x00004DD1 File Offset: 0x00002FD1
		public void SendLobbyMessage(long lobbyID, string data, LobbyManager.SendLobbyMessageHandler handler)
		{
			this.SendLobbyMessage(lobbyID, Encoding.UTF8.GetBytes(data), handler);
		}

		// Token: 0x04000100 RID: 256
		private IntPtr MethodsPtr;

		// Token: 0x04000101 RID: 257
		private object MethodsStructure;

		// Token: 0x04000102 RID: 258
		[CompilerGenerated]
		private LobbyManager.LobbyUpdateHandler OnLobbyUpdate;

		// Token: 0x04000103 RID: 259
		[CompilerGenerated]
		private LobbyManager.LobbyDeleteHandler OnLobbyDelete;

		// Token: 0x04000104 RID: 260
		[CompilerGenerated]
		private LobbyManager.MemberConnectHandler OnMemberConnect;

		// Token: 0x04000105 RID: 261
		[CompilerGenerated]
		private LobbyManager.MemberUpdateHandler OnMemberUpdate;

		// Token: 0x04000106 RID: 262
		[CompilerGenerated]
		private LobbyManager.MemberDisconnectHandler OnMemberDisconnect;

		// Token: 0x04000107 RID: 263
		[CompilerGenerated]
		private LobbyManager.LobbyMessageHandler OnLobbyMessage;

		// Token: 0x04000108 RID: 264
		[CompilerGenerated]
		private LobbyManager.SpeakingHandler OnSpeaking;

		// Token: 0x04000109 RID: 265
		[CompilerGenerated]
		private LobbyManager.NetworkMessageHandler OnNetworkMessage;

		// Token: 0x020000A9 RID: 169
		internal struct FFIEvents
		{
			// Token: 0x040003EE RID: 1006
			internal LobbyManager.FFIEvents.LobbyUpdateHandler OnLobbyUpdate;

			// Token: 0x040003EF RID: 1007
			internal LobbyManager.FFIEvents.LobbyDeleteHandler OnLobbyDelete;

			// Token: 0x040003F0 RID: 1008
			internal LobbyManager.FFIEvents.MemberConnectHandler OnMemberConnect;

			// Token: 0x040003F1 RID: 1009
			internal LobbyManager.FFIEvents.MemberUpdateHandler OnMemberUpdate;

			// Token: 0x040003F2 RID: 1010
			internal LobbyManager.FFIEvents.MemberDisconnectHandler OnMemberDisconnect;

			// Token: 0x040003F3 RID: 1011
			internal LobbyManager.FFIEvents.LobbyMessageHandler OnLobbyMessage;

			// Token: 0x040003F4 RID: 1012
			internal LobbyManager.FFIEvents.SpeakingHandler OnSpeaking;

			// Token: 0x040003F5 RID: 1013
			internal LobbyManager.FFIEvents.NetworkMessageHandler OnNetworkMessage;

			// Token: 0x020001A0 RID: 416
			// (Invoke) Token: 0x06000765 RID: 1893
			[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
			internal delegate void LobbyUpdateHandler(IntPtr ptr, long lobbyId);

			// Token: 0x020001A1 RID: 417
			// (Invoke) Token: 0x06000769 RID: 1897
			[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
			internal delegate void LobbyDeleteHandler(IntPtr ptr, long lobbyId, uint reason);

			// Token: 0x020001A2 RID: 418
			// (Invoke) Token: 0x0600076D RID: 1901
			[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
			internal delegate void MemberConnectHandler(IntPtr ptr, long lobbyId, long userId);

			// Token: 0x020001A3 RID: 419
			// (Invoke) Token: 0x06000771 RID: 1905
			[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
			internal delegate void MemberUpdateHandler(IntPtr ptr, long lobbyId, long userId);

			// Token: 0x020001A4 RID: 420
			// (Invoke) Token: 0x06000775 RID: 1909
			[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
			internal delegate void MemberDisconnectHandler(IntPtr ptr, long lobbyId, long userId);

			// Token: 0x020001A5 RID: 421
			// (Invoke) Token: 0x06000779 RID: 1913
			[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
			internal delegate void LobbyMessageHandler(IntPtr ptr, long lobbyId, long userId, IntPtr dataPtr, int dataLen);

			// Token: 0x020001A6 RID: 422
			// (Invoke) Token: 0x0600077D RID: 1917
			[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
			internal delegate void SpeakingHandler(IntPtr ptr, long lobbyId, long userId, bool speaking);

			// Token: 0x020001A7 RID: 423
			// (Invoke) Token: 0x06000781 RID: 1921
			[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
			internal delegate void NetworkMessageHandler(IntPtr ptr, long lobbyId, long userId, byte channelId, IntPtr dataPtr, int dataLen);
		}

		// Token: 0x020000AA RID: 170
		internal struct FFIMethods
		{
			// Token: 0x040003F6 RID: 1014
			internal LobbyManager.FFIMethods.GetLobbyCreateTransactionMethod GetLobbyCreateTransaction;

			// Token: 0x040003F7 RID: 1015
			internal LobbyManager.FFIMethods.GetLobbyUpdateTransactionMethod GetLobbyUpdateTransaction;

			// Token: 0x040003F8 RID: 1016
			internal LobbyManager.FFIMethods.GetMemberUpdateTransactionMethod GetMemberUpdateTransaction;

			// Token: 0x040003F9 RID: 1017
			internal LobbyManager.FFIMethods.CreateLobbyMethod CreateLobby;

			// Token: 0x040003FA RID: 1018
			internal LobbyManager.FFIMethods.UpdateLobbyMethod UpdateLobby;

			// Token: 0x040003FB RID: 1019
			internal LobbyManager.FFIMethods.DeleteLobbyMethod DeleteLobby;

			// Token: 0x040003FC RID: 1020
			internal LobbyManager.FFIMethods.ConnectLobbyMethod ConnectLobby;

			// Token: 0x040003FD RID: 1021
			internal LobbyManager.FFIMethods.ConnectLobbyWithActivitySecretMethod ConnectLobbyWithActivitySecret;

			// Token: 0x040003FE RID: 1022
			internal LobbyManager.FFIMethods.DisconnectLobbyMethod DisconnectLobby;

			// Token: 0x040003FF RID: 1023
			internal LobbyManager.FFIMethods.GetLobbyMethod GetLobby;

			// Token: 0x04000400 RID: 1024
			internal LobbyManager.FFIMethods.GetLobbyActivitySecretMethod GetLobbyActivitySecret;

			// Token: 0x04000401 RID: 1025
			internal LobbyManager.FFIMethods.GetLobbyMetadataValueMethod GetLobbyMetadataValue;

			// Token: 0x04000402 RID: 1026
			internal LobbyManager.FFIMethods.GetLobbyMetadataKeyMethod GetLobbyMetadataKey;

			// Token: 0x04000403 RID: 1027
			internal LobbyManager.FFIMethods.LobbyMetadataCountMethod LobbyMetadataCount;

			// Token: 0x04000404 RID: 1028
			internal LobbyManager.FFIMethods.MemberCountMethod MemberCount;

			// Token: 0x04000405 RID: 1029
			internal LobbyManager.FFIMethods.GetMemberUserIdMethod GetMemberUserId;

			// Token: 0x04000406 RID: 1030
			internal LobbyManager.FFIMethods.GetMemberUserMethod GetMemberUser;

			// Token: 0x04000407 RID: 1031
			internal LobbyManager.FFIMethods.GetMemberMetadataValueMethod GetMemberMetadataValue;

			// Token: 0x04000408 RID: 1032
			internal LobbyManager.FFIMethods.GetMemberMetadataKeyMethod GetMemberMetadataKey;

			// Token: 0x04000409 RID: 1033
			internal LobbyManager.FFIMethods.MemberMetadataCountMethod MemberMetadataCount;

			// Token: 0x0400040A RID: 1034
			internal LobbyManager.FFIMethods.UpdateMemberMethod UpdateMember;

			// Token: 0x0400040B RID: 1035
			internal LobbyManager.FFIMethods.SendLobbyMessageMethod SendLobbyMessage;

			// Token: 0x0400040C RID: 1036
			internal LobbyManager.FFIMethods.GetSearchQueryMethod GetSearchQuery;

			// Token: 0x0400040D RID: 1037
			internal LobbyManager.FFIMethods.SearchMethod Search;

			// Token: 0x0400040E RID: 1038
			internal LobbyManager.FFIMethods.LobbyCountMethod LobbyCount;

			// Token: 0x0400040F RID: 1039
			internal LobbyManager.FFIMethods.GetLobbyIdMethod GetLobbyId;

			// Token: 0x04000410 RID: 1040
			internal LobbyManager.FFIMethods.ConnectVoiceMethod ConnectVoice;

			// Token: 0x04000411 RID: 1041
			internal LobbyManager.FFIMethods.DisconnectVoiceMethod DisconnectVoice;

			// Token: 0x04000412 RID: 1042
			internal LobbyManager.FFIMethods.ConnectNetworkMethod ConnectNetwork;

			// Token: 0x04000413 RID: 1043
			internal LobbyManager.FFIMethods.DisconnectNetworkMethod DisconnectNetwork;

			// Token: 0x04000414 RID: 1044
			internal LobbyManager.FFIMethods.FlushNetworkMethod FlushNetwork;

			// Token: 0x04000415 RID: 1045
			internal LobbyManager.FFIMethods.OpenNetworkChannelMethod OpenNetworkChannel;

			// Token: 0x04000416 RID: 1046
			internal LobbyManager.FFIMethods.SendNetworkMessageMethod SendNetworkMessage;

			// Token: 0x020001A8 RID: 424
			// (Invoke) Token: 0x06000785 RID: 1925
			[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
			internal delegate Result GetLobbyCreateTransactionMethod(IntPtr methodsPtr, ref IntPtr transaction);

			// Token: 0x020001A9 RID: 425
			// (Invoke) Token: 0x06000789 RID: 1929
			[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
			internal delegate Result GetLobbyUpdateTransactionMethod(IntPtr methodsPtr, long lobbyId, ref IntPtr transaction);

			// Token: 0x020001AA RID: 426
			// (Invoke) Token: 0x0600078D RID: 1933
			[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
			internal delegate Result GetMemberUpdateTransactionMethod(IntPtr methodsPtr, long lobbyId, long userId, ref IntPtr transaction);

			// Token: 0x020001AB RID: 427
			// (Invoke) Token: 0x06000791 RID: 1937
			[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
			internal delegate void CreateLobbyCallback(IntPtr ptr, Result result, ref Lobby lobby);

			// Token: 0x020001AC RID: 428
			// (Invoke) Token: 0x06000795 RID: 1941
			[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
			internal delegate void CreateLobbyMethod(IntPtr methodsPtr, IntPtr transaction, IntPtr callbackData, LobbyManager.FFIMethods.CreateLobbyCallback callback);

			// Token: 0x020001AD RID: 429
			// (Invoke) Token: 0x06000799 RID: 1945
			[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
			internal delegate void UpdateLobbyCallback(IntPtr ptr, Result result);

			// Token: 0x020001AE RID: 430
			// (Invoke) Token: 0x0600079D RID: 1949
			[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
			internal delegate void UpdateLobbyMethod(IntPtr methodsPtr, long lobbyId, IntPtr transaction, IntPtr callbackData, LobbyManager.FFIMethods.UpdateLobbyCallback callback);

			// Token: 0x020001AF RID: 431
			// (Invoke) Token: 0x060007A1 RID: 1953
			[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
			internal delegate void DeleteLobbyCallback(IntPtr ptr, Result result);

			// Token: 0x020001B0 RID: 432
			// (Invoke) Token: 0x060007A5 RID: 1957
			[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
			internal delegate void DeleteLobbyMethod(IntPtr methodsPtr, long lobbyId, IntPtr callbackData, LobbyManager.FFIMethods.DeleteLobbyCallback callback);

			// Token: 0x020001B1 RID: 433
			// (Invoke) Token: 0x060007A9 RID: 1961
			[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
			internal delegate void ConnectLobbyCallback(IntPtr ptr, Result result, ref Lobby lobby);

			// Token: 0x020001B2 RID: 434
			// (Invoke) Token: 0x060007AD RID: 1965
			[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
			internal delegate void ConnectLobbyMethod(IntPtr methodsPtr, long lobbyId, [MarshalAs(UnmanagedType.LPStr)] string secret, IntPtr callbackData, LobbyManager.FFIMethods.ConnectLobbyCallback callback);

			// Token: 0x020001B3 RID: 435
			// (Invoke) Token: 0x060007B1 RID: 1969
			[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
			internal delegate void ConnectLobbyWithActivitySecretCallback(IntPtr ptr, Result result, ref Lobby lobby);

			// Token: 0x020001B4 RID: 436
			// (Invoke) Token: 0x060007B5 RID: 1973
			[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
			internal delegate void ConnectLobbyWithActivitySecretMethod(IntPtr methodsPtr, [MarshalAs(UnmanagedType.LPStr)] string activitySecret, IntPtr callbackData, LobbyManager.FFIMethods.ConnectLobbyWithActivitySecretCallback callback);

			// Token: 0x020001B5 RID: 437
			// (Invoke) Token: 0x060007B9 RID: 1977
			[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
			internal delegate void DisconnectLobbyCallback(IntPtr ptr, Result result);

			// Token: 0x020001B6 RID: 438
			// (Invoke) Token: 0x060007BD RID: 1981
			[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
			internal delegate void DisconnectLobbyMethod(IntPtr methodsPtr, long lobbyId, IntPtr callbackData, LobbyManager.FFIMethods.DisconnectLobbyCallback callback);

			// Token: 0x020001B7 RID: 439
			// (Invoke) Token: 0x060007C1 RID: 1985
			[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
			internal delegate Result GetLobbyMethod(IntPtr methodsPtr, long lobbyId, ref Lobby lobby);

			// Token: 0x020001B8 RID: 440
			// (Invoke) Token: 0x060007C5 RID: 1989
			[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
			internal delegate Result GetLobbyActivitySecretMethod(IntPtr methodsPtr, long lobbyId, StringBuilder secret);

			// Token: 0x020001B9 RID: 441
			// (Invoke) Token: 0x060007C9 RID: 1993
			[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
			internal delegate Result GetLobbyMetadataValueMethod(IntPtr methodsPtr, long lobbyId, [MarshalAs(UnmanagedType.LPStr)] string key, StringBuilder value);

			// Token: 0x020001BA RID: 442
			// (Invoke) Token: 0x060007CD RID: 1997
			[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
			internal delegate Result GetLobbyMetadataKeyMethod(IntPtr methodsPtr, long lobbyId, int index, StringBuilder key);

			// Token: 0x020001BB RID: 443
			// (Invoke) Token: 0x060007D1 RID: 2001
			[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
			internal delegate Result LobbyMetadataCountMethod(IntPtr methodsPtr, long lobbyId, ref int count);

			// Token: 0x020001BC RID: 444
			// (Invoke) Token: 0x060007D5 RID: 2005
			[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
			internal delegate Result MemberCountMethod(IntPtr methodsPtr, long lobbyId, ref int count);

			// Token: 0x020001BD RID: 445
			// (Invoke) Token: 0x060007D9 RID: 2009
			[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
			internal delegate Result GetMemberUserIdMethod(IntPtr methodsPtr, long lobbyId, int index, ref long userId);

			// Token: 0x020001BE RID: 446
			// (Invoke) Token: 0x060007DD RID: 2013
			[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
			internal delegate Result GetMemberUserMethod(IntPtr methodsPtr, long lobbyId, long userId, ref User user);

			// Token: 0x020001BF RID: 447
			// (Invoke) Token: 0x060007E1 RID: 2017
			[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
			internal delegate Result GetMemberMetadataValueMethod(IntPtr methodsPtr, long lobbyId, long userId, [MarshalAs(UnmanagedType.LPStr)] string key, StringBuilder value);

			// Token: 0x020001C0 RID: 448
			// (Invoke) Token: 0x060007E5 RID: 2021
			[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
			internal delegate Result GetMemberMetadataKeyMethod(IntPtr methodsPtr, long lobbyId, long userId, int index, StringBuilder key);

			// Token: 0x020001C1 RID: 449
			// (Invoke) Token: 0x060007E9 RID: 2025
			[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
			internal delegate Result MemberMetadataCountMethod(IntPtr methodsPtr, long lobbyId, long userId, ref int count);

			// Token: 0x020001C2 RID: 450
			// (Invoke) Token: 0x060007ED RID: 2029
			[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
			internal delegate void UpdateMemberCallback(IntPtr ptr, Result result);

			// Token: 0x020001C3 RID: 451
			// (Invoke) Token: 0x060007F1 RID: 2033
			[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
			internal delegate void UpdateMemberMethod(IntPtr methodsPtr, long lobbyId, long userId, IntPtr transaction, IntPtr callbackData, LobbyManager.FFIMethods.UpdateMemberCallback callback);

			// Token: 0x020001C4 RID: 452
			// (Invoke) Token: 0x060007F5 RID: 2037
			[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
			internal delegate void SendLobbyMessageCallback(IntPtr ptr, Result result);

			// Token: 0x020001C5 RID: 453
			// (Invoke) Token: 0x060007F9 RID: 2041
			[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
			internal delegate void SendLobbyMessageMethod(IntPtr methodsPtr, long lobbyId, byte[] data, int dataLen, IntPtr callbackData, LobbyManager.FFIMethods.SendLobbyMessageCallback callback);

			// Token: 0x020001C6 RID: 454
			// (Invoke) Token: 0x060007FD RID: 2045
			[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
			internal delegate Result GetSearchQueryMethod(IntPtr methodsPtr, ref IntPtr query);

			// Token: 0x020001C7 RID: 455
			// (Invoke) Token: 0x06000801 RID: 2049
			[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
			internal delegate void SearchCallback(IntPtr ptr, Result result);

			// Token: 0x020001C8 RID: 456
			// (Invoke) Token: 0x06000805 RID: 2053
			[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
			internal delegate void SearchMethod(IntPtr methodsPtr, IntPtr query, IntPtr callbackData, LobbyManager.FFIMethods.SearchCallback callback);

			// Token: 0x020001C9 RID: 457
			// (Invoke) Token: 0x06000809 RID: 2057
			[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
			internal delegate void LobbyCountMethod(IntPtr methodsPtr, ref int count);

			// Token: 0x020001CA RID: 458
			// (Invoke) Token: 0x0600080D RID: 2061
			[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
			internal delegate Result GetLobbyIdMethod(IntPtr methodsPtr, int index, ref long lobbyId);

			// Token: 0x020001CB RID: 459
			// (Invoke) Token: 0x06000811 RID: 2065
			[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
			internal delegate void ConnectVoiceCallback(IntPtr ptr, Result result);

			// Token: 0x020001CC RID: 460
			// (Invoke) Token: 0x06000815 RID: 2069
			[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
			internal delegate void ConnectVoiceMethod(IntPtr methodsPtr, long lobbyId, IntPtr callbackData, LobbyManager.FFIMethods.ConnectVoiceCallback callback);

			// Token: 0x020001CD RID: 461
			// (Invoke) Token: 0x06000819 RID: 2073
			[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
			internal delegate void DisconnectVoiceCallback(IntPtr ptr, Result result);

			// Token: 0x020001CE RID: 462
			// (Invoke) Token: 0x0600081D RID: 2077
			[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
			internal delegate void DisconnectVoiceMethod(IntPtr methodsPtr, long lobbyId, IntPtr callbackData, LobbyManager.FFIMethods.DisconnectVoiceCallback callback);

			// Token: 0x020001CF RID: 463
			// (Invoke) Token: 0x06000821 RID: 2081
			[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
			internal delegate Result ConnectNetworkMethod(IntPtr methodsPtr, long lobbyId);

			// Token: 0x020001D0 RID: 464
			// (Invoke) Token: 0x06000825 RID: 2085
			[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
			internal delegate Result DisconnectNetworkMethod(IntPtr methodsPtr, long lobbyId);

			// Token: 0x020001D1 RID: 465
			// (Invoke) Token: 0x06000829 RID: 2089
			[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
			internal delegate Result FlushNetworkMethod(IntPtr methodsPtr);

			// Token: 0x020001D2 RID: 466
			// (Invoke) Token: 0x0600082D RID: 2093
			[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
			internal delegate Result OpenNetworkChannelMethod(IntPtr methodsPtr, long lobbyId, byte channelId, bool reliable);

			// Token: 0x020001D3 RID: 467
			// (Invoke) Token: 0x06000831 RID: 2097
			[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
			internal delegate Result SendNetworkMessageMethod(IntPtr methodsPtr, long lobbyId, long userId, byte channelId, byte[] data, int dataLen);
		}

		// Token: 0x020000AB RID: 171
		// (Invoke) Token: 0x06000403 RID: 1027
		public delegate void CreateLobbyHandler(Result result, ref Lobby lobby);

		// Token: 0x020000AC RID: 172
		// (Invoke) Token: 0x06000407 RID: 1031
		public delegate void UpdateLobbyHandler(Result result);

		// Token: 0x020000AD RID: 173
		// (Invoke) Token: 0x0600040B RID: 1035
		public delegate void DeleteLobbyHandler(Result result);

		// Token: 0x020000AE RID: 174
		// (Invoke) Token: 0x0600040F RID: 1039
		public delegate void ConnectLobbyHandler(Result result, ref Lobby lobby);

		// Token: 0x020000AF RID: 175
		// (Invoke) Token: 0x06000413 RID: 1043
		public delegate void ConnectLobbyWithActivitySecretHandler(Result result, ref Lobby lobby);

		// Token: 0x020000B0 RID: 176
		// (Invoke) Token: 0x06000417 RID: 1047
		public delegate void DisconnectLobbyHandler(Result result);

		// Token: 0x020000B1 RID: 177
		// (Invoke) Token: 0x0600041B RID: 1051
		public delegate void UpdateMemberHandler(Result result);

		// Token: 0x020000B2 RID: 178
		// (Invoke) Token: 0x0600041F RID: 1055
		public delegate void SendLobbyMessageHandler(Result result);

		// Token: 0x020000B3 RID: 179
		// (Invoke) Token: 0x06000423 RID: 1059
		public delegate void SearchHandler(Result result);

		// Token: 0x020000B4 RID: 180
		// (Invoke) Token: 0x06000427 RID: 1063
		public delegate void ConnectVoiceHandler(Result result);

		// Token: 0x020000B5 RID: 181
		// (Invoke) Token: 0x0600042B RID: 1067
		public delegate void DisconnectVoiceHandler(Result result);

		// Token: 0x020000B6 RID: 182
		// (Invoke) Token: 0x0600042F RID: 1071
		public delegate void LobbyUpdateHandler(long lobbyId);

		// Token: 0x020000B7 RID: 183
		// (Invoke) Token: 0x06000433 RID: 1075
		public delegate void LobbyDeleteHandler(long lobbyId, uint reason);

		// Token: 0x020000B8 RID: 184
		// (Invoke) Token: 0x06000437 RID: 1079
		public delegate void MemberConnectHandler(long lobbyId, long userId);

		// Token: 0x020000B9 RID: 185
		// (Invoke) Token: 0x0600043B RID: 1083
		public delegate void MemberUpdateHandler(long lobbyId, long userId);

		// Token: 0x020000BA RID: 186
		// (Invoke) Token: 0x0600043F RID: 1087
		public delegate void MemberDisconnectHandler(long lobbyId, long userId);

		// Token: 0x020000BB RID: 187
		// (Invoke) Token: 0x06000443 RID: 1091
		public delegate void LobbyMessageHandler(long lobbyId, long userId, byte[] data);

		// Token: 0x020000BC RID: 188
		// (Invoke) Token: 0x06000447 RID: 1095
		public delegate void SpeakingHandler(long lobbyId, long userId, bool speaking);

		// Token: 0x020000BD RID: 189
		// (Invoke) Token: 0x0600044B RID: 1099
		public delegate void NetworkMessageHandler(long lobbyId, long userId, byte channelId, byte[] data);
	}
}
