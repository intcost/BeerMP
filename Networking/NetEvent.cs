using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using BeerMP.Networking.Managers;
using Steamworks;

namespace BeerMP.Networking
{
	// Token: 0x02000048 RID: 72
	public class NetEvent<T> : IDisposable
	{
		// Token: 0x1700001C RID: 28
		// (get) Token: 0x06000162 RID: 354 RVA: 0x00007650 File Offset: 0x00005850
		// (set) Token: 0x06000163 RID: 355 RVA: 0x00007658 File Offset: 0x00005858
		public string name
		{
			[CompilerGenerated]
			get
			{
				return this.<name>k__BackingField;
			}
			[CompilerGenerated]
			internal set
			{
				this.<name>k__BackingField = value;
			}
		}

		// Token: 0x06000164 RID: 356 RVA: 0x00007664 File Offset: 0x00005864
		internal NetEvent(string name, NetEventHandler handler)
		{
			this.name = name;
			this.typeFullname = typeof(T).FullName;
			NetEvent<T>.instances.Add(this);
			if (!NetEventWorker.eventHandlers.ContainsKey(this.typeFullname))
			{
				NetEventWorker.eventHandlers.Add(this.typeFullname, new Dictionary<string, NetEventHandler>());
			}
			Dictionary<string, NetEventHandler> dictionary = NetEventWorker.eventHandlers[this.typeFullname];
			if (!dictionary.ContainsKey(name))
			{
				dictionary.Add(name, handler);
			}
		}

		// Token: 0x06000165 RID: 357 RVA: 0x000076E7 File Offset: 0x000058E7
		public static NetEvent<T> Register(string name, NetEventHandler handler)
		{
			return new NetEvent<T>(name, handler);
		}

		// Token: 0x06000166 RID: 358 RVA: 0x000076F0 File Offset: 0x000058F0
		public void Send(Packet packet, bool sendReliable = true)
		{
			NetEvent<T>.Send(this.name, packet, sendReliable);
		}

		// Token: 0x06000167 RID: 359 RVA: 0x00007700 File Offset: 0x00005900
		public static void Send(string name, Packet packet, bool sendReliable = true)
		{
			packet.Write(packet.scene, 0);
			packet.Write(packet.id, 0);
			packet.InsertString(name);
			packet.InsertString(typeof(T).FullName);
			if (sendReliable)
			{
				NetManager.SendReliable(packet, default(CSteamID));
				return;
			}
			NetManager.SendUnreliable(packet, default(CSteamID));
		}

		// Token: 0x06000168 RID: 360 RVA: 0x00007765 File Offset: 0x00005965
		public void Send(Packet packet, ulong target, bool sendReliable = true)
		{
			NetEvent<T>.Send(this.name, packet, target, sendReliable);
		}

		// Token: 0x06000169 RID: 361 RVA: 0x00007778 File Offset: 0x00005978
		public static void Send(string name, Packet packet, ulong target, bool sendReliable = true)
		{
			packet.Write(packet.scene, 0);
			packet.Write(packet.id, 0);
			packet.InsertString(name);
			packet.InsertString(typeof(T).FullName);
			if (sendReliable)
			{
				NetManager.SendReliable(packet, (CSteamID)target);
				return;
			}
			NetManager.SendUnreliable(packet, (CSteamID)target);
		}

		// Token: 0x0600016A RID: 362 RVA: 0x000077D7 File Offset: 0x000059D7
		public void Unregister()
		{
			NetEvent<T>.instances.Remove(this);
			NetEventWorker.eventHandlers[this.typeFullname].Remove(this.name);
		}

		// Token: 0x0600016B RID: 363 RVA: 0x00007801 File Offset: 0x00005A01
		public void Dispose()
		{
			GC.SuppressFinalize(this);
			this.Unregister();
		}

		// Token: 0x0600016C RID: 364 RVA: 0x0000780F File Offset: 0x00005A0F
		// Note: this type is marked as 'beforefieldinit'.
		static NetEvent()
		{
		}

		// Token: 0x0400013E RID: 318
		[CompilerGenerated]
		private string <name>k__BackingField;

		// Token: 0x0400013F RID: 319
		internal string typeFullname;

		// Token: 0x04000140 RID: 320
		internal static List<NetEvent<T>> instances = new List<NetEvent<T>>();
	}
}
