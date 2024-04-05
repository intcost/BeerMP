using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Threading;

namespace Discord
{
	// Token: 0x02000034 RID: 52
	public class NetworkManager
	{
		// Token: 0x1700000B RID: 11
		// (get) Token: 0x060000BA RID: 186 RVA: 0x00004DE6 File Offset: 0x00002FE6
		private NetworkManager.FFIMethods Methods
		{
			get
			{
				if (this.MethodsStructure == null)
				{
					this.MethodsStructure = Marshal.PtrToStructure(this.MethodsPtr, typeof(NetworkManager.FFIMethods));
				}
				return (NetworkManager.FFIMethods)this.MethodsStructure;
			}
		}

		// Token: 0x14000010 RID: 16
		// (add) Token: 0x060000BB RID: 187 RVA: 0x00004E18 File Offset: 0x00003018
		// (remove) Token: 0x060000BC RID: 188 RVA: 0x00004E50 File Offset: 0x00003050
		public event NetworkManager.MessageHandler OnMessage
		{
			[CompilerGenerated]
			add
			{
				NetworkManager.MessageHandler messageHandler = this.OnMessage;
				NetworkManager.MessageHandler messageHandler2;
				do
				{
					messageHandler2 = messageHandler;
					NetworkManager.MessageHandler messageHandler3 = (NetworkManager.MessageHandler)Delegate.Combine(messageHandler2, value);
					messageHandler = Interlocked.CompareExchange<NetworkManager.MessageHandler>(ref this.OnMessage, messageHandler3, messageHandler2);
				}
				while (messageHandler != messageHandler2);
			}
			[CompilerGenerated]
			remove
			{
				NetworkManager.MessageHandler messageHandler = this.OnMessage;
				NetworkManager.MessageHandler messageHandler2;
				do
				{
					messageHandler2 = messageHandler;
					NetworkManager.MessageHandler messageHandler3 = (NetworkManager.MessageHandler)Delegate.Remove(messageHandler2, value);
					messageHandler = Interlocked.CompareExchange<NetworkManager.MessageHandler>(ref this.OnMessage, messageHandler3, messageHandler2);
				}
				while (messageHandler != messageHandler2);
			}
		}

		// Token: 0x14000011 RID: 17
		// (add) Token: 0x060000BD RID: 189 RVA: 0x00004E88 File Offset: 0x00003088
		// (remove) Token: 0x060000BE RID: 190 RVA: 0x00004EC0 File Offset: 0x000030C0
		public event NetworkManager.RouteUpdateHandler OnRouteUpdate
		{
			[CompilerGenerated]
			add
			{
				NetworkManager.RouteUpdateHandler routeUpdateHandler = this.OnRouteUpdate;
				NetworkManager.RouteUpdateHandler routeUpdateHandler2;
				do
				{
					routeUpdateHandler2 = routeUpdateHandler;
					NetworkManager.RouteUpdateHandler routeUpdateHandler3 = (NetworkManager.RouteUpdateHandler)Delegate.Combine(routeUpdateHandler2, value);
					routeUpdateHandler = Interlocked.CompareExchange<NetworkManager.RouteUpdateHandler>(ref this.OnRouteUpdate, routeUpdateHandler3, routeUpdateHandler2);
				}
				while (routeUpdateHandler != routeUpdateHandler2);
			}
			[CompilerGenerated]
			remove
			{
				NetworkManager.RouteUpdateHandler routeUpdateHandler = this.OnRouteUpdate;
				NetworkManager.RouteUpdateHandler routeUpdateHandler2;
				do
				{
					routeUpdateHandler2 = routeUpdateHandler;
					NetworkManager.RouteUpdateHandler routeUpdateHandler3 = (NetworkManager.RouteUpdateHandler)Delegate.Remove(routeUpdateHandler2, value);
					routeUpdateHandler = Interlocked.CompareExchange<NetworkManager.RouteUpdateHandler>(ref this.OnRouteUpdate, routeUpdateHandler3, routeUpdateHandler2);
				}
				while (routeUpdateHandler != routeUpdateHandler2);
			}
		}

		// Token: 0x060000BF RID: 191 RVA: 0x00004EF8 File Offset: 0x000030F8
		internal NetworkManager(IntPtr ptr, IntPtr eventsPtr, ref NetworkManager.FFIEvents events)
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

		// Token: 0x060000C0 RID: 192 RVA: 0x00004F47 File Offset: 0x00003147
		private void InitEvents(IntPtr eventsPtr, ref NetworkManager.FFIEvents events)
		{
			events.OnMessage = new NetworkManager.FFIEvents.MessageHandler(NetworkManager.OnMessageImpl);
			events.OnRouteUpdate = new NetworkManager.FFIEvents.RouteUpdateHandler(NetworkManager.OnRouteUpdateImpl);
			Marshal.StructureToPtr(events, eventsPtr, false);
		}

		// Token: 0x060000C1 RID: 193 RVA: 0x00004F80 File Offset: 0x00003180
		public ulong GetPeerId()
		{
			ulong num = 0UL;
			this.Methods.GetPeerId(this.MethodsPtr, ref num);
			return num;
		}

		// Token: 0x060000C2 RID: 194 RVA: 0x00004FAC File Offset: 0x000031AC
		public void Flush()
		{
			Result result = this.Methods.Flush(this.MethodsPtr);
			if (result != Result.Ok)
			{
				throw new ResultException(result);
			}
		}

		// Token: 0x060000C3 RID: 195 RVA: 0x00004FDC File Offset: 0x000031DC
		public void OpenPeer(ulong peerId, string routeData)
		{
			Result result = this.Methods.OpenPeer(this.MethodsPtr, peerId, routeData);
			if (result != Result.Ok)
			{
				throw new ResultException(result);
			}
		}

		// Token: 0x060000C4 RID: 196 RVA: 0x0000500C File Offset: 0x0000320C
		public void UpdatePeer(ulong peerId, string routeData)
		{
			Result result = this.Methods.UpdatePeer(this.MethodsPtr, peerId, routeData);
			if (result != Result.Ok)
			{
				throw new ResultException(result);
			}
		}

		// Token: 0x060000C5 RID: 197 RVA: 0x0000503C File Offset: 0x0000323C
		public void ClosePeer(ulong peerId)
		{
			Result result = this.Methods.ClosePeer(this.MethodsPtr, peerId);
			if (result != Result.Ok)
			{
				throw new ResultException(result);
			}
		}

		// Token: 0x060000C6 RID: 198 RVA: 0x0000506C File Offset: 0x0000326C
		public void OpenChannel(ulong peerId, byte channelId, bool reliable)
		{
			Result result = this.Methods.OpenChannel(this.MethodsPtr, peerId, channelId, reliable);
			if (result != Result.Ok)
			{
				throw new ResultException(result);
			}
		}

		// Token: 0x060000C7 RID: 199 RVA: 0x000050A0 File Offset: 0x000032A0
		public void CloseChannel(ulong peerId, byte channelId)
		{
			Result result = this.Methods.CloseChannel(this.MethodsPtr, peerId, channelId);
			if (result != Result.Ok)
			{
				throw new ResultException(result);
			}
		}

		// Token: 0x060000C8 RID: 200 RVA: 0x000050D0 File Offset: 0x000032D0
		public void SendMessage(ulong peerId, byte channelId, byte[] data)
		{
			Result result = this.Methods.SendMessage(this.MethodsPtr, peerId, channelId, data, data.Length);
			if (result != Result.Ok)
			{
				throw new ResultException(result);
			}
		}

		// Token: 0x060000C9 RID: 201 RVA: 0x00005104 File Offset: 0x00003304
		[MonoPInvokeCallback]
		private static void OnMessageImpl(IntPtr ptr, ulong peerId, byte channelId, IntPtr dataPtr, int dataLen)
		{
			Discord discord = (Discord)GCHandle.FromIntPtr(ptr).Target;
			if (discord.NetworkManagerInstance.OnMessage != null)
			{
				byte[] array = new byte[dataLen];
				Marshal.Copy(dataPtr, array, 0, dataLen);
				discord.NetworkManagerInstance.OnMessage(peerId, channelId, array);
			}
		}

		// Token: 0x060000CA RID: 202 RVA: 0x00005158 File Offset: 0x00003358
		[MonoPInvokeCallback]
		private static void OnRouteUpdateImpl(IntPtr ptr, string routeData)
		{
			Discord discord = (Discord)GCHandle.FromIntPtr(ptr).Target;
			if (discord.NetworkManagerInstance.OnRouteUpdate != null)
			{
				discord.NetworkManagerInstance.OnRouteUpdate(routeData);
			}
		}

		// Token: 0x0400010A RID: 266
		private IntPtr MethodsPtr;

		// Token: 0x0400010B RID: 267
		private object MethodsStructure;

		// Token: 0x0400010C RID: 268
		[CompilerGenerated]
		private NetworkManager.MessageHandler OnMessage;

		// Token: 0x0400010D RID: 269
		[CompilerGenerated]
		private NetworkManager.RouteUpdateHandler OnRouteUpdate;

		// Token: 0x020000BE RID: 190
		internal struct FFIEvents
		{
			// Token: 0x04000417 RID: 1047
			internal NetworkManager.FFIEvents.MessageHandler OnMessage;

			// Token: 0x04000418 RID: 1048
			internal NetworkManager.FFIEvents.RouteUpdateHandler OnRouteUpdate;

			// Token: 0x020001D4 RID: 468
			// (Invoke) Token: 0x06000835 RID: 2101
			[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
			internal delegate void MessageHandler(IntPtr ptr, ulong peerId, byte channelId, IntPtr dataPtr, int dataLen);

			// Token: 0x020001D5 RID: 469
			// (Invoke) Token: 0x06000839 RID: 2105
			[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
			internal delegate void RouteUpdateHandler(IntPtr ptr, [MarshalAs(UnmanagedType.LPStr)] string routeData);
		}

		// Token: 0x020000BF RID: 191
		internal struct FFIMethods
		{
			// Token: 0x04000419 RID: 1049
			internal NetworkManager.FFIMethods.GetPeerIdMethod GetPeerId;

			// Token: 0x0400041A RID: 1050
			internal NetworkManager.FFIMethods.FlushMethod Flush;

			// Token: 0x0400041B RID: 1051
			internal NetworkManager.FFIMethods.OpenPeerMethod OpenPeer;

			// Token: 0x0400041C RID: 1052
			internal NetworkManager.FFIMethods.UpdatePeerMethod UpdatePeer;

			// Token: 0x0400041D RID: 1053
			internal NetworkManager.FFIMethods.ClosePeerMethod ClosePeer;

			// Token: 0x0400041E RID: 1054
			internal NetworkManager.FFIMethods.OpenChannelMethod OpenChannel;

			// Token: 0x0400041F RID: 1055
			internal NetworkManager.FFIMethods.CloseChannelMethod CloseChannel;

			// Token: 0x04000420 RID: 1056
			internal NetworkManager.FFIMethods.SendMessageMethod SendMessage;

			// Token: 0x020001D6 RID: 470
			// (Invoke) Token: 0x0600083D RID: 2109
			[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
			internal delegate void GetPeerIdMethod(IntPtr methodsPtr, ref ulong peerId);

			// Token: 0x020001D7 RID: 471
			// (Invoke) Token: 0x06000841 RID: 2113
			[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
			internal delegate Result FlushMethod(IntPtr methodsPtr);

			// Token: 0x020001D8 RID: 472
			// (Invoke) Token: 0x06000845 RID: 2117
			[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
			internal delegate Result OpenPeerMethod(IntPtr methodsPtr, ulong peerId, [MarshalAs(UnmanagedType.LPStr)] string routeData);

			// Token: 0x020001D9 RID: 473
			// (Invoke) Token: 0x06000849 RID: 2121
			[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
			internal delegate Result UpdatePeerMethod(IntPtr methodsPtr, ulong peerId, [MarshalAs(UnmanagedType.LPStr)] string routeData);

			// Token: 0x020001DA RID: 474
			// (Invoke) Token: 0x0600084D RID: 2125
			[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
			internal delegate Result ClosePeerMethod(IntPtr methodsPtr, ulong peerId);

			// Token: 0x020001DB RID: 475
			// (Invoke) Token: 0x06000851 RID: 2129
			[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
			internal delegate Result OpenChannelMethod(IntPtr methodsPtr, ulong peerId, byte channelId, bool reliable);

			// Token: 0x020001DC RID: 476
			// (Invoke) Token: 0x06000855 RID: 2133
			[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
			internal delegate Result CloseChannelMethod(IntPtr methodsPtr, ulong peerId, byte channelId);

			// Token: 0x020001DD RID: 477
			// (Invoke) Token: 0x06000859 RID: 2137
			[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
			internal delegate Result SendMessageMethod(IntPtr methodsPtr, ulong peerId, byte channelId, byte[] data, int dataLen);
		}

		// Token: 0x020000C0 RID: 192
		// (Invoke) Token: 0x0600044F RID: 1103
		public delegate void MessageHandler(ulong peerId, byte channelId, byte[] data);

		// Token: 0x020000C1 RID: 193
		// (Invoke) Token: 0x06000453 RID: 1107
		public delegate void RouteUpdateHandler(string routeData);
	}
}
