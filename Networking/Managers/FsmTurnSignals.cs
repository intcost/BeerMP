using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using BeerMP.Helpers;
using HutongGames.PlayMaker;

namespace BeerMP.Networking.Managers
{
	// Token: 0x02000065 RID: 101
	internal class FsmTurnSignals
	{
		// Token: 0x060002B7 RID: 695 RVA: 0x00014018 File Offset: 0x00012218
		public FsmTurnSignals(PlayMakerFSM fsm)
		{
			this.hash = fsm.transform.GetGameobjectHashString().GetHashCode();
			this.fsm = fsm;
			this.SetupFSM();
			if (FsmTurnSignals.updateEvent == null)
			{
				FsmTurnSignals.updateEvent = NetEvent<FsmTurnSignals>.Register("Update", new NetEventHandler(FsmTurnSignals.OnUpdate));
			}
			if (!FsmTurnSignals.initSyncLoaded)
			{
				FsmTurnSignals.initSyncLoaded = true;
				BeerMPGlobals.OnMemberReady += delegate(ulong u)
				{
					if (!BeerMPGlobals.IsHost)
					{
						return;
					}
					for (int i = 0; i < FsmTurnSignals.turnSignals.Count; i++)
					{
						FsmTurnSignals.turnSignals[i].SendUpdate(FsmTurnSignals.turnSignals[i].current, u);
					}
				};
			}
			FsmTurnSignals.turnSignals.Add(this);
			NetManager.sceneLoaded = (Action<GameScene>)Delegate.Combine(NetManager.sceneLoaded, new Action<GameScene>(delegate(GameScene a)
			{
				if (FsmTurnSignals.turnSignals.Contains(this))
				{
					FsmTurnSignals.turnSignals.Remove(this);
				}
			}));
		}

		// Token: 0x060002B8 RID: 696 RVA: 0x000140E0 File Offset: 0x000122E0
		private void SetupFSM()
		{
			FsmState state = this.fsm.GetState("State 2");
			string toState = state.Transitions.First((FsmTransition t) => t.EventName == "LEFT").ToState;
			string toState2 = state.Transitions.First((FsmTransition t) => t.EventName == "RIGHT").ToState;
			this.fsm.InsertAction("State 3", delegate
			{
				this.SendUpdate(0, 0UL);
			}, 0);
			this.fsm.AddGlobalTransition(this.fsm.AddEvent(FsmTurnSignals.eventNames[0]), "State 3");
			this.fsm.InsertAction(toState, delegate
			{
				this.SendUpdate(1, 0UL);
			}, 0);
			this.fsm.AddGlobalTransition(this.fsm.AddEvent(FsmTurnSignals.eventNames[1]), toState);
			this.fsm.InsertAction(toState2, delegate
			{
				this.SendUpdate(2, 0UL);
			}, 0);
			this.fsm.AddGlobalTransition(this.fsm.AddEvent(FsmTurnSignals.eventNames[2]), toState2);
		}

		// Token: 0x060002B9 RID: 697 RVA: 0x0001420C File Offset: 0x0001240C
		private void SendUpdate(int i, ulong target = 0UL)
		{
			if (this.updating == i)
			{
				this.updating = -1;
				return;
			}
			using (Packet packet = new Packet(1))
			{
				packet.Write(this.hash, -1);
				packet.Write((byte)i, -1);
				this.current = i;
				if (target == 0UL)
				{
					FsmTurnSignals.updateEvent.Send(packet, true);
				}
				else
				{
					FsmTurnSignals.updateEvent.Send(packet, target, true);
				}
			}
		}

		// Token: 0x060002BA RID: 698 RVA: 0x00014288 File Offset: 0x00012488
		private static void OnUpdate(ulong sender, Packet p)
		{
			int hash = p.ReadInt(true);
			FsmTurnSignals fsmTurnSignals = FsmTurnSignals.turnSignals.FirstOrDefault((FsmTurnSignals ts) => ts.hash == hash);
			if (fsmTurnSignals == null)
			{
				Console.LogError(string.Format("Received turn signal of hash {0} update, but it does not exist", hash), false);
				return;
			}
			int num = (int)p.ReadByte(true);
			fsmTurnSignals.updating = (fsmTurnSignals.current = num);
			fsmTurnSignals.fsm.SendEvent(FsmTurnSignals.eventNames[num]);
		}

		// Token: 0x060002BB RID: 699 RVA: 0x00014308 File Offset: 0x00012508
		// Note: this type is marked as 'beforefieldinit'.
		static FsmTurnSignals()
		{
		}

		// Token: 0x060002BC RID: 700 RVA: 0x0001433D File Offset: 0x0001253D
		[CompilerGenerated]
		private void <.ctor>b__8_1(GameScene a)
		{
			if (FsmTurnSignals.turnSignals.Contains(this))
			{
				FsmTurnSignals.turnSignals.Remove(this);
			}
		}

		// Token: 0x060002BD RID: 701 RVA: 0x00014358 File Offset: 0x00012558
		[CompilerGenerated]
		private void <SetupFSM>b__9_2()
		{
			this.SendUpdate(0, 0UL);
		}

		// Token: 0x060002BE RID: 702 RVA: 0x00014363 File Offset: 0x00012563
		[CompilerGenerated]
		private void <SetupFSM>b__9_3()
		{
			this.SendUpdate(1, 0UL);
		}

		// Token: 0x060002BF RID: 703 RVA: 0x0001436E File Offset: 0x0001256E
		[CompilerGenerated]
		private void <SetupFSM>b__9_4()
		{
			this.SendUpdate(2, 0UL);
		}

		// Token: 0x040002BA RID: 698
		private PlayMakerFSM fsm;

		// Token: 0x040002BB RID: 699
		private int hash;

		// Token: 0x040002BC RID: 700
		private int updating = -1;

		// Token: 0x040002BD RID: 701
		private int current;

		// Token: 0x040002BE RID: 702
		private static readonly string[] eventNames = new string[] { "MP_OFF", "MP_LEFT", "MP_RIGHT" };

		// Token: 0x040002BF RID: 703
		private static NetEvent<FsmTurnSignals> updateEvent;

		// Token: 0x040002C0 RID: 704
		private static List<FsmTurnSignals> turnSignals = new List<FsmTurnSignals>();

		// Token: 0x040002C1 RID: 705
		private static bool initSyncLoaded = false;

		// Token: 0x02000116 RID: 278
		[CompilerGenerated]
		[Serializable]
		private sealed class <>c
		{
			// Token: 0x06000587 RID: 1415 RVA: 0x0001F551 File Offset: 0x0001D751
			// Note: this type is marked as 'beforefieldinit'.
			static <>c()
			{
			}

			// Token: 0x06000588 RID: 1416 RVA: 0x0001F55D File Offset: 0x0001D75D
			public <>c()
			{
			}

			// Token: 0x06000589 RID: 1417 RVA: 0x0001F568 File Offset: 0x0001D768
			internal void <.ctor>b__8_0(ulong u)
			{
				if (!BeerMPGlobals.IsHost)
				{
					return;
				}
				for (int i = 0; i < FsmTurnSignals.turnSignals.Count; i++)
				{
					FsmTurnSignals.turnSignals[i].SendUpdate(FsmTurnSignals.turnSignals[i].current, u);
				}
			}

			// Token: 0x0600058A RID: 1418 RVA: 0x0001F5B3 File Offset: 0x0001D7B3
			internal bool <SetupFSM>b__9_0(FsmTransition t)
			{
				return t.EventName == "LEFT";
			}

			// Token: 0x0600058B RID: 1419 RVA: 0x0001F5C5 File Offset: 0x0001D7C5
			internal bool <SetupFSM>b__9_1(FsmTransition t)
			{
				return t.EventName == "RIGHT";
			}

			// Token: 0x04000540 RID: 1344
			public static readonly FsmTurnSignals.<>c <>9 = new FsmTurnSignals.<>c();

			// Token: 0x04000541 RID: 1345
			public static Action<ulong> <>9__8_0;

			// Token: 0x04000542 RID: 1346
			public static Func<FsmTransition, bool> <>9__9_0;

			// Token: 0x04000543 RID: 1347
			public static Func<FsmTransition, bool> <>9__9_1;
		}

		// Token: 0x02000117 RID: 279
		[CompilerGenerated]
		private sealed class <>c__DisplayClass11_0
		{
			// Token: 0x0600058C RID: 1420 RVA: 0x0001F5D7 File Offset: 0x0001D7D7
			public <>c__DisplayClass11_0()
			{
			}

			// Token: 0x0600058D RID: 1421 RVA: 0x0001F5DF File Offset: 0x0001D7DF
			internal bool <OnUpdate>b__0(FsmTurnSignals ts)
			{
				return ts.hash == this.hash;
			}

			// Token: 0x04000544 RID: 1348
			public int hash;
		}
	}
}
