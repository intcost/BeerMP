using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using BeerMP.Helpers;
using HutongGames.PlayMaker;
using Steamworks;

namespace BeerMP.Networking.Managers
{
	// Token: 0x02000068 RID: 104
	internal class FsmDashboardButton
	{
		// Token: 0x060002D3 RID: 723 RVA: 0x00014DD0 File Offset: 0x00012FD0
		public FsmDashboardButton(PlayMakerFSM fsm)
		{
			this.fsm = fsm;
			this.hash = fsm.transform.GetGameobjectHashString().GetHashCode();
			this.SetupFSM();
			if (FsmDashboardButton.knobToggleEvent == null)
			{
				FsmDashboardButton.knobToggleEvent = NetEvent<FsmDashboardButton>.Register("Toggle", new NetEventHandler(FsmDashboardButton.OnTriggeredAction));
			}
			if (!FsmDashboardButton.initSyncLoaded)
			{
				FsmDashboardButton.initSyncLoaded = true;
				BeerMPGlobals.OnMemberReady += delegate(ulong u)
				{
					if (!BeerMPGlobals.IsHost)
					{
						return;
					}
					for (int i = 0; i < FsmDashboardButton.dashboardButtons.Count; i++)
					{
						int num = FsmDashboardButton.dashboardButtons[i].currentAction;
						FsmDashboardButton.dashboardButtons[i].TriggeredAction(num, u);
					}
				};
			}
			FsmDashboardButton.dashboardButtons.Add(this);
			NetManager.sceneLoaded = (Action<GameScene>)Delegate.Combine(NetManager.sceneLoaded, new Action<GameScene>(delegate(GameScene a)
			{
				if (FsmDashboardButton.dashboardButtons.Contains(this))
				{
					FsmDashboardButton.dashboardButtons.Remove(this);
				}
			}));
		}

		// Token: 0x060002D4 RID: 724 RVA: 0x00014EA0 File Offset: 0x000130A0
		private void SetupFSM()
		{
			FsmState fsmState = this.fsm.FsmStates.FirstOrDefault((FsmState s) => s.Name.Contains("Test"));
			this.actionEvents = new string[fsmState.Transitions.Length];
			for (int i = 0; i < this.actionEvents.Length; i++)
			{
				string text = "MP_" + fsmState.Transitions[i].EventName;
				this.actionEvents[i] = text;
				FsmEvent fsmEvent = this.fsm.AddEvent(text);
				this.fsm.AddGlobalTransition(fsmEvent, fsmState.Transitions[i].ToState);
				int index = i;
				this.fsm.InsertAction(fsmState.Transitions[i].ToState, delegate
				{
					this.TriggeredAction(index, 0UL);
				}, 0);
			}
		}

		// Token: 0x060002D5 RID: 725 RVA: 0x00014F8C File Offset: 0x0001318C
		private void TriggeredAction(int index, ulong target = 0UL)
		{
			if (this.updatingAction == index)
			{
				this.updatingAction = -1;
				return;
			}
			using (Packet packet = new Packet(1))
			{
				packet.Write(this.hash, -1);
				packet.Write((byte)index, -1);
				this.currentAction = index;
				if (target == 0UL)
				{
					FsmDashboardButton.knobToggleEvent.Send(packet, true);
				}
				else
				{
					FsmDashboardButton.knobToggleEvent.Send(packet, target, true);
				}
			}
		}

		// Token: 0x060002D6 RID: 726 RVA: 0x00015008 File Offset: 0x00013208
		private static void OnTriggeredAction(ulong sender, Packet p)
		{
			int hash = p.ReadInt(true);
			FsmDashboardButton fsmDashboardButton = FsmDashboardButton.dashboardButtons.FirstOrDefault((FsmDashboardButton b) => b.hash == hash);
			if (fsmDashboardButton == null)
			{
				Console.LogError(string.Format("Received dashboard button triggered action from {0} but the hash {1} cannot be found", NetManager.playerNames[(CSteamID)sender], hash), false);
				return;
			}
			int num = (int)p.ReadByte(true);
			if (num == 255)
			{
				return;
			}
			fsmDashboardButton.updatingAction = (fsmDashboardButton.currentAction = num);
			fsmDashboardButton.fsm.SendEvent(fsmDashboardButton.actionEvents[num]);
		}

		// Token: 0x060002D7 RID: 727 RVA: 0x000150A2 File Offset: 0x000132A2
		// Note: this type is marked as 'beforefieldinit'.
		static FsmDashboardButton()
		{
		}

		// Token: 0x060002D8 RID: 728 RVA: 0x000150B4 File Offset: 0x000132B4
		[CompilerGenerated]
		private void <.ctor>b__8_1(GameScene a)
		{
			if (FsmDashboardButton.dashboardButtons.Contains(this))
			{
				FsmDashboardButton.dashboardButtons.Remove(this);
			}
		}

		// Token: 0x040002DD RID: 733
		private PlayMakerFSM fsm;

		// Token: 0x040002DE RID: 734
		private string[] actionEvents;

		// Token: 0x040002DF RID: 735
		private int hash;

		// Token: 0x040002E0 RID: 736
		private int updatingAction = -1;

		// Token: 0x040002E1 RID: 737
		private int currentAction = 255;

		// Token: 0x040002E2 RID: 738
		private static NetEvent<FsmDashboardButton> knobToggleEvent;

		// Token: 0x040002E3 RID: 739
		private static List<FsmDashboardButton> dashboardButtons = new List<FsmDashboardButton>();

		// Token: 0x040002E4 RID: 740
		private static bool initSyncLoaded = false;

		// Token: 0x0200011D RID: 285
		[CompilerGenerated]
		[Serializable]
		private sealed class <>c
		{
			// Token: 0x0600059F RID: 1439 RVA: 0x0001F7FC File Offset: 0x0001D9FC
			// Note: this type is marked as 'beforefieldinit'.
			static <>c()
			{
			}

			// Token: 0x060005A0 RID: 1440 RVA: 0x0001F808 File Offset: 0x0001DA08
			public <>c()
			{
			}

			// Token: 0x060005A1 RID: 1441 RVA: 0x0001F810 File Offset: 0x0001DA10
			internal void <.ctor>b__8_0(ulong u)
			{
				if (!BeerMPGlobals.IsHost)
				{
					return;
				}
				for (int i = 0; i < FsmDashboardButton.dashboardButtons.Count; i++)
				{
					int currentAction = FsmDashboardButton.dashboardButtons[i].currentAction;
					FsmDashboardButton.dashboardButtons[i].TriggeredAction(currentAction, u);
				}
			}

			// Token: 0x060005A2 RID: 1442 RVA: 0x0001F85D File Offset: 0x0001DA5D
			internal bool <SetupFSM>b__9_0(FsmState s)
			{
				return s.Name.Contains("Test");
			}

			// Token: 0x04000551 RID: 1361
			public static readonly FsmDashboardButton.<>c <>9 = new FsmDashboardButton.<>c();

			// Token: 0x04000552 RID: 1362
			public static Action<ulong> <>9__8_0;

			// Token: 0x04000553 RID: 1363
			public static Func<FsmState, bool> <>9__9_0;
		}

		// Token: 0x0200011E RID: 286
		[CompilerGenerated]
		private sealed class <>c__DisplayClass11_0
		{
			// Token: 0x060005A3 RID: 1443 RVA: 0x0001F86F File Offset: 0x0001DA6F
			public <>c__DisplayClass11_0()
			{
			}

			// Token: 0x060005A4 RID: 1444 RVA: 0x0001F877 File Offset: 0x0001DA77
			internal bool <OnTriggeredAction>b__0(FsmDashboardButton b)
			{
				return b.hash == this.hash;
			}

			// Token: 0x04000554 RID: 1364
			public int hash;
		}

		// Token: 0x0200011F RID: 287
		[CompilerGenerated]
		private sealed class <>c__DisplayClass9_0
		{
			// Token: 0x060005A5 RID: 1445 RVA: 0x0001F887 File Offset: 0x0001DA87
			public <>c__DisplayClass9_0()
			{
			}

			// Token: 0x060005A6 RID: 1446 RVA: 0x0001F88F File Offset: 0x0001DA8F
			internal void <SetupFSM>b__1()
			{
				this.<>4__this.TriggeredAction(this.index, 0UL);
			}

			// Token: 0x04000555 RID: 1365
			public int index;

			// Token: 0x04000556 RID: 1366
			public FsmDashboardButton <>4__this;
		}
	}
}
