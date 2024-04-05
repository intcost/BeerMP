using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using HutongGames.PlayMaker;
using UnityEngine;

namespace BeerMP.Helpers
{
	// Token: 0x02000082 RID: 130
	internal static class PlayMakerUtilities
	{
		// Token: 0x060003A7 RID: 935 RVA: 0x0001B39C File Offset: 0x0001959C
		public static PlayMakerFSM GetPlayMaker(this GameObject go, string fsmName)
		{
			return go.GetComponentsInChildren<PlayMakerFSM>(true).FirstOrDefault((PlayMakerFSM x) => x.FsmName == fsmName && x.transform == go.transform);
		}

		// Token: 0x060003A8 RID: 936 RVA: 0x0001B3DA File Offset: 0x000195DA
		public static PlayMakerFSM GetPlayMaker(this Transform tf, string fsmName)
		{
			return tf.gameObject.GetPlayMaker(fsmName);
		}

		// Token: 0x060003A9 RID: 937 RVA: 0x0001B3E8 File Offset: 0x000195E8
		public static void Initialize(this PlayMakerFSM fsm)
		{
			fsm.Fsm.InitData();
		}

		// Token: 0x060003AA RID: 938 RVA: 0x0001B3F8 File Offset: 0x000195F8
		public static FsmEvent AddEvent(this PlayMakerFSM fsm, string eventName)
		{
			fsm.Fsm.InitData();
			FsmEvent fsmEvent = FsmEvent.GetFsmEvent(eventName);
			if (fsmEvent == null)
			{
				fsmEvent = new FsmEvent(eventName);
			}
			List<FsmEvent> list = fsm.FsmEvents.ToList<FsmEvent>();
			list.Add(fsmEvent);
			fsm.Fsm.Events = list.ToArray();
			fsm.Fsm.InitData();
			return fsmEvent;
		}

		// Token: 0x060003AB RID: 939 RVA: 0x0001B454 File Offset: 0x00019654
		public static void AddGlobalTransition(this PlayMakerFSM pm, FsmEvent fsmEvent, string stateName)
		{
			pm.Fsm.InitData();
			List<FsmTransition> list = pm.Fsm.GlobalTransitions.ToList<FsmTransition>();
			list.Add(new FsmTransition
			{
				FsmEvent = fsmEvent,
				ToState = stateName
			});
			pm.Fsm.GlobalTransitions = list.ToArray();
			pm.Fsm.InitData();
		}

		// Token: 0x060003AC RID: 940 RVA: 0x0001B4B4 File Offset: 0x000196B4
		public static bool HasState(this PlayMakerFSM pm, string stateName)
		{
			return pm.FsmStates.Any((FsmState x) => x.Name == stateName);
		}

		// Token: 0x060003AD RID: 941 RVA: 0x0001B4E8 File Offset: 0x000196E8
		public static FsmState GetState(this PlayMakerFSM pm, string stateName)
		{
			return pm.FsmStates.FirstOrDefault((FsmState x) => x.Name == stateName);
		}

		// Token: 0x060003AE RID: 942 RVA: 0x0001B51C File Offset: 0x0001971C
		public static void InsertAction<T>(this PlayMakerFSM pm, string stateName, T action, int index = -1) where T : FsmStateAction
		{
			FsmState state = pm.GetState(stateName);
			if (state == null)
			{
				Console.Log(string.Concat(new string[]
				{
					"InsertAction(FSM, string, T, int): The state of name ",
					stateName,
					" is null on fsm ",
					pm.FsmName,
					" on object ",
					pm.gameObject.name
				}), true);
				return;
			}
			List<FsmStateAction> list = state.Actions.ToList<FsmStateAction>();
			if (index == -1)
			{
				list.Add(action);
			}
			else
			{
				list.Insert(index, action);
			}
			state.Actions = list.ToArray();
		}

		// Token: 0x060003AF RID: 943 RVA: 0x0001B5B0 File Offset: 0x000197B0
		public static void InsertAction(this PlayMakerFSM pm, string stateName, Action action, int index = -1)
		{
			pm.InsertAction(stateName, new PlayMakerUtilities.PM_Hook(action, false), index);
		}

		// Token: 0x02000154 RID: 340
		internal class PM_Hook : FsmStateAction
		{
			// Token: 0x06000647 RID: 1607 RVA: 0x000213C5 File Offset: 0x0001F5C5
			public PM_Hook(Action action, bool everyFrame = false)
			{
				this.action = action;
				this.everyFrame = everyFrame;
			}

			// Token: 0x06000648 RID: 1608 RVA: 0x000213DB File Offset: 0x0001F5DB
			public override void OnEnter()
			{
				Action action = this.action;
				if (action != null)
				{
					action();
				}
				if (!this.everyFrame)
				{
					base.Finish();
				}
			}

			// Token: 0x06000649 RID: 1609 RVA: 0x000213FC File Offset: 0x0001F5FC
			public override void OnUpdate()
			{
				if (this.everyFrame)
				{
					Action action = this.action;
					if (action == null)
					{
						return;
					}
					action();
				}
			}

			// Token: 0x040005C9 RID: 1481
			public Action action;

			// Token: 0x040005CA RID: 1482
			public bool everyFrame;
		}

		// Token: 0x02000155 RID: 341
		[CompilerGenerated]
		private sealed class <>c__DisplayClass0_0
		{
			// Token: 0x0600064A RID: 1610 RVA: 0x00021416 File Offset: 0x0001F616
			public <>c__DisplayClass0_0()
			{
			}

			// Token: 0x0600064B RID: 1611 RVA: 0x0002141E File Offset: 0x0001F61E
			internal bool <GetPlayMaker>b__0(PlayMakerFSM x)
			{
				return x.FsmName == this.fsmName && x.transform == this.go.transform;
			}

			// Token: 0x040005CB RID: 1483
			public string fsmName;

			// Token: 0x040005CC RID: 1484
			public GameObject go;
		}

		// Token: 0x02000156 RID: 342
		[CompilerGenerated]
		private sealed class <>c__DisplayClass5_0
		{
			// Token: 0x0600064C RID: 1612 RVA: 0x0002144B File Offset: 0x0001F64B
			public <>c__DisplayClass5_0()
			{
			}

			// Token: 0x0600064D RID: 1613 RVA: 0x00021453 File Offset: 0x0001F653
			internal bool <HasState>b__0(FsmState x)
			{
				return x.Name == this.stateName;
			}

			// Token: 0x040005CD RID: 1485
			public string stateName;
		}

		// Token: 0x02000157 RID: 343
		[CompilerGenerated]
		private sealed class <>c__DisplayClass6_0
		{
			// Token: 0x0600064E RID: 1614 RVA: 0x00021466 File Offset: 0x0001F666
			public <>c__DisplayClass6_0()
			{
			}

			// Token: 0x0600064F RID: 1615 RVA: 0x0002146E File Offset: 0x0001F66E
			internal bool <GetState>b__0(FsmState x)
			{
				return x.Name == this.stateName;
			}

			// Token: 0x040005CE RID: 1486
			public string stateName;
		}
	}
}
