using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using BeerMP.Helpers;
using HutongGames.PlayMaker;
using UnityEngine;

namespace BeerMP.Networking.Managers
{
	// Token: 0x02000072 RID: 114
	[ManagerCreate(10)]
	internal class NetSwitchManager : MonoBehaviour
	{
		// Token: 0x06000354 RID: 852 RVA: 0x000192A8 File Offset: 0x000174A8
		private void Start()
		{
			NetSwitchManager.Instance = this;
			GameObject[] array = Resources.FindObjectsOfTypeAll<GameObject>().Where((GameObject x) => x.name.StartsWith("switch")).ToArray<GameObject>();
			for (int i = 0; i < array.Length; i++)
			{
				NetSwitchManager.Switch.Create(array[i]);
			}
			this.ToggleSwitch = NetEvent<NetSwitchManager>.Register("ToggleSwitch", new NetEventHandler(this.OnToggleSwitch));
			BeerMPGlobals.OnMemberReady += delegate(ulong sender)
			{
				if (BeerMPGlobals.IsHost)
				{
					foreach (NetSwitchManager.Switch @switch in NetSwitchManager.switches)
					{
						@switch.SyncStateInitial(sender);
					}
				}
			};
		}

		// Token: 0x06000355 RID: 853 RVA: 0x0001934C File Offset: 0x0001754C
		private void OnToggleSwitch(ulong userId, Packet packet)
		{
			if (userId == BeerMPGlobals.UserID)
			{
				return;
			}
			Vector3 switchPos = packet.ReadVector3(true);
			bool flag = packet.ReadBool(true);
			NetSwitchManager.switches.First((NetSwitchManager.Switch x) => x.switchPos == switchPos).Toggle(flag);
		}

		// Token: 0x06000356 RID: 854 RVA: 0x00019399 File Offset: 0x00017599
		public NetSwitchManager()
		{
		}

		// Token: 0x06000357 RID: 855 RVA: 0x000193A1 File Offset: 0x000175A1
		// Note: this type is marked as 'beforefieldinit'.
		static NetSwitchManager()
		{
		}

		// Token: 0x04000352 RID: 850
		public static NetSwitchManager Instance;

		// Token: 0x04000353 RID: 851
		internal static List<NetSwitchManager.Switch> switches = new List<NetSwitchManager.Switch>();

		// Token: 0x04000354 RID: 852
		private NetEvent<NetSwitchManager> ToggleSwitch;

		// Token: 0x02000145 RID: 325
		public class Switch
		{
			// Token: 0x0600060A RID: 1546 RVA: 0x000208A8 File Offset: 0x0001EAA8
			public static void Create(GameObject switchObject)
			{
				if (!switchObject.name.StartsWith("switch"))
				{
					return;
				}
				PlayMakerFSM playMaker = switchObject.GetPlayMaker("Use");
				if (playMaker == null)
				{
					return;
				}
				playMaker.Initialize();
				if (playMaker.FsmStates.Any((FsmState x) => x.Name == "Switch"))
				{
					if (playMaker.FsmStates.Any((FsmState x) => x.Name == "Position"))
					{
						NetSwitchManager.Switch @switch = new NetSwitchManager.Switch();
						@switch.switchPos = switchObject.transform.position;
						@switch.fsm = playMaker;
						FsmEvent fsmEvent = playMaker.AddEvent("MP_UpdateSwitch");
						@switch.fsmEvent = fsmEvent;
						if (!playMaker.FsmStates.Any((FsmState x) => x.Name == "Switch"))
						{
							@switch.switchOn = playMaker.FsmVariables.FindFsmBool("SwitchOn");
							playMaker.AddGlobalTransition(fsmEvent, "Position");
							playMaker.InsertAction("Position", new NetSwitchManager.SwitchAction
							{
								sw = @switch
							}, 0);
						}
						else
						{
							@switch.switchOn = playMaker.FsmVariables.FindFsmBool("Switch");
							playMaker.AddGlobalTransition(fsmEvent, "Switch");
							playMaker.InsertAction("Switch", new NetSwitchManager.SwitchAction
							{
								sw = @switch
							}, 0);
						}
						NetSwitchManager.switches.Add(@switch);
						return;
					}
				}
			}

			// Token: 0x0600060B RID: 1547 RVA: 0x00020A1B File Offset: 0x0001EC1B
			public void Toggle(bool on)
			{
				if (this.switchOn.Value == on)
				{
					return;
				}
				this.doSync = false;
				this.fsm.Fsm.Event(this.fsmEvent);
			}

			// Token: 0x0600060C RID: 1548 RVA: 0x00020A4C File Offset: 0x0001EC4C
			public void SyncStateInitial(ulong userId)
			{
				using (Packet packet = new Packet(1))
				{
					packet.Write(this.switchPos, -1);
					packet.Write(this.switchOn.Value, -1);
					NetEvent<NetSwitchManager>.Send("ToggleSwitch", packet, userId, true);
				}
			}

			// Token: 0x0600060D RID: 1549 RVA: 0x00020AA8 File Offset: 0x0001ECA8
			public Switch()
			{
			}

			// Token: 0x040005A4 RID: 1444
			public Vector3 switchPos;

			// Token: 0x040005A5 RID: 1445
			public PlayMakerFSM fsm;

			// Token: 0x040005A6 RID: 1446
			public FsmBool switchOn;

			// Token: 0x040005A7 RID: 1447
			public FsmEvent fsmEvent;

			// Token: 0x040005A8 RID: 1448
			public bool doSync = true;

			// Token: 0x02000220 RID: 544
			[CompilerGenerated]
			[Serializable]
			private sealed class <>c
			{
				// Token: 0x06000966 RID: 2406 RVA: 0x0002176E File Offset: 0x0001F96E
				// Note: this type is marked as 'beforefieldinit'.
				static <>c()
				{
				}

				// Token: 0x06000967 RID: 2407 RVA: 0x0002177A File Offset: 0x0001F97A
				public <>c()
				{
				}

				// Token: 0x06000968 RID: 2408 RVA: 0x00021782 File Offset: 0x0001F982
				internal bool <Create>b__0_0(FsmState x)
				{
					return x.Name == "Switch";
				}

				// Token: 0x06000969 RID: 2409 RVA: 0x00021794 File Offset: 0x0001F994
				internal bool <Create>b__0_1(FsmState x)
				{
					return x.Name == "Position";
				}

				// Token: 0x0600096A RID: 2410 RVA: 0x000217A6 File Offset: 0x0001F9A6
				internal bool <Create>b__0_2(FsmState x)
				{
					return x.Name == "Switch";
				}

				// Token: 0x040005DF RID: 1503
				public static readonly NetSwitchManager.Switch.<>c <>9 = new NetSwitchManager.Switch.<>c();

				// Token: 0x040005E0 RID: 1504
				public static Func<FsmState, bool> <>9__0_0;

				// Token: 0x040005E1 RID: 1505
				public static Func<FsmState, bool> <>9__0_1;

				// Token: 0x040005E2 RID: 1506
				public static Func<FsmState, bool> <>9__0_2;
			}
		}

		// Token: 0x02000146 RID: 326
		public class SwitchAction : FsmStateAction
		{
			// Token: 0x0600060E RID: 1550 RVA: 0x00020AB8 File Offset: 0x0001ECB8
			public override void OnEnter()
			{
				using (Packet packet = new Packet(1))
				{
					packet.Write(this.sw.switchPos, -1);
					packet.Write(!this.sw.switchOn.Value, -1);
					if (this.sw.doSync)
					{
						NetEvent<NetSwitchManager>.Send("ToggleSwitch", packet, true);
					}
				}
				this.sw.doSync = true;
				base.Finish();
			}

			// Token: 0x0600060F RID: 1551 RVA: 0x00020B40 File Offset: 0x0001ED40
			public SwitchAction()
			{
			}

			// Token: 0x040005A9 RID: 1449
			public NetSwitchManager.Switch sw;
		}

		// Token: 0x02000147 RID: 327
		[CompilerGenerated]
		[Serializable]
		private sealed class <>c
		{
			// Token: 0x06000610 RID: 1552 RVA: 0x00020B48 File Offset: 0x0001ED48
			// Note: this type is marked as 'beforefieldinit'.
			static <>c()
			{
			}

			// Token: 0x06000611 RID: 1553 RVA: 0x00020B54 File Offset: 0x0001ED54
			public <>c()
			{
			}

			// Token: 0x06000612 RID: 1554 RVA: 0x00020B5C File Offset: 0x0001ED5C
			internal bool <Start>b__3_0(GameObject x)
			{
				return x.name.StartsWith("switch");
			}

			// Token: 0x06000613 RID: 1555 RVA: 0x00020B70 File Offset: 0x0001ED70
			internal void <Start>b__3_1(ulong sender)
			{
				if (BeerMPGlobals.IsHost)
				{
					foreach (NetSwitchManager.Switch @switch in NetSwitchManager.switches)
					{
						@switch.SyncStateInitial(sender);
					}
				}
			}

			// Token: 0x040005AA RID: 1450
			public static readonly NetSwitchManager.<>c <>9 = new NetSwitchManager.<>c();

			// Token: 0x040005AB RID: 1451
			public static Func<GameObject, bool> <>9__3_0;

			// Token: 0x040005AC RID: 1452
			public static Action<ulong> <>9__3_1;
		}

		// Token: 0x02000148 RID: 328
		[CompilerGenerated]
		private sealed class <>c__DisplayClass4_0
		{
			// Token: 0x06000614 RID: 1556 RVA: 0x00020BC8 File Offset: 0x0001EDC8
			public <>c__DisplayClass4_0()
			{
			}

			// Token: 0x06000615 RID: 1557 RVA: 0x00020BD0 File Offset: 0x0001EDD0
			internal bool <OnToggleSwitch>b__0(NetSwitchManager.Switch x)
			{
				return x.switchPos == this.switchPos;
			}

			// Token: 0x040005AD RID: 1453
			public Vector3 switchPos;
		}
	}
}
