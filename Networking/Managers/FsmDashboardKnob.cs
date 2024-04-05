using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using BeerMP.Helpers;
using HutongGames.PlayMaker;
using HutongGames.PlayMaker.Actions;
using Steamworks;

namespace BeerMP.Networking.Managers
{
	// Token: 0x02000066 RID: 102
	internal class FsmDashboardKnob
	{
		// Token: 0x060002C0 RID: 704 RVA: 0x0001437C File Offset: 0x0001257C
		public FsmDashboardKnob(PlayMakerFSM fsm)
		{
			this.fsm = fsm;
			this.hash = fsm.transform.GetGameobjectHashString().GetHashCode();
			this.SetupFSM();
			if (FsmDashboardKnob.updateEvent == null)
			{
				FsmDashboardKnob.updateEvent = NetEvent<FsmDashboardKnob>.Register("Twist", new NetEventHandler(FsmDashboardKnob.OnKnobUpdate));
			}
			if (!FsmDashboardKnob.initSyncLoaded)
			{
				FsmDashboardKnob.initSyncLoaded = true;
				BeerMPGlobals.OnMemberReady += delegate(ulong u)
				{
					if (!BeerMPGlobals.IsHost)
					{
						return;
					}
					for (int i = 0; i < FsmDashboardKnob.knobs.Count; i++)
					{
						FsmDashboardKnob.knobs[i].SendKnobUpdate();
					}
				};
			}
			FsmDashboardKnob.knobs.Add(this);
			NetManager.sceneLoaded = (Action<GameScene>)Delegate.Combine(NetManager.sceneLoaded, new Action<GameScene>(delegate(GameScene a)
			{
				if (FsmDashboardKnob.knobs.Contains(this))
				{
					FsmDashboardKnob.knobs.Remove(this);
				}
			}));
		}

		// Token: 0x060002C1 RID: 705 RVA: 0x0001443C File Offset: 0x0001263C
		private void SetupFSM()
		{
			this.fsm.Initialize();
			try
			{
				FsmState state = this.fsm.GetState("Increase");
				string toState = state.Transitions[0].ToState;
				for (int i = 0; i < state.Actions.Length; i++)
				{
					FloatAdd floatAdd = state.Actions[i] as FloatAdd;
					if (floatAdd != null)
					{
						this.targetValue = floatAdd.floatVariable;
						this.add = floatAdd.add.Value;
						break;
					}
				}
				this.update = this.fsm.AddEvent("MP_UPDATE");
				this.fsm.AddGlobalTransition(this.update, "Increase");
				this.fsm.InsertAction(toState, new Action(this.SendKnobUpdate), 0);
			}
			catch (Exception ex)
			{
				Console.LogError(string.Format("Failed to setup dashboard knob {0} ({1}): {2}, {3}, {4}", new object[]
				{
					this.hash,
					this.fsm.transform.GetGameobjectHashString(),
					ex.GetType(),
					ex.Message,
					ex.StackTrace
				}), false);
			}
		}

		// Token: 0x060002C2 RID: 706 RVA: 0x00014568 File Offset: 0x00012768
		private static void OnKnobUpdate(ulong sender, Packet packet)
		{
			if (!NetRadioManager.radioLoaded)
			{
				return;
			}
			int hash = packet.ReadInt(true);
			float num = packet.ReadFloat(true);
			FsmDashboardKnob fsmDashboardKnob = FsmDashboardKnob.knobs.FirstOrDefault((FsmDashboardKnob b) => b.hash == hash);
			if (fsmDashboardKnob == null)
			{
				Console.LogError(string.Format("Received dashboard knob triggered action from {0} but the hash {1} cannot be found", NetManager.playerNames[(CSteamID)sender], hash), false);
				return;
			}
			fsmDashboardKnob.updating = true;
			fsmDashboardKnob.targetValue.Value = num - fsmDashboardKnob.add;
			fsmDashboardKnob.fsm.Fsm.Event(fsmDashboardKnob.update);
		}

		// Token: 0x060002C3 RID: 707 RVA: 0x00014610 File Offset: 0x00012810
		private void SendKnobUpdate()
		{
			if (!NetRadioManager.radioLoaded)
			{
				return;
			}
			if (this.updating)
			{
				this.updating = false;
				return;
			}
			using (Packet packet = new Packet(1))
			{
				packet.Write(this.hash, -1);
				packet.Write(this.targetValue.Value, -1);
				FsmDashboardKnob.updateEvent.Send(packet, true);
			}
		}

		// Token: 0x060002C4 RID: 708 RVA: 0x00014684 File Offset: 0x00012884
		// Note: this type is marked as 'beforefieldinit'.
		static FsmDashboardKnob()
		{
		}

		// Token: 0x060002C5 RID: 709 RVA: 0x00014696 File Offset: 0x00012896
		[CompilerGenerated]
		private void <.ctor>b__9_1(GameScene a)
		{
			if (FsmDashboardKnob.knobs.Contains(this))
			{
				FsmDashboardKnob.knobs.Remove(this);
			}
		}

		// Token: 0x040002C2 RID: 706
		private PlayMakerFSM fsm;

		// Token: 0x040002C3 RID: 707
		private FsmFloat targetValue;

		// Token: 0x040002C4 RID: 708
		private FsmEvent update;

		// Token: 0x040002C5 RID: 709
		private float add;

		// Token: 0x040002C6 RID: 710
		private int hash;

		// Token: 0x040002C7 RID: 711
		private bool updating;

		// Token: 0x040002C8 RID: 712
		private static NetEvent<FsmDashboardKnob> updateEvent;

		// Token: 0x040002C9 RID: 713
		private static List<FsmDashboardKnob> knobs = new List<FsmDashboardKnob>();

		// Token: 0x040002CA RID: 714
		private static bool initSyncLoaded = false;

		// Token: 0x02000118 RID: 280
		[CompilerGenerated]
		[Serializable]
		private sealed class <>c
		{
			// Token: 0x0600058E RID: 1422 RVA: 0x0001F5EF File Offset: 0x0001D7EF
			// Note: this type is marked as 'beforefieldinit'.
			static <>c()
			{
			}

			// Token: 0x0600058F RID: 1423 RVA: 0x0001F5FB File Offset: 0x0001D7FB
			public <>c()
			{
			}

			// Token: 0x06000590 RID: 1424 RVA: 0x0001F604 File Offset: 0x0001D804
			internal void <.ctor>b__9_0(ulong u)
			{
				if (!BeerMPGlobals.IsHost)
				{
					return;
				}
				for (int i = 0; i < FsmDashboardKnob.knobs.Count; i++)
				{
					FsmDashboardKnob.knobs[i].SendKnobUpdate();
				}
			}

			// Token: 0x04000545 RID: 1349
			public static readonly FsmDashboardKnob.<>c <>9 = new FsmDashboardKnob.<>c();

			// Token: 0x04000546 RID: 1350
			public static Action<ulong> <>9__9_0;
		}

		// Token: 0x02000119 RID: 281
		[CompilerGenerated]
		private sealed class <>c__DisplayClass11_0
		{
			// Token: 0x06000591 RID: 1425 RVA: 0x0001F63E File Offset: 0x0001D83E
			public <>c__DisplayClass11_0()
			{
			}

			// Token: 0x06000592 RID: 1426 RVA: 0x0001F646 File Offset: 0x0001D846
			internal bool <OnKnobUpdate>b__0(FsmDashboardKnob b)
			{
				return b.hash == this.hash;
			}

			// Token: 0x04000547 RID: 1351
			public int hash;
		}
	}
}
