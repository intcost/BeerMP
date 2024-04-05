using System;
using System.Linq;
using System.Runtime.CompilerServices;
using BeerMP.Helpers;
using HutongGames.PlayMaker;
using UnityEngine;

namespace BeerMP.Networking.Managers
{
	// Token: 0x02000077 RID: 119
	[ManagerCreate(10)]
	internal class NetFloorJackManager : MonoBehaviour
	{
		// Token: 0x0600036F RID: 879 RVA: 0x0001A00C File Offset: 0x0001820C
		private void Start()
		{
			NetEvent<NetFloorJackManager>.Register("Move", new NetEventHandler(this.OnMove));
			Transform transform = GameObject.Find("ITEMS").transform.Find("floor jack(itemx)");
			this.usageFsm = transform.Find("Trigger").GetPlayMaker("Use");
			this.y = this.usageFsm.FsmVariables.FindFsmFloat("Y");
			Action<bool> move = delegate(bool isUp)
			{
				if (this.receivedJackEvent)
				{
					this.receivedJackEvent = false;
					return;
				}
				using (Packet packet = new Packet(1))
				{
					packet.Write(isUp, -1);
					if (isUp)
					{
						packet.Write(this.y.Value, -1);
					}
					NetEvent<NetFloorJackManager>.Send("Move", packet, true);
				}
			};
			this.usageFsm.InsertAction("Up", delegate
			{
				move(true);
			}, 0);
			this.usageFsm.InsertAction("Down", delegate
			{
				move(false);
			}, 0);
			this.usageFsm.AddGlobalTransition(this.usageFsm.FsmEvents.First((FsmEvent e) => e.Name == "LIFT UP"), "Up");
			this.usageFsm.AddGlobalTransition(this.usageFsm.FsmEvents.First((FsmEvent e) => e.Name == "LIFT DOWN"), "Down");
			BeerMPGlobals.OnMemberReady += delegate(ulong user)
			{
				if (this.y.Value != 0f)
				{
					using (Packet packet2 = new Packet(1))
					{
						packet2.Write(true, -1);
						packet2.Write(this.y.Value, -1);
						NetEvent<NetFloorJackManager>.Send("Move", packet2, user, true);
					}
				}
			};
		}

		// Token: 0x06000370 RID: 880 RVA: 0x0001A174 File Offset: 0x00018374
		private void OnMove(ulong sender, Packet packet)
		{
			this.receivedJackEvent = true;
			bool flag = packet.ReadBool(true);
			if (flag)
			{
				this.y.Value = packet.ReadFloat(true);
			}
			this.usageFsm.SendEvent("LIFT " + (flag ? "UP" : "DOWN"));
		}

		// Token: 0x06000371 RID: 881 RVA: 0x0001A1C9 File Offset: 0x000183C9
		public NetFloorJackManager()
		{
		}

		// Token: 0x04000373 RID: 883
		private FsmFloat y;

		// Token: 0x04000374 RID: 884
		private PlayMakerFSM usageFsm;

		// Token: 0x04000375 RID: 885
		private bool receivedJackEvent;

		// Token: 0x0200014E RID: 334
		[CompilerGenerated]
		[Serializable]
		private sealed class <>c
		{
			// Token: 0x0600062D RID: 1581 RVA: 0x00021060 File Offset: 0x0001F260
			// Note: this type is marked as 'beforefieldinit'.
			static <>c()
			{
			}

			// Token: 0x0600062E RID: 1582 RVA: 0x0002106C File Offset: 0x0001F26C
			public <>c()
			{
			}

			// Token: 0x0600062F RID: 1583 RVA: 0x00021074 File Offset: 0x0001F274
			internal bool <Start>b__3_3(FsmEvent e)
			{
				return e.Name == "LIFT UP";
			}

			// Token: 0x06000630 RID: 1584 RVA: 0x00021086 File Offset: 0x0001F286
			internal bool <Start>b__3_4(FsmEvent e)
			{
				return e.Name == "LIFT DOWN";
			}

			// Token: 0x040005BB RID: 1467
			public static readonly NetFloorJackManager.<>c <>9 = new NetFloorJackManager.<>c();

			// Token: 0x040005BC RID: 1468
			public static Func<FsmEvent, bool> <>9__3_3;

			// Token: 0x040005BD RID: 1469
			public static Func<FsmEvent, bool> <>9__3_4;
		}

		// Token: 0x0200014F RID: 335
		[CompilerGenerated]
		private sealed class <>c__DisplayClass3_0
		{
			// Token: 0x06000631 RID: 1585 RVA: 0x00021098 File Offset: 0x0001F298
			public <>c__DisplayClass3_0()
			{
			}

			// Token: 0x06000632 RID: 1586 RVA: 0x000210A0 File Offset: 0x0001F2A0
			internal void <Start>b__0(bool isUp)
			{
				if (this.<>4__this.receivedJackEvent)
				{
					this.<>4__this.receivedJackEvent = false;
					return;
				}
				using (Packet packet = new Packet(1))
				{
					packet.Write(isUp, -1);
					if (isUp)
					{
						packet.Write(this.<>4__this.y.Value, -1);
					}
					NetEvent<NetFloorJackManager>.Send("Move", packet, true);
				}
			}

			// Token: 0x06000633 RID: 1587 RVA: 0x00021118 File Offset: 0x0001F318
			internal void <Start>b__1()
			{
				this.move(true);
			}

			// Token: 0x06000634 RID: 1588 RVA: 0x00021126 File Offset: 0x0001F326
			internal void <Start>b__2()
			{
				this.move(false);
			}

			// Token: 0x06000635 RID: 1589 RVA: 0x00021134 File Offset: 0x0001F334
			internal void <Start>b__5(ulong user)
			{
				if (this.<>4__this.y.Value != 0f)
				{
					using (Packet packet = new Packet(1))
					{
						packet.Write(true, -1);
						packet.Write(this.<>4__this.y.Value, -1);
						NetEvent<NetFloorJackManager>.Send("Move", packet, user, true);
					}
				}
			}

			// Token: 0x040005BE RID: 1470
			public NetFloorJackManager <>4__this;

			// Token: 0x040005BF RID: 1471
			public Action<bool> move;
		}
	}
}
