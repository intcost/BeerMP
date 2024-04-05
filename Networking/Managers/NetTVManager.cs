using System;
using System.Runtime.CompilerServices;
using BeerMP.Helpers;
using HutongGames.PlayMaker;
using UnityEngine;

namespace BeerMP.Networking.Managers
{
	// Token: 0x02000074 RID: 116
	[ManagerCreate(10)]
	internal class NetTVManager : MonoBehaviour
	{
		// Token: 0x0600035E RID: 862 RVA: 0x00019714 File Offset: 0x00017914
		private void Start()
		{
			NetEvent<NetTVManager>.Register("On", new NetEventHandler(this.OnTVToggle));
			this.TVswitch = GameObject.Find("YARD").transform.Find("Building/LIVINGROOM/TV/Switch").GetPlayMaker("Use");
			this.isOn = this.TVswitch.FsmVariables.FindFsmBool("Open");
			FsmEvent fsmEvent = this.TVswitch.AddEvent("MP_OPEN");
			Action<ulong, bool> a = delegate(ulong target, bool init)
			{
				using (Packet packet = new Packet(1))
				{
					packet.Write(init ? this.isOn.Value : (!this.isOn.Value), -1);
					if (target == 0UL)
					{
						NetEvent<NetTVManager>.Send("On", packet, true);
					}
					else
					{
						NetEvent<NetTVManager>.Send("On", packet, target, true);
					}
				}
			};
			this.TVswitch.InsertAction("Switch", delegate
			{
				a(0UL, false);
			}, 0);
			this.TVswitch.AddGlobalTransition(fsmEvent, "State 5");
			BeerMPGlobals.OnMemberReady += delegate(ulong user)
			{
				a(user, true);
			};
		}

		// Token: 0x0600035F RID: 863 RVA: 0x000197F8 File Offset: 0x000179F8
		private void OnTVToggle(ulong sender, Packet packet)
		{
			bool flag = packet.ReadBool(true);
			this.TVswitch.SendEvent(flag ? "MP_OPEN" : "GLOBALEVENT");
		}

		// Token: 0x06000360 RID: 864 RVA: 0x00019827 File Offset: 0x00017A27
		public NetTVManager()
		{
		}

		// Token: 0x0400035C RID: 860
		private PlayMakerFSM TVswitch;

		// Token: 0x0400035D RID: 861
		private FsmBool isOn;

		// Token: 0x0200014B RID: 331
		[CompilerGenerated]
		private sealed class <>c__DisplayClass2_0
		{
			// Token: 0x0600061B RID: 1563 RVA: 0x00020CDB File Offset: 0x0001EEDB
			public <>c__DisplayClass2_0()
			{
			}

			// Token: 0x0600061C RID: 1564 RVA: 0x00020CE4 File Offset: 0x0001EEE4
			internal void <Start>b__0(ulong target, bool init)
			{
				using (Packet packet = new Packet(1))
				{
					packet.Write(init ? this.<>4__this.isOn.Value : (!this.<>4__this.isOn.Value), -1);
					if (target == 0UL)
					{
						NetEvent<NetTVManager>.Send("On", packet, true);
					}
					else
					{
						NetEvent<NetTVManager>.Send("On", packet, target, true);
					}
				}
			}

			// Token: 0x0600061D RID: 1565 RVA: 0x00020D64 File Offset: 0x0001EF64
			internal void <Start>b__1()
			{
				this.a(0UL, false);
			}

			// Token: 0x0600061E RID: 1566 RVA: 0x00020D74 File Offset: 0x0001EF74
			internal void <Start>b__2(ulong user)
			{
				this.a(user, true);
			}

			// Token: 0x040005B4 RID: 1460
			public NetTVManager <>4__this;

			// Token: 0x040005B5 RID: 1461
			public Action<ulong, bool> a;
		}
	}
}
